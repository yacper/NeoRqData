/********************************************************************
    created:	2021/6/24 17:41:06
    author:		rush
    email:		yacper@gmail.com	
	
    purpose:
    modifiers:	Finnhub的一个连接
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RLib.Base;
using RLib.Base.Utils;

namespace NeoRqData
{
    public partial class RqDataClient : ObservableObject, IRqDataClient
    {
        public RqDataClient(string acccount, string pwd, int callsPerMin = 300)
        {
            Acccount_ = acccount;
            Pwd_ = pwd;

            CallsPerMin = callsPerMin;
        }

        public string ApiKey => ApiKey_;
        public int CallsPerMin { get; set; }

        public int            CallsInScope
        {
            get { return _Calls.Count; }
        }

        protected List<DateTime> _Calls = new List<DateTime>();
        protected async Task _CheckCalls()
        {
            DateTime now = DateTime.Now;
            _Calls.Add(now);

            if (_Calls.Count < CallsPerMin)
                return;

            while (_Calls.Any())
            {
                if (now - _Calls[0] >= TimeSpan.FromMinutes(1))
                {
                    _Calls.RemoveAt(0);
                }
                else
                    break;
            }

            if (_Calls.Count >= CallsPerMin)
            {
                Thread.Sleep(TimeSpan.FromMinutes(1) - (now - _Calls[0]));
            }
        }


        public string       LastErrMsg
        {
            get { return _LastErrMsg; }
            set
            {
                //Set(nameof(LastErrMsg), ref _LastErrMsg, value);
                SetProperty(ref _LastErrMsg, value);
            }
        }

        public EConnectionState ConnectionState { get => ConnectionState_; set => SetProperty(ref ConnectionState_, value); }

        public async Task<bool>   Connect(string user = null, string pwd = null)
        {
            if (ConnectionState == EConnectionState.Disconnected)
            {
                if (user != null)
                    Acccount_ = user;
                if (pwd != null)
                    Pwd_ = pwd;

                //error:auth failed
                //认证失败
                ConnectionState = EConnectionState.Connecting;

                var ret = await AuthUrl.PostJsonAsync(new
                {
                    user_name = Acccount_,
                    password  = Pwd_
                }).ReceiveString();

                if (ret.Contains("error"))
                {
                    LastErrMsg = ret;

                    ConnectionState = EConnectionState.Disconnected;

                    return false;
                }

                ApiKey_ = ret;

                ConnectionState = EConnectionState.Connected;
            }

            return ConnectionState == EConnectionState.Connected;
        }

        public async Task<int> get_query_count()
        {
            var ret = await ApiUrl.PostJsonAsync(new
            {
                method = "get_query_count",
                token=ApiKey_
            }).ReceiveString();


            return Convert.ToInt32(ret);
        }

		/*
,order_book_id,underlying_symbol,market_tplus,symbol,margin_rate,maturity_date,type,trading_code,exchange,product,contract_multiplier,round_lot,trading_hours,listed_date,industry_name,de_listed_date,underlying_order_book_id
0,A0303,A,0.0,豆一0303,0.05,2003-03-14,Future,a0303,DCE,Commodity,10.0,1.0,"21:01-23:00,09:01-10:15,10:31-11:30,13:31-15:00",2002-03-15,油脂,2003-03-14,null
1,A0305,A,0.0,豆一0305,0.05,2003-05-23,Future,a0305,DCE,Commodity,10.0,1.0,"21:01-23:00,09:01-10:15,10:31-11:30,13:31-15:00",2002-03-15,油脂,2003-05-23,null
...
121,A2305,A,0.0,黄大豆1号2305,0.12,2023-05-18,Future,a2305,DCE,Commodity,10.0,1.0,"21:01-23:00,09:01-10:15,10:31-11:30,13:31-15:00",2022-05-19,油脂,2023-05-18,
122,A2307,A,0.0,黄大豆1号2307,0.12,2023-07-14,Future,a2307,DCE,Commodity,10.0,1.0,"21:01-23:00,09:01-10:15,10:31-11:30,13:31-15:00",2022-07-15,油脂,2023-07-14,
123,A88,A,0.0,黄大豆号主力连续,0.12,0000-00-00,Future,A2211,DCE,Commodity,10.0,1.0,"21:01-23:00,09:01-10:15,10:31-11:30,13:31-15:00",0000-00-00,油脂,0000-00-00,
124,A888,A,0.0,黄大豆号主力连续价差平滑,0.12,0000-00-00,Future,A2211,DCE,Commodity,10.0,1.0,"21:01-23:00,09:01-10:15,10:31-11:30,13:31-15:00",0000-00-00,油脂,0000-00-00,
125,A889,A,0.0,黄大豆号主力连续价差平滑（后复权）,0.12,0000-00-00,Future,A2211,DCE,Commodity,10.0,1.0,"21:01-23:00,09:01-10:15,10:31-11:30,13:31-15:00",0000-00-00,油脂,0000-00-00,
126,A99,A,0.0,黄大豆号指数连续,0.12,0000-00-00,Future,a2205,DCE,Commodity,10.0,1.0,"21:01-23:00,09:01-10:15,10:31-11:30,13:31-15:00",0000-00-00,油脂,0000-00-00,
         */
		public async Task<List<SecurityInfo>> all_instruments(ESymbolType type, EMarket market = EMarket.cn, DateTime? date = null)
		{
			var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
			{
				method         = "all_instruments",
				type = type.ToString(),
				market = market.ToString(),
				date = date?.Date??null
			}).ReceiveStream();
			//}).ReceiveString();

			List<SecurityInfo> rtn = ret.FromCsv<SecurityInfo>();
			return rtn;
		}

		public async Task<SecurityInfo> instruments(string order_book_id)
		{
			return (await instruments(order_book_id.ToEnumerable())).FirstOrDefault();
		}

		public async Task<List<SecurityInfo>> instruments(IEnumerable<string> order_book_ids)
		{
			var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
			{
				method         = "instruments",
				order_book_ids = order_book_ids,
			}).ReceiveStream();
			//}).ReceiveString();

			List<SecurityInfo> rtn = ret.FromCsv<SecurityInfo>();
			return rtn;
		}




		public async Task<SecurityInfo> get_security_info(string code)
        {
            //await _CheckCalls();

            var ret = await ApiUrl.PostJsonAsync(new
            {
                method = "get_security_info",
                code=code,
                token=ApiKey_
            }).ReceiveStream();

            SecurityInfo rtn = ret.FromCsv<SecurityInfo>().FirstOrDefault();

            return rtn;
        }

        public async Task<List<Bar>> get_price(string order_book_id,   DateTime    start_date,                   DateTime end_date,               ETimeFrame frequency = ETimeFrame.D1,
            IEnumerable<string>                                    fields    = null, EAdjustType adjust_type = EAdjustType.pre, bool     skip_suspended = false, EMarket    market    = EMarket.cn,
            bool                                                   expect_df = true, string      time_slice = null)
        {
            var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
            {
                method      = "get_price",
                order_book_ids        = order_book_id,
                start_date = start_date.ToString(),
                end_date = end_date.ToString(),
                frequency = frequency.ToResolutionString(),
                fields = fields,
                adjust_type = adjust_type.ToString(),
                skip_suspended = skip_suspended,
                market = market.ToString(),
                expect_df = expect_df,
                time_slice = time_slice

                }).ReceiveStream();
            //}).ReceiveString();

            List<Bar> rtn = ret.FromCsv<Bar>();

            return rtn;

        }



        //public async Task<List<Bar>> get_price(string code, ETimeFrame timeframe = ETimeFrame.m1, int count = 5000,
        //    DateTime? endDate = null, DateTime? fq_ref_date = null)
        //{
        //    //code: 证券代码
        //    //count: 大于0的整数，表示获取bar的条数，不能超过5000
        //    //unit: bar的时间单位, 支持如下周期：1m, 5m, 15m, 30m, 60m, 120m, 1d, 1w, 1M。其中m表示分钟，d表示天，w表示周，M表示月
        //    //end_date：查询的截止时间，默认是今天
        //    //fq_ref_date：复权基准日期，该参数为空时返回不复权数据
        //    if (endDate == null)
        //        endDate = DateTime.Now;

        //    var ret = await ApiUrl.PostJsonAsync(new
        //    {
        //        method = "get_price",
        //        code=code,
        //        unit=timeframe.ToResolutionString(),
        //        count=count,
        //        end_date=endDate.Value.ToJqDate(),
        //        fq_ref_date=fq_ref_date==null?endDate.Value.ToJqDate():fq_ref_date.Value.ToJqDate(),
        //        token=ApiKey_
        //    }).ReceiveStream();

        //    List<Bar> rtn = ret.FromCsv<Bar>();

        //    return rtn;
        //}
        public async Task<List<Bar>>     get_price_period(string code, ETimeFrame timeframe, DateTime date, DateTime? endDate = null, DateTime? fq_ref_date = null)
        {
            //指定开始时间date和结束时间end_date时间段，获取行情数据
            //code: 证券代码
            //unit: bar的时间单位, 支持如下周期：1m, 5m, 15m, 30m, 60m, 120m, 1d, 1w, 1M。其中m表示分钟，d表示天，w表示周，M表示月
            //date : 开始时间，不能为空，格式2018-07-03或2018-07-03 10:40:00，如果是2018-07-03则默认为2018-07-03 00:00:00
            //end_date：结束时间，不能为空，格式2018-07-03或2018-07-03 10:40:00，如果是2018-07-03则默认为2018-07-03 23:59:00
            //fq_ref_date：复权基准日期，该参数为空时返回不复权数据
            //注：当unit是1w或1M时，第一条数据是开始时间date所在的周或月的行情。当unit为分钟时，第一条数据是开始时间date所在的一个unit切片的行情。 最大获取1000个交易日数据
            if (endDate == null)
                endDate = DateTime.Now;

            var ret = await ApiUrl.PostJsonAsync(new
            {
                method = "get_price_period",
                code=code,
                unit=timeframe.ToResolutionString(),
                date=date.ToJqDate(),
                end_date=endDate.Value.ToJqDate(),
                fq_ref_date=fq_ref_date==null?endDate.Value.ToJqDate():fq_ref_date.Value.ToJqDate(),
                token=ApiKey_
            }).ReceiveStream();

            List<Bar> rtn = ret.FromCsv<Bar>();

            return rtn;
        }

        public async Task<List<CurrentPrice>> get_current_price(IEnumerable<string> codes)
        {
            var ret = await ApiUrl.PostJsonAsync(new
            {
                method = "get_current_price",
                code= codes.Join(','),
                token=ApiKey_
            }).ReceiveStream();

            List<CurrentPrice> rtn = ret.FromCsv<CurrentPrice>();

            return rtn;

        }
       
        public async Task<List<Tick>> get_ticks(string order_book_id)
        {
			var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
			{
				method         = "get_ticks",
				order_book_id = order_book_id,
			}).ReceiveStream();
			//}).ReceiveString();

			List<Tick> rtn = ret.FromCsv<Tick>();

			return rtn;
		}

		//获取期权合约 2020 年 3 月 9 日 9 时 40 分 00 秒-2020 年 3 月 9 日 9 时 40 分 02 秒之间的 tick 数据
		//get_live_ticks(order_book_ids=['10002726'], start_dt= '20210309094000', end_dt= '20210309094002')
		public async Task<List<Tick>>  get_live_ticks(string order_book_id, Tuple<DateTime, DateTime> range = null, IEnumerable<string> fields = null)
        {
			var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
			{
				method         = "get_live_ticks",
				order_book_ids = order_book_id,
				start_dt     = range != null?range.Item1.ToString():null,
				end_dt       =range != null?range.Item2.ToString():null,
				fields = fields
            }).ReceiveStream();
            //}).ReceiveString();


			List<Tick> rtn = ret.FromCsv<Tick>();

			return rtn;
		}


		#region 期货

		//获取某期货品种在指定日期下的可交易合约标的列表
		//code: 期货合约品种，如 AG (白银)
		//date: 指定日期
		public async Task<List<string>> get_contracts(string underlying_symbol, DateTime? date = null)
        {
           	var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
			{
				method         = "futures.get_contracts",
				underlying_symbol = underlying_symbol,
				date     = date != null?date.Value.ToString():null,
            //}).ReceiveStream();
            }).ReceiveString();

			List<string>         rtn = ret.Split('\n').ToList();
            rtn.RemoveAt(0);

			return rtn;
        }

        public async Task<List<DominantFuture>> get_dominant(string underlying_symbol, DateTime? start_date = null, DateTime? end_date = null, int rule = 0, int rank = 1)
        {
			var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
			{
				method         = "futures.get_dominant",
				underlying_symbol = underlying_symbol,
				start_date     = start_date != null?start_date.Value.ToString():null,
				end_date     = end_date != null?end_date.Value.ToString():null,
				rule =rule,
				rank=rank
            }).ReceiveStream();
            //}).ReceiveString();


			List<DominantFuture> rtn = ret.FromCsv<DominantFuture>();

			return rtn;
		}

        #endregion


        protected string ApiKey_;
        protected string Acccount_;
        protected string Pwd_;

        protected string _LastErrMsg;

        protected EConnectionState ConnectionState_;
    }
}
