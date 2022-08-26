/*
License Key: QLORsFzE-N5KL9P9OxNWAYZxIlNcNh4Bw6XZRETVLkVxZbWkpKERuSkoJhGGdLELBUm4BwXLBZrHJcYUflbTqC0H53MVndKvjBUIv5mVj44VkU9dHTpgrIfX74xB8eKpmqbiobnKsUIRN51T_WmUWvlfJJmsl9RQ3j150GboApI=MjDdTjuNAz3JIgozBi7BQpf8NJ1Nb4n19or0COWAxj3BW-gsaAtbRI93OdBWpgnA44vitTKC-lUXzyPmOYh6TNOAujiJHWQcmVSKbD_wUwwatfrxEQ4onZtzhnqzB8W51N2ns3Q2jv8LzSGWugf-PlWJYa8LJzN_Y50ybssJ-SM=
账号：15755006301
密码：Welcome123
 */

using System;
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
        Client_ = new RqDataClient("15755006301", "Welcome123");

        bool ret = await Client_.Connect();
        ret.Should().Be(true);
    }

    [Test]
    public async Task all_instruments()
    {
        //var rtn = await Client_.all_instruments(ESymbolType.Future, EMarket.cn);
        var rtn2 = await Client_.all_instruments(ESymbolType.Future, EMarket.cn, DateTime.Now);
    }

    [Test]
    public async Task instruments()
    {
        //var rtn = await Client_.all_instruments(ESymbolType.Future, EMarket.cn);
        var rtn2 = await Client_.instruments("au2210");
    }


    [Test]
    public async Task get_query_count()
    {
        var rtn = await Client_.get_query_count();
    }


    [Test]
    public async Task get_price()
    {
        //var rtn = await Client_.get_price("AU2210".ToEnumerable(), DateTime.Parse("2022/08/01"), DateTime.Parse("2022/08/20"), ETimeFrame.D1, new []{"open","close"}); // 主力连续合约

        var rtn2 = await Client_.get_price("AU2210", DateTime.Parse("2022/08/01"), DateTime.Parse("2022/08/20"), ETimeFrame.D1, new []{"close"}); // 主力连续合约

    }

  [Test]
    public async Task get_ticks()
    {
        var rtn = await Client_.get_ticks("AU2210"); //
    }


  [Test]
    public async Task get_live_ticks()
    {
        //var rtn = await Client_.get_live_ticks("AU2210"); //
        //var rtn = await Client_.get_live_ticks("rm2209"); //
        //var rtn2 = await Client_.get_live_ticks("AU2210", new Tuple<DateTime, DateTime>(DateTime.Parse("2022/08/26 21:00:00"), DateTime.Parse("2022/08/26 22:00:00"))); //
        var rtn3 = await Client_.get_live_ticks("AU2210", new Tuple<DateTime, DateTime>(DateTime.Parse("2022/08/26 21:00:00"), DateTime.Parse("2022/08/26 21:02:00")), new []{"open", "high"}); //
    }



    [Test]
    public async Task get_future_contracts()
    {
        var rtn2 = await Client_.get_contracts("AU", DateTime.Parse("2022/8/20"));
    }

    [Test]
    public async Task get_dominant_future()
    {
       // var rtn = await Client_.get_dominant("AU");

        var rtn2 = await Client_.get_dominant("AU", DateTime.Parse("2022/8/20"), DateTime.Parse("2022/8/20"));
    }
}
}