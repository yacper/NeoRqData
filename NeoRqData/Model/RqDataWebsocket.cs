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
}

public enum EWebsocketRtnType
{
    Error,
    Trade,
    Ping
}

public class RqWebsocketReq
{
    public virtual ERqWebsocketActionType action { get; }
    public string                 request_id { get; set; }
}


public class RqWebsocketRsp
{
    public ERqWebsocketActionType Type       { get; set; }
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

    public List<RqDataChannel> channels { get; set; }
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


public class RtTick     // realtime 下的tick，和历史tick稍有不同
{
    [JsonProperty("s")]
    public string Symbol { get; set; }

    public DateTime Time { get { return TimeValue.FromUnixTimeStamp(); } }

    [JsonProperty("t")]
    public double TimeValue { get; set; }

    [JsonProperty("p")]
    public double Price { get; set; }

    [JsonProperty("v")]
    public double Volume { get; set; }

    public override string ToString() { return $"{Symbol,-10} {Time,-25} {Price,-20}({Volume}) "; }

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

public class RtBar // realtime 下的分钟bar
{


}
}