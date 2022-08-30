/********************************************************************
    created:	2021/6/24 17:41:06
    author:		rush
    email:		yacper@gmail.com	
	
    purpose:
    modifiers:	Rqdata的一个连接
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoRqData
{
	public partial class RqDataClient
	{
		public const string HostUrl      = "https://ricequant.com/";
		public const string AuthUrl      = "https://rqdata.ricequant.com/auth";
		public const string ApiUrl       = "https://rqdata.ricequant.com/api";
		public const string WebsocketUrl = "wss://rqdata.ricequant.com/live_md";
	}

	public enum EConnectionState
	{
		Disconnected = 0,
		Connecting,
		Connected,
		Disconnecting,
	}


	public interface IRqDataClient
	{
		string Token { get; }

		EConnectionState     ConnectionState { get; }
        event EventHandler<string> OnErrorEvent;

		Task<bool> Connect(string user = null, string pwd = null, string licenseKey = null);

        Task DisConnect();

		Task<string>    info();		// 无用
		Task<QuoteInfo> get_quota(); // 获取用户配额信息

		#region Instrumnet
		Task<List<SecurityInfo>> all_instruments(ESymbolType type, EMarket market = EMarket.cn, DateTime? date = null);

		// todo:这个函数目前测试返回有问题
		Task<SecurityInfo> instruments(string order_book_id);       // 
		Task<List<SecurityInfo>> instruments(IEnumerable<string> order_book_ids);
		#endregion

		#region 历史行情

		// 获取合约历史行情数据
		// https://www.ricequant.com/doc/rqdata/python/generic-api.html#get-price-%E8%8E%B7%E5%8F%96%E5%90%88%E7%BA%A6%E5%8E%86%E5%8F%B2%E8%A1%8C%E6%83%85%E6%95%B0%E6%8D%AE
		Task<List<Bar>> get_price(string order_book_id, DateTime start_date, DateTime end_date, ETimeFrame frequency = ETimeFrame.D1,
			IEnumerable<string> fields = null, EAdjustType adjust_type = EAdjustType.pre, bool skip_suspended = false,
			EMarket market = EMarket.cn, bool expect_df = true, string time_slice = null);
		#endregion

		#region tick
		Task<List<Tick>> get_ticks(string order_book_id);
		Task<List<Tick>> get_live_ticks(string order_book_id, Tuple<DateTime, DateTime> range = null, IEnumerable<string> fields = null);
		#endregion

#region websocket realtime
        event EventHandler<AuthRsp> OnRspAuthEvent;

        event EventHandler<SubscribeRsp>                   OnRspSubscribeEvent;
        event EventHandler<UnSubscribeRsp>                 OnRspUnSubscribeEvent;
		public ReadOnlyObservableCollection<RqDataChannel> Subscribed { get; }

        event EventHandler<RtTick> OnTickEvent;
        event EventHandler<RtBar> OnBarEvent;


        Task<bool> Subscribe(string order_book_id, ETimeFrame tf);
        Task<List<RqDataChannel>> Subscribe(IEnumerable<RqDataChannel> channels);
        Task       UnSubscribe(string order_book_id, ETimeFrame tf);
        Task       UnSubscribe(IEnumerable<RqDataChannel> channels);
#endregion

		#region 期货

		//获取某期货品种在指定日期下的可交易合约标的列表
		//code: 期货合约品种，如 AG (白银)
		//date: 指定日期
		Task<List<string>> get_contracts(string underlying_symbol, DateTime? date = null);

		//获取主力合约对应的标的
		//code: 期货合约品种，如 AG (白银)
		// 默认是 rule=0,采用最大昨仓为当日主力合约，每个合约只能做一次主力合约，不会重复出现。针对股指期货，只在当月和次月选择主力合约。当rule =1 时，主力合约的选取只考虑最大昨仓这个条件。
		//默认rank =1。1-主力合约，2-次主力合约，3-次次主力合约。'2','3' 仅对 IC、IH、IF 合约，且 rule =1 时生效
		Task<List<DominantFuture>> get_dominant(string underlying_symbol, DateTime? start_date = null, DateTime? end_date = null, int rule = 0, int rank = 1);

		#endregion
	}
}