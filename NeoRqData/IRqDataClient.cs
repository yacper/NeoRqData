/********************************************************************
    created:	2021/6/24 17:41:06
    author:		rush
    email:		yacper@gmail.com	
	
    purpose:
    modifiers:	Rqdata的一个连接
*********************************************************************/

using System;
using System.Collections.Generic;
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
    public const string WebsocketUrl = "wss://ws.finnhub.io";
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
    string ApiKey      { get; }
    int    CallsPerMin { get; } // api calss per min

    int CallsInScope { get; }

    string LastErrMsg { get; }

    EConnectionState ConnectionState { get; }

    Task<bool> Connect(string user = null, string pwd = null);

    Task<int> get_query_count();

    Task<List<Security>> get_all_securities(ECodeType type, DateTime? date = null);
    Task<SecurityInfo>   get_security_info(string     code);

    // 获取合约历史行情数据
    // https://www.ricequant.com/doc/rqdata/python/generic-api.html#get-price-%E8%8E%B7%E5%8F%96%E5%90%88%E7%BA%A6%E5%8E%86%E5%8F%B2%E8%A1%8C%E6%83%85%E6%95%B0%E6%8D%AE
    Task<List<Bar>> get_price(IEnumerable<string> order_book_ids,               DateTime start_date,  DateTime end_date, ETimeFrame frequency = ETimeFrame.D1, IEnumerable<string> fields = null,
        EAdjustType                               adjust_type = EAdjustType.pre, bool      skip_suspended = false, EMarket market = EMarket.cn, bool expect_df = true, string time_slice = null);

    //指定开始时间date和结束时间end_date时间段，获取行情数据
    //code: 证券代码
    //unit: bar的时间单位, 支持如下周期：1m, 5m, 15m, 30m, 60m, 120m, 1d, 1w, 1M。其中m表示分钟，d表示天，w表示周，M表示月
    //date : 开始时间，不能为空，格式2018-07-03或2018-07-03 10:40:00，如果是2018-07-03则默认为2018-07-03 00:00:00
    //end_date：结束时间，不能为空，格式2018-07-03或2018-07-03 10:40:00，如果是2018-07-03则默认为2018-07-03 23:59:00
    //fq_ref_date：复权基准日期，该参数为空时返回不复权数据
    //注：当unit是1w或1M时，第一条数据是开始时间date所在的周或月的行情。当unit为分钟时，第一条数据是开始时间date所在的一个unit切片的行情。 最大获取1000个交易日数据
    Task<List<Bar>> get_price_period(string code, ETimeFrame timeframe, DateTime date, DateTime? endDate = null, DateTime? fq_ref_date = null);

    Task<List<CurrentPrice>> get_current_price(IEnumerable<string> codes);

    // 需要付费
    // startdate的参数名不对，无法正确指定，目前函数有问题
    Task<List<Tick>> get_ticks(string code, DateTime? startDate = null, DateTime? endDate = null, int count = 5000, string fields = "None", bool skip = true, bool df = false);


#region 期货

    //获取某期货品种在指定日期下的可交易合约标的列表
    //code: 期货合约品种，如 AG (白银)
    //date: 指定日期
    Task<List<string>> get_future_contracts(string code, DateTime? date = null);


    //获取主力合约对应的标的
    //code: 期货合约品种，如 AG (白银)
    //date: 指定日期参数，获取历史上该日期的主力期货合约
    Task<string> get_dominant_future(string code);

#endregion
}
}