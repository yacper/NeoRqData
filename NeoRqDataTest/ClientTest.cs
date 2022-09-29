using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NeoRqData;
using NUnit.Framework;
using RLib.Base;
using RqDataSharp;


namespace RqDataSharp
{
public class Tests
{
    public IRqDataClient Client_;

    [OneTimeSetUp]
    public async Task Setup()
    {
        Client_ = new RqDataClient("user", "password",
                                   "lisencekey");
        Client_.OnRspAuthEvent += (s, e) => { Debug.WriteLine(e.Dump()); };

        bool ret = await Client_.Connect();
        ret.Should().Be(true);
        Debug.WriteLine(Client_.Dump());
    }


    [Test]
    public async Task info()
    {
        var rtn = await Client_.info();
        Debug.WriteLine(rtn);
    }


    [Test]
    public async Task get_quota()
    {
        var rtn = await Client_.get_quota();
    }


    [Test]
    public async Task all_instruments()
    {
        //var rtn = await Client_.all_instruments(ESymbolType.Future, EMarket.cn);
        var rtn2 = await Client_.all_instruments(ESymbolType.Future, EMarket.cn, DateTime.Now);
        Debug.WriteLine(rtn2.Dump());
    }

    [Test]
    public async Task instruments()
    {
        //var rtn1 = await Client_.instruments("000001.XSHE");      // 指数
        var rtn2 = await Client_.instruments("rb2210"); // 期货
        Debug.WriteLine(rtn2.Dump());
    }

    [Test]
    public async Task get_price()
    {
        //var rtn = await Client_.get_price("AU2210".ToEnumerable(), DateTime.Parse("2022/08/01"), DateTime.Parse("2022/08/20"), ETimeFrame.D1, new []{"open","close"}); // 主力连续合约

        //var rtn3 = await Client_.get_price("au2210", DateTime.Parse("2022/08/01"), DateTime.Parse("2022/08/02"), ETimeFrame.m1); // 主力连续合约
        //Debug.WriteLine(rtn3.Dump());


        //var rtn2 = await Client_.get_price("AU2210", DateTime.Parse("2022/08/01"), DateTime.Parse("2022/08/30 23:40:00"), ETimeFrame.D1); // 主力连续合约

        // 不支持
        var rtn2 = await Client_.get_price("AU2210", DateTime.Parse("2022/08/29 21:00:00"), DateTime.Parse("2022/08/29 22:00:00"), ETimeFrame.tick); // 主力连续合约
        rtn2.ForEach(p=>Debug.WriteLine(p.ToString()));
    }

    [Test]
    public async Task get_ticks()
    {
        var rtn = await Client_.get_ticks("AU2210"); //
        rtn.ForEach(p=>Debug.WriteLine(p.ToString()));
        //Debug.WriteLine(rtn.ToJson());
    }


    [Test]
    public async Task get_live_ticks()
    {
        //var rtn = await Client_.get_live_ticks("AU2210"); //
        //var rtn = await Client_.get_live_ticks("rm2209"); //
        var rtn = await Client_.get_live_ticks("AU2210", new Tuple<DateTime, DateTime>(DateTime.Parse("2022/08/29 21:00:00"), DateTime.Parse("2022/08/29 22:00:00"))); //
        //var rtn3 = await Client_.get_live_ticks("AU2210", new Tuple<DateTime, DateTime>(DateTime.Parse("2022/08/26 21:00:00"), DateTime.Parse("2022/08/26 21:02:00")), new[] { "open", "high" }); //
        rtn.ForEach(p=>Debug.WriteLine(p.ToString()));
    }


    [Test]
    public async Task get_future_contracts()
    {
        var rtn2 = await Client_.get_contracts("AU", DateTime.Parse("2022/8/20"));
        Debug.WriteLine(rtn2.Dump());
    }

    [Test]
    public async Task get_dominant_future()
    {
        // var rtn = await Client_.get_dominant("AU");

        var rtn2 = await Client_.get_dominant("AU", DateTime.Parse("2022/7/20"), DateTime.Parse("2022/8/20"));
        Debug.WriteLine(rtn2.Dump());
    }

    [Test]
    public async Task Websocket_tick()
    {
        var rt = await Client_.Subscribe("AU2210", ETimeFrame.tick);
        Debug.WriteLine(rt.Dump());

        Client_.OnTickEvent += (s, e) => { Debug.WriteLine(e.Dump()); };

        await Task.Delay(1000000);
    }

    [Test]
    public async Task Websocket_bar()
    {
        var rt = await Client_.Subscribe("AU2210", ETimeFrame.m5);
        Debug.WriteLine(rt.Dump());

        Client_.OnBarEvent += (s, e) => { Debug.WriteLine(e.Dump()); };

        await Task.Delay(1000000);
    }
}
}