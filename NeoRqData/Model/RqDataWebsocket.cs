/********************************************************************
    created:	2021/6/25 10:27:58
    author:		rush
    email:		yacper@gmail.com	
	
    purpose:
    modifiers:	
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RLib.Base;

namespace NeoRqData
{
public enum ERqWebsocketActionType
{
    auth,       // 
    auth_reply, // 
    subscribe,
    subscribe_reply,
    unsubscribe,
    unsubscribe_reply,

    feed, // tick/bar 事件
}

public enum EWebsocketRtnType
{
    Error,
    Trade,
    Ping
}

public class RqWebsocketReq
{
    public virtual ERqWebsocketActionType action     { get; }
    public         string                 request_id { get; set; }
}


public class RqWebsocketRsp
{
    public ERqWebsocketActionType action     { get; set; }
    public string                 request_id { get; set; }
}


public class AuthReq : RqWebsocketReq
{
    public override ERqWebsocketActionType action => ERqWebsocketActionType.auth;

    public string license { get; set; }
}

public class AuthRsp : RqWebsocketRsp
{
    /*
     {
        "action":"auth_reply",
        "permissions":["tick_futures","bar_equity","bar_futures","tick_equity"],
        "quota":1073741824.0,
        "request_id":null
     }
     */
    public List<string> permissions { get; set; } //	
    public double       quota       { get; set; } //	流量限制
}


public class SubscribeReq : RqWebsocketReq
{
    public override ERqWebsocketActionType action => ERqWebsocketActionType.subscribe;

    public List<string> channels { get; set; }
}


public class SubscribeRsp : RqWebsocketRsp
{
    public List<RqDataChannel> subscribed { get; set; }
}

public class UnSubscribeReq : RqWebsocketReq
{
    public override ERqWebsocketActionType action => ERqWebsocketActionType.subscribe;

    public List<RqDataChannel> channels { get; set; }
}


public class UnSubscribeRsp : RqWebsocketRsp
{
    public List<RqDataChannel> unsubscribed { get; set; }
}

public class FeedRtn : RqWebsocketRsp
{
    public RqDataChannel channel { get; set; }
}


/*
{"order_book_id":"AU2210","trading_date":20220830,"datetime":20220829214932500,"update_time":214932500,
"open":388.98,"high":389.78,"low":388.72,"last":389.4,"prev_close":387.36,"volume":1851,"total_turnover":720183460.0,
"open_interest":32850.0,"prev_settlement":388.44,"limit_up":419.5,"limit_down":357.36,"ask":[389.46,389.5,389.52,389.58,389.6],
"ask_vol":[1,13,23,2,3],"bid":[389.42,389.4,389.38,389.36,389.28],"bid_vol":[2,44,12,1,1],"channel":"tick_AU2210","action":"feed"}
*/
public class RtTick : FeedRtn // realtime 下的tick，和历史tick稍有不同
{
    public string order_book_id { get; set; }

    [JsonConverter(typeof(DateTimeFormatConverter), "yyyyMMddHHmmssfff")] //20220829222929000 
    public DateTime datetime { get; set; }

    [JsonConverter(typeof(DateTimeFormatConverter), "HHmmssfff")]
    public DateTime update_time { get; set; }

    [JsonConverter(typeof(DateTimeFormatConverter), "yyyyMMdd")]
    public DateTime trading_date { get; set; } //              pandasTimeStamp 交易日期，对应期货夜盘的情况

    public double       open            { get; set; } //             当日开盘价
    public double       high            { get; set; } //                                当日最高价
    public double       low             { get; set; } //                                 当日最低价
    public double       last            { get; set; } //                                最新价

    public double       prev_close      { get; set; } //                          昨日收盘价
    public double       prev_settlement { get; set; } //                                    昨日结算价（仅期货有效）

    public double       total_turnover  { get; set; } //                      当天累计成交额
    public double       volume          { get; set; } //                             当天累计成交量

    public double       limit_up        { get; set; } //                            涨停价
    public double       limit_down      { get; set; } //                          跌停价
    public double       open_interest   { get; set; } //                      累计持仓量

    public List<double> ask             { get; set; } //                             卖一至五档报盘价格
    public List<int>    ask_vol         { get; set; } //                             卖一至五档报盘价格
    public List<double> bid             { get; set; } //                             卖一至五档报盘价格
    public List<int>    bid_vol         { get; set; } //                             卖一至五档报盘价格

    public double change_rate { get; set; } //                        涨跌幅
    public double iopv        { get; set; } //      场内基金实时估算净值
    public double prev_iopv   { get; set; } // 场内基金前估算净值


    public override string ToString() { return $" {datetime,-30} C:{last,-10} {bid[0]}({bid_vol[0]}) -- {ask[0]}({ask_vol[0]}) H:{high,-10} L:{low,-10} V:{volume,-10} "; }
}

//{"order_book_id":"AU2210","trading_date":20220830,"open":389.62,"high":389.66,"low":389.58,"close":389.64,"volume":16,"total_turnover":6234000.0,
//"limit_up":419.5,"limit_down":357.36,"open_interest":32560.0,"datetime":20220830104000,"channel":"bar_AU2210","action":"feed"}
public class RtBar : FeedRtn // realtime 下的分钟bar
{
    //date,open,close,high,low,volume,money
    //2018-12-04 10:00,11.00,11.03,11.07,10.97,4302800,47472956.00
    //2018-12-04 10:30,11.04,11.04,11.06,10.98,3047800,33599476.00
    public string order_book_id { get; set; }

    [JsonConverter(typeof(DateTimeFormatConverter), "yyyyMMddHHmmss")] //20220830104000 
    public DateTime datetime { get; set; }

    [JsonConverter(typeof(DateTimeFormatConverter), "yyyyMMdd")]
    public DateTime trading_date { get; set; } //              pandasTimeStamp 交易日期，对应期货夜盘的情况

    public double open  { get; set; }
    public double high  { get; set; }
    public double low   { get; set; }
    public double close { get; set; }
    public double volume { get; set; } //            成交量
    public double total_turnover      { get; set; } //  成交额

    public double limit_up            { get; set; } //	涨停价
    public double limit_down          { get; set; } //  跌停价
    public double open_interest       { get; set; } //  累计持仓量（期货专用）


    public int    num_trades          { get; set; } //  成交笔数 （仅支持股票、ETF、LOF；科创板提供范围为 2021-06-25 至今）
    public double prev_close          { get; set; } //  昨日收盘价 （交易所披露的原始昨收价，复权方法对该字段无效）
    public double settlement          { get; set; } //  结算价 （仅限期货日线数据）
    public double prev_settlement     { get; set; } //  昨日结算价（仅限期货日线数据）
    public string dominant_id         { get; set; } //  实际合约的                            order_book_id，对应期货 888 系主力连续合约的情况
    public double strike_price        { get; set; } //  行权价，仅限           ETF 期权日线数据
    public double contract_multiplier { get; set; } //  合约乘数，仅限 ETF 期权日线数据
    public double iopv                { get; set; } //  场内基金实时估算净值

    //   public double pre_close     { get; set; } // 

    public override string ToString() => $" {datetime,-30} O:{open,-10} H:{high,-10} L:{low,-10} C:{close,-10} V:{volume,-10} T:{total_turnover,-10}";
}
}