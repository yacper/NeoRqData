/********************************************************************
    created:	2021/8/21 17:52:27
    author:		rush
    email:		yacper@gmail.com	
	
    purpose:
    modifiers:	
*********************************************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoRqData
{
public enum ETimeFrame
{
    tick = 1,

    m1   = 2,
    m5   = 8,
    m15  = 16,
    m30  = 32,
    m60  = 64,
    m120 = 128,
    D1   = 1024,
    W1   = 2048,
    M1   = 4096
}

public enum EAdjustType
{
    // 两组前后复权方式仅 volume 字段处理不同，其他字段相同。
    // 其中'pre'、'post'中的 volume 采用拆分因子调整；
    // 'pre_volume'、'post_volume'中的 volume 采用复权因子调整。

    none = 0,   // 不复权，
    pre,        // 前复权
    post,       // 后复权
    pre_volume, // 前复权
    post_volume // 后复权
}

public enum EMarket
{
    cn = 0
}

public enum EExchange
{
    DCE,
    SHFE,
    CZCE,
    CFFEX,
    INE,
    GFEX,   // 广东期货交易所 工业硅

    XSHE, // 深圳
}


public static class TimeFrame
{
    public static string ToResolutionString(this ETimeFrame tf)
    {
        switch (tf)
        {
            case ETimeFrame.tick:
                return "tick";
            case ETimeFrame.m1:
                return "1m";
            case ETimeFrame.m5:
                return "5m";
            case ETimeFrame.m15:
                return "15m";
            case ETimeFrame.m30:
                return "30m";
            case ETimeFrame.m60:
                return "60m";
            case ETimeFrame.m120:
                return "120m";
            case ETimeFrame.D1:
                return "1d";
            case ETimeFrame.W1:
                return "1w";
            case ETimeFrame.M1:
                return "1M";
        }

        throw new NotImplementedException();
    }

    public static ETimeFrame FromString(string str)
    {
        switch (str)
        {
            case "tick":
                return ETimeFrame.tick;
            case "1m":
                return ETimeFrame.m1;
            case "5m":
                return ETimeFrame.m5;
            case "15m":
                return ETimeFrame.m15;
            case "30m":
                return ETimeFrame.m30;
            case "60m":
                return ETimeFrame.m60;
            case "120m":
                return ETimeFrame.m120;
            case "1d":
                return ETimeFrame.D1;
            case "1w":
                return ETimeFrame.W1;
            case "1M":
                return ETimeFrame.M1;
        }

        throw new NotImplementedException();
    }
}

public enum ESymbolType // 证券类型
{
    CS,          //  Common Stock, 即股票
    ETF,         // Exchange Traded Fund, 即交易所交易基金
    LOF,         // Listed Open-Ended Fund，即上市型开放式基金 （以下分级基金已并入）
    INDX,        //    Index, 即指数
    Future,      //  Futures，即期货，包含股指、国债和商品期货
    Spot,        //    Spot，即现货，目前包括上海黄金交易所现货合约
    Option,      //  期权，包括目前国内已上市的全部期权合约
    Convertible, // 沪深两市场内有交易的可转债合约
    Repo         //沪深两市交易所交易的回购合约
}

public enum EFutureProduct // 期货类型
{
    Commodity,
    Index,     // 指数
    Government // 国债
}


// 期货		,type,order_book_id,exchange,symbol,underlying_symbol,underlying_order_book_id,product,maturity_date,de_listed_date,listed_date,round_lot,margin_rate,contract_multiplier,trading_code,market_tplus,trading_hours,industry_name,start_delivery_date,end_delivery_date
//         0,Future,A2307,DCE,黄大豆1号2307,A,,Commodity,2023-07-14,2023-07-14,2022-07-15,1.0,0.08,10.0,a2307,0.0,"21:01-23:00,09:01-10:15,10:31-11:30,13:31-15:00",油脂,2023-07-19,2023-07-19
// 股票/指数 order_book_id,industry_code,market_tplus,symbol,special_type,exchange,status,type,de_listed_date,listed_date,sector_code_name,abbrev_symbol,sector_code,round_lot,trading_hours,board_type,industry_name,issue_price,trading_code 
public class SecurityInfo
{
    public ESymbolType type           { get; set; }
    public string      order_book_id  { get; set; }
    public EExchange   exchange       { get; set; }
    public string      symbol         { get; set; }
    public string      trading_code   { get; set; }
    public double      market_tplus   { get; set; }
    public double      round_lot      { get; set; }
    public string      trading_hours  { get; set; }
    public string      listed_date    { get; set; }
    public string      de_listed_date { get; set; }
    public string      industry_name  { get; set; }

#region 期货

    public string         underlying_symbol        { get; set; } // 期货底层代码 RB
    public string         underlying_order_book_id { get; set; }
    public EFutureProduct product                  { get; set; }
    public string         maturity_date            { get; set; }
    public double         margin_rate              { get; set; }
    public double         contract_multiplier      { get; set; }
    public string       start_delivery_date { get;              set; }
    public string       end_delivery_date { get;              set; }
#endregion

#region 股票/指数
    public string industry_code    { get; set; }
    public string special_type     { get; set; }
    public string status           { get; set; }
    public string sector_code_name { get; set; }
    public string abbrev_symbol    { get; set; }
    public string sector_code      { get; set; }
    public string board_type       { get; set; }
    public double issue_price      { get; set; }
#endregion

    public override string ToString() { return $"{type,-18} {order_book_id,-10} {symbol,14}"; }
}

public enum license_type
{
    TRIAL, // 表示试用账户,
    FULL,  //表示付费用户
    OTHER
}

public class KeyValue
{
    public string name  { get; set; }
    public string value { get; set; }
}

public class QuoteInfo
{
    public double bytes_limit { get; set; } // 每日流量使用上限（单位为字节），如为 0 则表示不受限

    public double       bytes_used     { get; set; } // 当日已用流量（单位为字节）
    public long         remaining_days { get; set; } // 剩余期限（单位为天）
    public license_type license_type   { get; set; }
}

public class DominantFuture
{
    public DateTime date     { get; set; }
    public string   dominant { get; set; }
}

public class Bar
{
    //date,open,close,high,low,volume,money
    //2018-12-04 10:00,11.00,11.03,11.07,10.97,4302800,47472956.00
    //2018-12-04 10:30,11.04,11.04,11.06,10.98,3047800,33599476.00
    public string   order_book_id { get; set; }
    public DateTime datetime      { get; set; }
    public double   open          { get; set; }
    public double   high          { get; set; }
    public double   low           { get; set; }
    public double   close         { get; set; }

    public double volume { get; set; } //            成交量

    public double limit_up            { get; set; } //	涨停价
    public double limit_down          { get; set; } //  跌停价
    public double total_turnover      { get; set; } //  成交额
    public int    num_trades          { get; set; } //  成交笔数 （仅支持股票、ETF、LOF；科创板提供范围为 2021-06-25 至今）
    public double prev_close          { get; set; } //  昨日收盘价 （交易所披露的原始昨收价，复权方法对该字段无效）
    public double settlement          { get; set; } //  结算价 （仅限期货日线数据）
    public double prev_settlement     { get; set; } //  昨日结算价（仅限期货日线数据）
    public double open_interest       { get; set; } //  累计持仓量（期货专用）
    public string trading_date        { get; set; } //  pandasTimeStamp 交易日期（仅限期货分钟线数据），对应期货夜盘的情况
    public string dominant_id         { get; set; } //  实际合约的                            order_book_id，对应期货 888 系主力连续合约的情况
    public double strike_price        { get; set; } //  行权价，仅限           ETF 期权日线数据
    public double contract_multiplier { get; set; } //  合约乘数，仅限 ETF 期权日线数据
    public double iopv                { get; set; } //  场内基金实时估算净值


    //   public double pre_close     { get; set; } // 

    public override string ToString() => $" {datetime,-30} O:{open,-10} H:{high,-10} L:{low,-10} C:{close,-10} V:{volume,-10}";
}

public class Tick
{
    //	time	current	high	low	volume	money	position	a1_p	a1_v	b1_p	b1_v	spread	vol
    //0	2021/10/8 08:59:00.500	22555	22555	22555	280	31577000	83618	22565	20	22555	43	10	
    //1	2021/10/8 09:00:00.500	22555	22575	22555	380	42857625	83616	22560	18	22555	39	5	100
    //2	2021/10/8 09:00:01.000	22585	22585	22555	604	68133775	83559	22585	20	22565	3	20	224

    public DateTime datetime { get; set; }

    public double   open            { get; set; } //             当日开盘价
    public double   high            { get; set; } //                                当日最高价
    public double   low             { get; set; } //                                 当日最低价
    public double   last            { get; set; } //                                最新价
    public double   prev_close      { get; set; } //                          昨日收盘价
    public double   total_turnover  { get; set; } //                      当天累计成交额
    public double   volume          { get; set; } //                             当天累计成交量
    public double   limit_up        { get; set; } //                            涨停价
    public double   limit_down      { get; set; } //                          跌停价
    public double   open_interest   { get; set; } //                      累计持仓量
    public double   a1              { get; set; } //                             卖一至五档报盘价格
    public double   a2              { get; set; } //                               卖一至五档报盘价格
    public double   a3              { get; set; } //                               卖一至五档报盘价格
    public double   a4              { get; set; } //                               卖一至五档报盘价格
    public double   a5              { get; set; } //                               卖一至五档报盘价格
    public double   a1_v            { get; set; } //                        卖一至五档报盘量
    public double   a2_v            { get; set; } //                        卖一至五档报盘量
    public double   a3_v            { get; set; } //                        卖一至五档报盘量
    public double   a4_v            { get; set; } //                        卖一至五档报盘量
    public double   a5_v            { get; set; } //                        卖一至五档报盘量
    public double   b1              { get; set; } //                            买一至五档报盘价
    public double   b2              { get; set; } //                            买一至五档报盘价
    public double   b3              { get; set; } //                            买一至五档报盘价
    public double   b4              { get; set; } //                            买一至五档报盘价
    public double   b5              { get; set; } //                            买一至五档报盘价
    public double   b1_v            { get; set; } //                        买一至五档报盘量
    public double   b2_v            { get; set; } //                        买一至五档报盘量
    public double   b3_v            { get; set; } //                        买一至五档报盘量
    public double   b4_v            { get; set; } //                        买一至五档报盘量
    public double   b5_v            { get; set; } //                        买一至五档报盘量
    public double   change_rate     { get; set; } //                        涨跌幅
    public DateTime trading_date    { get; set; } //              pandasTimeStamp 交易日期，对应期货夜盘的情况
    public double   prev_settlement { get; set; } //                                    昨日结算价（仅期货有效）
    public double   iopv            { get; set; } //      场内基金实时估算净值
    public double   prev_iopv       { get; set; } // 场内基金前估算净值

    //public Bar ToBar()
    //{
    //    return new Bar()
    //    {
    //        datetime = datetime,
    //        open = 


    //    };

    //}

    public override string ToString() { return $" {datetime,-30} C:{last,-10} {b1}({b1_v}) -- {a1}({a1_v}) H:{high,-10} L:{low,-10} V:{volume,-10} "; }
}

public class FutureInfo
{
    public string order_book_id            { get; set; } //   期货代码，期货的独特的标识符（郑商所期货合约数字部分进行了补齐。例如原有代码'ZC609'补齐之后变为'ZC1609'）。主力连续合约 UnderlyingSymbol+88，例如'IF88' ；指数连续合约命名规则为 UnderlyingSymbol+99
    public string symbol                   { get; set; } //   期货的简称，例如'沪深 1005'
    public double margin_rate              { get; set; } //             期货合约的最低保证金率
    public double round_lot                { get; set; } //               期货全部为 1.0
    public string listed_date              { get; set; } // 期货的上市日期。主力连续合约与指数连续合约都为'0000-00-00'
    public string de_listed_date           { get; set; } //期货的退市日期。
    public string industry_name            { get; set; } // 行业分类名称
    public string trading_code             { get; set; } //           交易代码
    public string market_tplus             { get; set; } // 交易制度。'0'表示 T+0，'1'表示 T+1，往后顺推
    public string type                     { get; set; } //  合约类型，'Future'
    public double contract_multiplier      { get; set; } //     合约乘数，例如沪深 300 股指期货的乘数为 300.0
    public string underlying_order_book_id { get; set; } // 合约标的代码，目前除股指期货(IH, IF, IC)之外的期货合约，这一字段全部为'null'
    public string underlying_symbol        { get; set; } // 合约标的名称，例如 IF1005 的合约标的名称为'IF'
    public string maturity_date            { get; set; } //                    期货到期日。主力连续合约与指数连续合约都为'0000-00-00'
    public string exchange                 { get; set; } // 交易所，'DCE' - 大连商品交易所, 'SHFE' - 上海期货交易所，'CFFEX' - 中国金融期货交易所, 'CZCE'- 郑州商品交易所, 'INE' - 上海国际能源交易中心
    public string trading_hours            { get; set; } // 合约最新的交易时间，如需历史数据请使用get_trading_hours
    public string product                  { get; set; } // 合约种类，'Commodity'-商品期货，'Index'-股指期货，'Government'-国债期货
}


public static class RqDataCommon
{
    public static string ToJqDate(this DateTime dt) { return dt.ToString("yyyy-MM-dd"); }
}


public class RqDataChannel
{
    public override string ToString()
    {
        if (Tf == ETimeFrame.tick)
            return $"tick_{order_book_id}";
        else if (Tf == ETimeFrame.m1)
            return $"bar_{order_book_id}";
        else
            return $"bar_{order_book_id}_{Tf.ToResolutionString()}";
    }

    public static implicit operator RqDataChannel(string str)
    {
        var ss = str.Split('_');
        if (ss[0] == "tick")
            return new RqDataChannel() { order_book_id = ss[1].ToUpper(), Tf = ETimeFrame.tick };
        else
        {
            if (ss.Length == 2)
                return new RqDataChannel() { order_book_id = ss[1].ToUpper(), Tf = ETimeFrame.m1 };

            return new RqDataChannel() { order_book_id = ss[1].ToUpper(), Tf = TimeFrame.FromString(ss[2]) };
        }
    }


    public static bool operator ==(RqDataChannel l, RqDataChannel r)
    {
        if (l.order_book_id == r.order_book_id)
            return l.Tf == r.Tf;
        return false;
    }

    public static bool operator !=(RqDataChannel vector1, RqDataChannel vector2) { return !(vector1 == vector2); }

    public static bool Equals(RqDataChannel l, RqDataChannel r)
    {
        if (l.order_book_id.Equals(r.order_book_id))
            return l.Tf.Equals(r.Tf);
        return false;
    }

    public override bool Equals(object o)
    {
        if (o == null || !(o is RqDataChannel))
            return false;
        return RqDataChannel.Equals(this, (RqDataChannel)o);
    }

    public bool Equals(RqDataChannel value) { return RqDataChannel.Equals(this, value); }

    public override int GetHashCode() { return order_book_id == null ? Tf.GetHashCode() : order_book_id.GetHashCode() ^ Tf.GetHashCode(); }


    public RqDataChannel() { }

    public RqDataChannel(string order_book_id, ETimeFrame tf)
    {
        this.order_book_id = order_book_id.ToUpper();
        Tf                 = tf;
    }

    public string     order_book_id { get; set; }
    public ETimeFrame Tf            { get; set; }
}
}