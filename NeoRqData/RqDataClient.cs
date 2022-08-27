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
using Websocket.Client;

namespace NeoRqData
{
	public partial class RqDataClient:ObservableObject, IRqDataClient
	{
		public RqDataClient(string acccount, string pwd, int callsPerMin = 300)
		{
			Acccount_ = acccount;
			Pwd_ = pwd;

			CallsPerMin = callsPerMin;
		}

		public string ApiKey => ApiKey_;
		public int CallsPerMin { get; set; }

		public int CallsInScope
		{
			get { return _Calls.Count; }
		}

		protected List<DateTime> _Calls = new List<DateTime>();
		protected async Task _CheckCalls()
		{
			DateTime now = DateTime.Now;
			_Calls.Add(now);

			if(_Calls.Count < CallsPerMin)
				return;

			while(_Calls.Any())
			{
				if(now - _Calls[0] >= TimeSpan.FromMinutes(1))
				{
					_Calls.RemoveAt(0);
				}
				else
					break;
			}

			if(_Calls.Count >= CallsPerMin)
			{
				Thread.Sleep(TimeSpan.FromMinutes(1) - (now - _Calls[0]));
			}
		}


		public string LastErrMsg
		{
			get { return _LastErrMsg; }
			set
			{
				//Set(nameof(LastErrMsg), ref _LastErrMsg, value);
				SetProperty(ref _LastErrMsg, value);
			}
		}

		public EConnectionState ConnectionState { get => ConnectionState_; set => SetProperty(ref ConnectionState_, value); }

		public async Task<bool> Connect(string user = null, string pwd = null)
		{
			if(ConnectionState == EConnectionState.Disconnected)
			{
				if(user != null)
					Acccount_ = user;
				if(pwd != null)
					Pwd_ = pwd;

				//error:auth failed
				//认证失败
				ConnectionState = EConnectionState.Connecting;

				var ret = await AuthUrl.PostJsonAsync(new
				{
					user_name = Acccount_,
					password  = Pwd_
				}).ReceiveString();

				if(ret.Contains("error"))
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

		public async Task<string> info()
		{// 无用
			/*
                rqdatac version: 2.6.0
                sever address: rqdatad-pro.ricequant.com:16011
                You are using account: +86186XXXX6610
             */

			try
			{
				var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
				{
					method         = "info",
					//}).ReceiveStream();
				}).ReceiveString();

				return ret;
			}
			catch(Exception e)
			{
				LastErrMsg = e.ToString();
				return "";
			}
		}

		public async Task<QuoteInfo> get_quota()
		{
			/*
                name,value
                bytes_used,383802
                bytes_limit,52428800.0
                remaining_days,0
                license_type,OTHER
             */

			var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
			{
				method         = "user.get_quota",
			}).ReceiveStream();
			//}).ReceiveString();

			var rtn = ret.FromCsv<KeyValue>();

			return new QuoteInfo()
			{
				bytes_used = Convert.ToDouble(rtn.FirstOrDefault(p => p.name == nameof(QuoteInfo.bytes_used)).value),
				bytes_limit = Convert.ToDouble(rtn.FirstOrDefault(p => p.name == nameof(QuoteInfo.bytes_limit)).value),
				remaining_days = Convert.ToInt32(rtn.FirstOrDefault(p => p.name == nameof(QuoteInfo.remaining_days)).value),
				license_type = Enum.Parse<license_type>(rtn.FirstOrDefault(p => p.name == nameof(QuoteInfo.license_type)).value),
			};
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
			try
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
			catch(Exception e)
			{
				LastErrMsg = e.ToString();
				return new();
			}
		}

		public async Task<SecurityInfo> instruments(string order_book_id)
		{
			return (await instruments(order_book_id.ToEnumerable())).FirstOrDefault();
		}

		public async Task<List<SecurityInfo>> instruments(IEnumerable<string> order_book_ids)
		{
			try
			{
				// 必须用大写，否则不识别
				var idUppers = order_book_ids.Select(p => p.ToUpper());

				var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
				{
					method         = "instruments",
					order_book_ids = idUppers,
				}).ReceiveStream();
				//}).ReceiveString();

				List<SecurityInfo> rtn = ret.FromCsv<SecurityInfo>();
				return rtn;
			}
			catch(Exception e)
			{
				LastErrMsg = e.ToString();
				return new();
			}
		}

		public async Task<List<Bar>> get_price(string order_book_id, DateTime start_date, DateTime end_date, ETimeFrame frequency = ETimeFrame.D1,
			IEnumerable<string> fields = null, EAdjustType adjust_type = EAdjustType.pre, bool skip_suspended = false, EMarket market = EMarket.cn,
			bool expect_df = true, string time_slice = null)
		{
			try
			{

				var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
				{
					method      = "get_price",
					order_book_ids        = order_book_id.ToUpper(),
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
			catch(Exception e)
			{
				LastErrMsg = e.ToString();
				return new();
			}
		}

		public async Task<List<Tick>> get_ticks(string order_book_id)
		{
			try
			{
				var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
				{
					method         = "get_ticks",
					order_book_id = order_book_id.ToUpper(),
				}).ReceiveStream();
				//}).ReceiveString();

				List<Tick> rtn = ret.FromCsv<Tick>();

				return rtn;
			}
			catch(Exception e)
			{
				LastErrMsg = e.ToString();
				return new();
			}
		}

		//获取期权合约 2020 年 3 月 9 日 9 时 40 分 00 秒-2020 年 3 月 9 日 9 时 40 分 02 秒之间的 tick 数据
		//get_live_ticks(order_book_ids=['10002726'], start_dt= '20210309094000', end_dt= '20210309094002')
		public async Task<List<Tick>> get_live_ticks(string order_book_id, Tuple<DateTime, DateTime> range = null, IEnumerable<string> fields = null)
		{
			try
			{
				var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
				{
					method         = "get_live_ticks",
					order_book_ids = order_book_id.ToUpper(),
					start_dt     = range != null?range.Item1.ToString():null,
					end_dt       =range != null?range.Item2.ToString():null,
					fields = fields
				}).ReceiveStream();
				//}).ReceiveString();


				List<Tick> rtn = ret.FromCsv<Tick>();

				return rtn;
			}
			catch(Exception e)
			{
				LastErrMsg = e.ToString();
				return new();
			}
		}


#region websocket realtime

		// rqdata 实时行情推送
		//https://www.ricequant.com/doc/rqdata/python/generic-api.html#%E5%AE%9E%E6%97%B6%E8%A1%8C%E6%83%85%E6%8E%A8%E9%80%81
		public async Task StartWebsocket()
		{// 
			var wsUrl = WebsocketUrl
				.SetQueryParam("token", ApiKey);

			_WebsocketClient = new WebsocketClient(wsUrl.ToUri());
			_WebsocketClient.ReconnectTimeout = TimeSpan.FromSeconds(30);
			_WebsocketClient.ReconnectionHappened.Subscribe(info =>
			System.Diagnostics.Debug.WriteLine($"RqData Reconnection happened, type: {info.Type}")
			);

			_WebsocketClient.MessageReceived.Subscribe(msg =>
				{
					var rtn = msg.Text.ToJsonObj<WebsocketRtn>();
					switch(rtn.Type)
					{
						case EWebsocketRtnType.Error:

							break;
						case EWebsocketRtnType.Ping:

							break;

						case EWebsocketRtnType.Trade:
							//rtn.Data.ForEach(p => OnTrade?.Invoke(p));
							break;
					}

					System.Diagnostics.Debug.WriteLine($"Message received: {msg}");
				}

				);

			await _WebsocketClient.Start();
		}
		public void Subscribe(string order_book_id, ETimeFrame tf)
		{
		SubscribeReq req = new SubscribeReq() { symbol = order_book_id };

            string str = req.ToJson();
//            string str = "{\"type\":\"subscribe\",\"symbol\":\"BINANCE:BTCUSDT\"}";
            byte[] bsend = System.Text.Encoding.UTF8.GetBytes(str);
            _WebsocketClient.Send(bsend); 
            System.Diagnostics.Debug.WriteLine(str);

		}
        //public void                Subscribe(IEnumerable<string> order_book_ids);
        public void UnSubscribe(string order_book_id, ETimeFrame tf)
        {

        }
        //public void                UnSubscribe(IEnumerable<string> order_book_ids);

        //event Action<Trade> OnTrade;                                        // ontrade 更新
#endregion




		#region 期货

		//获取某期货品种在指定日期下的可交易合约标的列表
		//code: 期货合约品种，如 AG (白银)
		//date: 指定日期
		public async Task<List<string>> get_contracts(string underlying_symbol, DateTime? date = null)
		{
			try
			{
				var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
				{
					method         = "futures.get_contracts",
					underlying_symbol = underlying_symbol.ToUpper(),
					date     = date != null?date.Value.ToString():null,
					//}).ReceiveStream();
				}).ReceiveString();

				List<string>         rtn = ret.Split('\n').ToList();
				rtn.RemoveAt(0);

				return rtn;
			}
			catch(Exception e)
			{
				LastErrMsg = e.ToString();
				return new();
			}
		}

		public async Task<List<DominantFuture>> get_dominant(string underlying_symbol, DateTime? start_date = null, DateTime? end_date = null, int rule = 0, int rank = 1)
		{
			try
			{
				var ret = await ApiUrl.WithHeader("token", ApiKey).PostJsonAsync(new
				{
					method         = "futures.get_dominant",
					underlying_symbol = underlying_symbol.ToUpper(),
					start_date     = start_date != null?start_date.Value.ToString():null,
					end_date     = end_date != null?end_date.Value.ToString():null,
					rule =rule,
					rank=rank
				}).ReceiveStream();
				//}).ReceiveString();


				List<DominantFuture> rtn = ret.FromCsv<DominantFuture>();

				return rtn;

			}
			catch(Exception e)
			{
				LastErrMsg = e.ToString();
				return new();
			}
		}
		#endregion


		protected string ApiKey_;
		protected string Acccount_;
		protected string Pwd_;

		protected string _LastErrMsg;

		protected EConnectionState ConnectionState_;

        protected WebsocketClient _WebsocketClient;
	}
}
