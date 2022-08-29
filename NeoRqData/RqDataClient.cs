﻿/********************************************************************
    created:	2021/6/24 17:41:06
    author:		rush
    email:		yacper@gmail.com	
	
    purpose:
    modifiers:	Finnhub的一个连接
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		public RqDataClient(string acccount, string pwd, string licenseKey, int callsPerMin = 300)
		{
			Acccount   = acccount;
			Pwd        = pwd;
            LicenseKey = licenseKey;

			CallsPerMin = callsPerMin;
		}

		public int CallsPerMin { get; set; }

		public int CallsInScope
		{
			get { return _Calls.Count; }
		}

        public int TimeoutMilliseconds { get; set; } = 5000;

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

        public event EventHandler<string> OnErrorEvent;

		public EConnectionState ConnectionState { get => ConnectionState_; set => SetProperty(ref ConnectionState_, value); }

		public async Task<bool> Connect(string user = null, string pwd = null, string licenseKey = null)
		{
			if(ConnectionState == EConnectionState.Disconnected)
			{
				if(user != null)
					Acccount = user;
				if(pwd != null)
					Pwd = pwd;
                if (licenseKey != null)
                    LicenseKey = licenseKey;

				//error:auth failed
				//认证失败
				ConnectionState = EConnectionState.Connecting;

				var ret = await AuthUrl.PostJsonAsync(new
				{
					user_name = Acccount,
					password  = Pwd
				}).ReceiveString();

				if(ret.Contains("error"))
				{
					OnErrorEvent?.Invoke(this, ret);

					ConnectionState = EConnectionState.Disconnected;

					return false;
				}

				Token = ret;

                var r2 = await StartWebsocket();
                if (r2)
                    ConnectionState = EConnectionState.Connected;
                else
                    ConnectionState = EConnectionState.Disconnected;
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
				var ret = await ApiUrl.WithHeader("token", Token).PostJsonAsync(new
				{
					method         = "info",
					//}).ReceiveStream();
				}).ReceiveString();

				return ret;
			}
			catch(Exception e)
			{
                OnErrorEvent?.Invoke(this, e.ToString());
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

			var ret = await ApiUrl.WithHeader("token", Token).PostJsonAsync(new
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
				var ret = await ApiUrl.WithHeader("token", Token).PostJsonAsync(new
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
                OnErrorEvent?.Invoke(this, e.ToString());
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

				var ret = await ApiUrl.WithHeader("token", Token).PostJsonAsync(new
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
                OnErrorEvent?.Invoke(this, e.ToString());
				return new();
			}
		}

		public async Task<List<Bar>> get_price(string order_book_id, DateTime start_date, DateTime end_date, ETimeFrame frequency = ETimeFrame.D1,
			IEnumerable<string> fields = null, EAdjustType adjust_type = EAdjustType.pre, bool skip_suspended = false, EMarket market = EMarket.cn,
			bool expect_df = true, string time_slice = null)
		{
			try
			{

				var ret = await ApiUrl.WithHeader("token", Token).PostJsonAsync(new
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
                OnErrorEvent?.Invoke(this, e.ToString());
				return new();
			}
		}

		public async Task<List<Tick>> get_ticks(string order_book_id)
		{
			try
			{
				var ret = await ApiUrl.WithHeader("token", Token).PostJsonAsync(new
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
                OnErrorEvent?.Invoke(this, e.ToString());
				return new();
			}
		}

		//获取期权合约 2020 年 3 月 9 日 9 时 40 分 00 秒-2020 年 3 月 9 日 9 时 40 分 02 秒之间的 tick 数据
		//get_live_ticks(order_book_ids=['10002726'], start_dt= '20210309094000', end_dt= '20210309094002')
		public async Task<List<Tick>> get_live_ticks(string order_book_id, Tuple<DateTime, DateTime> range = null, IEnumerable<string> fields = null)
		{
			try
			{
				var ret = await ApiUrl.WithHeader("token", Token).PostJsonAsync(new
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
                OnErrorEvent?.Invoke(this, e.ToString());

				return new();
			}
		}


#region websocket realtime
        public event EventHandler<AuthRsp> OnRspAuthEvent;
        public event EventHandler<SubscribeRsp> OnRspSubscribeEvent;
        public event EventHandler<UnSubscribeRsp> OnRspUnSubscribeEvent;



		// rqdata 实时行情推送
		//https://www.ricequant.com/doc/rqdata/python/generic-api.html#%E5%AE%9E%E6%97%B6%E8%A1%8C%E6%83%85%E6%8E%A8%E9%80%81
		public async Task<bool> StartWebsocket()
		{// 
            var wsUrl = new Url(WebsocketUrl);
            //var wsUrl = WebsocketUrl;
				//.SetQueryParam("token", Token);

			_WebsocketClient = new WebsocketClient(wsUrl.ToUri());
			_WebsocketClient.ReconnectTimeout = TimeSpan.FromSeconds(30);
			_WebsocketClient.ReconnectionHappened.Subscribe(info =>
			System.Diagnostics.Debug.WriteLine($"RqData Reconnection happened, type: {info.Type}")
			);

            _WebsocketClient.MessageReceived.Subscribe(msg =>
                                                       {
                                                           var rtnStr = msg.Text;
                                                           if (rtnStr.Contains("Error"))
                                                           {
                                                               OnErrorEvent?.Invoke(this, rtnStr);
                                                               return;
                                                           }

                                                           var rspBase = rtnStr.ToJsonObj<RqWebsocketRsp>();
                                                           switch (rspBase.Type)
                                                           {
                                                               case ERqWebsocketActionType.auth_reply:
                                                                   {
                                                                       var rsp = rtnStr.ToJsonObj<AuthRsp>();
                                                                       OnRspAuthEvent?.Invoke(this, rsp);
                                                                   }
                                                                   break;
                                                               case ERqWebsocketActionType.subscribe_reply:
                                                                   {
                                                                       var rsp = rtnStr.ToJsonObj<SubscribeRsp>();
                                                                       OnRspSubscribeEvent?.Invoke(this, rsp);
                                                                   }
                                                                   break;
                                                               case ERqWebsocketActionType.unsubscribe_reply:
                                                                   {
                                                                       var rsp = rtnStr.ToJsonObj<UnSubscribeRsp>();
                                                                       OnRspUnSubscribeEvent?.Invoke(this, rsp);
                                                                   }
                                                                   break;
                                                           }

                                                           OnErrorEvent?.Invoke(this, $"Unknown Msg rcv:{rtnStr}");

                                                           System.Diagnostics.Debug.WriteLine($"Unknown Msg rcv:{rtnStr}");
                                                       }

                                                      );

			await _WebsocketClient.Start();

            await Task.Delay(1000);

            AuthReq req = new AuthReq()
            {
				request_id = GetNextReqId().ToString(),
                license =LicenseKey
            };

            string str = req.ToJson();
//            string str = "{\"type\":\"subscribe\",\"symbol\":\"BINANCE:BTCUSDT\"}";
            byte[] bsend = System.Text.Encoding.UTF8.GetBytes(str);
            _WebsocketClient.Send(bsend);

            return true;
        }

        public ReadOnlyObservableCollection<RqDataChannel> Subscribed => new(Subscribed_);
        public event EventHandler<RtTick> OnTickEvent;
        public event EventHandler<RtTick> OnBarEvent;


        public async Task<bool> Subscribe(string order_book_id, ETimeFrame tf)
        {
            var ch = new RqDataChannel(order_book_id, tf);
            var rt = await Subscribe(new[] { ch });
            return rt.Any(p => p == ch);
        }

        public Task<List<RqDataChannel>> Subscribe(IEnumerable<RqDataChannel> channels)
        {
            if (ConnectionState != EConnectionState.Connected)
                return Task.FromResult<List<RqDataChannel>>(new ());

            var already = Subscribed_.Except(channels);  // 已经订阅的
            var ongoing = channels.Except(Subscribed);	// 想要订阅的
            if (!ongoing.Any())
                return Task.FromResult<List<RqDataChannel>>(new ());

            var                        reqId                 = GetNextReqId().ToString();
            var                        taskSource            = new TaskCompletionSource<List<RqDataChannel>>();
            EventHandler<SubscribeRsp> onRspSubscribeHandler = null;
            onRspSubscribeHandler = (sender, e) =>
            {
                if (e.request_id == reqId)
                {
                    OnRspSubscribeEvent -= onRspSubscribeHandler;

					Subscribed_.AddRange(e.subscribed);

                    taskSource.TrySetResult(e.subscribed.Union(already).ToList());
                }
            };


            SubscribeReq req = new SubscribeReq()
            {
				request_id = reqId,
                channels = new List<RqDataChannel>(ongoing)
            };
            string str = req.ToJson();
//            string str = "{\"type\":\"subscribe\",\"symbol\":\"BINANCE:BTCUSDT\"}";
            byte[] bsend = System.Text.Encoding.UTF8.GetBytes(str);
            _WebsocketClient.Send(bsend);

            OnRspSubscribeEvent += onRspSubscribeHandler;

            CancellationTokenSource tokenSource = new CancellationTokenSource(TimeoutMilliseconds);
            tokenSource.Token.Register(() =>
            {
                OnRspSubscribeEvent -= onRspSubscribeHandler;
                taskSource.TrySetCanceled();
            });

            return taskSource.Task;
        }

        public Task UnSubscribe(string order_book_id, ETimeFrame tf) { return UnSubscribe(new[] { new RqDataChannel(order_book_id, tf) }); }
        public Task UnSubscribe(IEnumerable<RqDataChannel> channels)
        {
            if (ConnectionState != EConnectionState.Connected)
                return Task.CompletedTask;

            var                        reqId                 = GetNextReqId().ToString();
            var                        taskSource            = new TaskCompletionSource<List<RqDataChannel>>();
            EventHandler<UnSubscribeRsp> onRspUnSubscribeHandler = null;
            onRspUnSubscribeHandler = (sender, e) =>
            {
                if (e.request_id == reqId)
                {
                    OnRspUnSubscribeEvent -= onRspUnSubscribeHandler;

                    taskSource.TrySetResult(e.unsubscribed);
                }
            };


            UnSubscribeReq req = new UnSubscribeReq()
            {
                request_id = reqId,
                channels   = new List<RqDataChannel>(channels)
            };
            string str = req.ToJson();
//            string str = "{\"type\":\"subscribe\",\"symbol\":\"BINANCE:BTCUSDT\"}";
            byte[] bsend = System.Text.Encoding.UTF8.GetBytes(str);
            _WebsocketClient.Send(bsend);

            OnRspUnSubscribeEvent += onRspUnSubscribeHandler;

            CancellationTokenSource tokenSource = new CancellationTokenSource(TimeoutMilliseconds);
            tokenSource.Token.Register(() =>
            {
                OnRspUnSubscribeEvent -= onRspUnSubscribeHandler;
                taskSource.TrySetCanceled();
            });

            return taskSource.Task;
        }

		public long GetNextReqId()=>Interlocked.Increment(ref ReqId_);

#endregion




		#region 期货

		//获取某期货品种在指定日期下的可交易合约标的列表
		//code: 期货合约品种，如 AG (白银)
		//date: 指定日期
		public async Task<List<string>> get_contracts(string underlying_symbol, DateTime? date = null)
		{
			try
			{
				var ret = await ApiUrl.WithHeader("token", Token).PostJsonAsync(new
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
                OnErrorEvent?.Invoke(this, e.ToString());
				return new();
			}
		}

		public async Task<List<DominantFuture>> get_dominant(string underlying_symbol, DateTime? start_date = null, DateTime? end_date = null, int rule = 0, int rank = 1)
		{
			try
			{
				var ret = await ApiUrl.WithHeader("token", Token).PostJsonAsync(new
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
                OnErrorEvent?.Invoke(this, e.ToString());
				return new();
			}
		}
		#endregion


		public string Token     { get; set; }	// 动态获取
		public string Acccount   { get; set; }
		public string Pwd        { get; set; }
		public string LicenseKey { get; set; }  // 用于实时数据

		protected string _LastErrMsg;

		protected EConnectionState ConnectionState_;

        protected WebsocketClient _WebsocketClient;
        protected long            ReqId_ = 0;

        protected ObservableCollection<RqDataChannel> Subscribed_;
    }
}
