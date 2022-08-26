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
    m1,
    m5,
    m15,
    m30,
    m60,
    m120,
    D1,
    W1,
    M1
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
	INE 
}


public static class TimeFrame
{
    public static string ToResolutionString(this ETimeFrame tf)
    {
        switch (tf)
        {
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

        return "1d";
    }
}

	public enum ESymbolType // 证券类型
	{
		CS,//  Common Stock, 即股票
		ETF,// Exchange Traded Fund, 即交易所交易基金
		LOF, // Listed Open-Ended Fund，即上市型开放式基金 （以下分级基金已并入）
		INDX, //    Index, 即指数
		Future, //  Futures，即期货，包含股指、国债和商品期货
		Spot, //    Spot，即现货，目前包括上海黄金交易所现货合约
		Option, //  期权，包括目前国内已上市的全部期权合约
		Convertible, // 沪深两市场内有交易的可转债合约
		Repo    //沪深两市交易所交易的回购合约
	}

public enum EProduct
	{
Commodity,
Index,		// 指数
Government			// 国债
	}


	//A0303,A,0.0,豆一0303,0.05,2003-03-14,Future,a0303,DCE,Commodity,10.0,1.0,"21:01-23:00,09:01-10:15,10:31-11:30,13:31-15:00",2002-03-15,油脂,2003-03-14,null
	public class SecurityInfo
	{
		public string order_book_id { get; set; }
		public string underlying_symbol { get; set; }
		public double market_tplus { get; set; }
		public string symbol { get; set; }
		public double margin_rate { get; set; }
		public string maturity_date { get; set; }
		public ESymbolType type { get; set; }
		public string trading_code { get; set; }
		public EExchange exchange { get; set; }
		public EProduct product { get; set; }
		public double contract_multiplier { get; set; }
		public double round_lot { get; set; }
		public string trading_hours { get; set; }
		public string listed_date { get; set; }
		public string industry_name { get; set; }
		public string de_listed_date { get; set; }
		public string underlying_order_book_id { get; set; }

		public override string ToString() { return $"{type,-18} {order_book_id,-10} {symbol,14}"; }
	}

public class DominantFuture
{
	public     DateTime date     { get; set; }
		public string   dominant { get; set; }

}

public class Bar
{
    //date,open,close,high,low,volume,money
    //2018-12-04 10:00,11.00,11.03,11.07,10.97,4302800,47472956.00
    //2018-12-04 10:30,11.04,11.04,11.06,10.98,3047800,33599476.00
    public     string   order_book_id { get; set; }
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

		public double open { get; set; } //             当日开盘价
		public double high { get; set; } //                                当日最高价
		public double low { get; set; } //                                 当日最低价
		public double last { get; set; } //                                最新价
		public double prev_close { get; set; } //                          昨日收盘价
		public double total_turnover { get; set; } //                      当天累计成交额
		public double volume { get; set; } //                             当天累计成交量
		public double limit_up { get; set; } //                            涨停价
		public double limit_down { get; set; } //                          跌停价
		public double open_interest { get; set; } //                      累计持仓量
		public double a1 { get; set; } //                             卖一至五档报盘价格
		public double a2 { get; set; } //                               卖一至五档报盘价格
		public double a3 { get; set; } //                               卖一至五档报盘价格
		public double a4 { get; set; } //                               卖一至五档报盘价格
		public double a5 { get; set; } //                               卖一至五档报盘价格
		public double a1_v { get; set; } //                        卖一至五档报盘量
		public double a2_v { get; set; } //                        卖一至五档报盘量
		public double a3_v { get; set; } //                        卖一至五档报盘量
		public double a4_v { get; set; } //                        卖一至五档报盘量
		public double a5_v { get; set; } //                        卖一至五档报盘量
		public double b1 { get; set; } //                            买一至五档报盘价
		public double b2 { get; set; } //                            买一至五档报盘价
		public double b3 { get; set; } //                            买一至五档报盘价
		public double b4 { get; set; } //                            买一至五档报盘价
		public double b5 { get; set; } //                            买一至五档报盘价
		public double b1_v { get; set; } //                        买一至五档报盘量
		public double b2_v { get; set; } //                        买一至五档报盘量
		public double b3_v { get; set; } //                        买一至五档报盘量
		public double b4_v { get; set; } //                        买一至五档报盘量
		public double b5_v { get; set; } //                        买一至五档报盘量
		public double change_rate { get; set; } //                        涨跌幅
		public DateTime trading_date { get; set; } //              pandasTimeStamp 交易日期，对应期货夜盘的情况
		public double prev_settlement { get; set; } //                                    昨日结算价（仅期货有效）
		public double iopv { get; set; } //      场内基金实时估算净值
		public double prev_iopv { get; set; }// 场内基金前估算净值


		public override string ToString() { return $" {datetime,-30} C:{last,-10} {b1}({b1_v}) -- {a1}({a1_v}) H:{high,-10} L:{low,-10} V:{volume,-10} "; }
	}

	public class CurrentPrice
{
    //code,current
    //000001.XSHE,13.35
    //600600.XSHG,42.4

    public string code    { get; set; }
    public double current { get; set; }
}

	public class FutureInfo
	{
		public string order_book_id { get; set; } //   期货代码，期货的独特的标识符（郑商所期货合约数字部分进行了补齐。例如原有代码'ZC609'补齐之后变为'ZC1609'）。主力连续合约 UnderlyingSymbol+88，例如'IF88' ；指数连续合约命名规则为 UnderlyingSymbol+99
		public string symbol { get; set; } //   期货的简称，例如'沪深 1005'
		public double margin_rate { get; set; } //             期货合约的最低保证金率
		public double round_lot { get; set; } //               期货全部为 1.0
		public string listed_date { get; set; } // 期货的上市日期。主力连续合约与指数连续合约都为'0000-00-00'
		public string de_listed_date { get; set; } //期货的退市日期。
		public string industry_name { get; set; } // 行业分类名称
		public string trading_code { get; set; } //           交易代码
		public string market_tplus { get; set; } // 交易制度。'0'表示 T+0，'1'表示 T+1，往后顺推
		public string type { get; set; } //  合约类型，'Future'
		public double contract_multiplier { get; set; } //     合约乘数，例如沪深 300 股指期货的乘数为 300.0
		public string underlying_order_book_id { get; set; } // 合约标的代码，目前除股指期货(IH, IF, IC)之外的期货合约，这一字段全部为'null'
		public string underlying_symbol { get; set; } // 合约标的名称，例如 IF1005 的合约标的名称为'IF'
		public string maturity_date { get; set; } //                    期货到期日。主力连续合约与指数连续合约都为'0000-00-00'
		public string exchange { get; set; } // 交易所，'DCE' - 大连商品交易所, 'SHFE' - 上海期货交易所，'CFFEX' - 中国金融期货交易所, 'CZCE'- 郑州商品交易所, 'INE' - 上海国际能源交易中心
		public string trading_hours { get; set; } // 合约最新的交易时间，如需历史数据请使用get_trading_hours
		public string product { get; set; }// 合约种类，'Commodity'-商品期货，'Index'-股指期货，'Government'-国债期货
	}




	public static class RqDataCommon
{
    public static string ToJqDate(this DateTime dt) { return dt.ToString("yyyy-MM-dd"); }
}
}