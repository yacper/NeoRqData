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
    public enum EWebsocketReqType
    {
        subscribe,
    }
    public enum EWebsocketRtnType
    {
        Error,
        Trade,
        Ping
    }

    public class WebsocketReq
    {
        public virtual EWebsocketReqType type { get; set; }
    }

    public class Trade
    {
        [JsonProperty("s")]
        public string Symbol { get; set; }

        public DateTime Time
        {
            get { return TimeValue.FromUnixTimeStamp(); }
        }
        [JsonProperty("t")]
        public double TimeValue { get; set; }
 
        [JsonProperty("p")]
        public double Price { get; set; }

        [JsonProperty("v")]
        public double Volume { get; set; }

        public override string ToString()
        {
            return $"{Symbol, -10} {Time, -25} {Price, -20}({Volume}) ";
        }

        /*{
                {"data":[
                {"c":null,"p":34727.99,"s":"BINANCE:BTCUSDT","t":1624945862327,"v":0.000004},
                {"c":null,"p":34732.02,"s":"BINANCE:BTCUSDT","t":1624945862327,"v":0.002734},
                {"c":null,"p":34732.02,"s":"BINANCE:BTCUSDT","t":1624945862378,"v":0.00113},
                {"c":null,"p":34732.02,"s":"BINANCE:BTCUSDT","t":1624945862408,"v":0.014362},
                {"c":null,"p":34729.12,"s":"BINANCE:BTCUSDT","t":1624945862622,"v":0.001107}
                ],
                "type":"trade"}
        }*/
    }
    public class WebsocketRtn
    {
        public EWebsocketRtnType Type { get; set; }

        public List<Trade> Data { get; set; }
        public string Msg { get; set; }

    }

    public class SubscribeReq: WebsocketReq
    {
        public override EWebsocketReqType type => EWebsocketReqType.subscribe;

        public string       symbol { get; set; } 
    }

   
}
