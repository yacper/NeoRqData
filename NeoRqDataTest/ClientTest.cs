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
    public async Task GetCandels()
    {
        var rtn = await Client_.get_security_info("000001.XSHE");
    }

    [Test]
    public async Task get_query_count()
    {
        var rtn = await Client_.get_query_count();
    }

    [Test]
    public async Task get_all_securities()
    {
        var rtn = await Client_.get_all_securities(ECodeType.futures);
        rtn.ToExcel("D:/futures.xls");
    }

    [Test]
    public async Task get_security_info()
    {
        var rtn = await Client_.get_security_info("000001.XSHE");
    }

    [Test]
    public async Task get_current_price()
    {
        var ret = await Client_.get_current_price(new[] { "AU9999.XSGE", "AG9999.XSGE", "000001.XSHE", "RB2201.XSGE" });
    }

    [Test]
    public async Task get_price()
    {
        //var rtn5 = await Client_.get_price("AU2210".ToEnumerable(), DateTime.Parse("2022/08/01"), DateTime.Parse("2022/08/20"), ETimeFrame.D1, new []{"datetime", "open"}); // 主力连续合约
        var rtn5 = await Client_.get_price("AU2210".ToEnumerable(), DateTime.Parse("2022/08/01"), DateTime.Parse("2022/08/20"), ETimeFrame.D1, new []{"open","close"}); // 主力连续合约

        //var rtn3 = await Client_.get_price("AU2112.XSGE", ETimeFrame.D1);

        //var rtn2 = await Client_.get_price("000001.XSHE", ETimeFrame.D1);
        //var rtn  = await Client_.get_price("000001.XSHE", ETimeFrame.D1);
    }

    [Test]
    public async Task get_price_period()
    {
        var rtn = await Client_.get_price_period("RB9999.XSGE", ETimeFrame.m5, ("2019/1/1 9:00:00").ToDateTime(),
            ("2019/3/31 23:00:00").ToDateTime()); // 主力连续合约
        //var rtn = await  Client_.get_price_period("RB9999.XSGE", ETimeFrame.m5, DateTime.Parse("2020/7/1 9:00:00"), DateTime.Parse("2020/7/2 23:00:00"));   // 主力连续合约

        var rtn5 = await Client_.get_price_period("AU9999.XSGE", ETimeFrame.D1, DateTime.Parse("2020/8/31 17:43:51"),
            DateTime.Now); // 主力连续合约

        //var rtn3 = await  Client_.get_price_period("AU2112.XSGE", ETimeFrame.D1);

        //var rtn2 = await  Client_.get_price("000001.XSHE", ETimeFrame.D1);
        //var rtn = await  Client_.get_price("000001.XSHE", ETimeFrame.D1);
    }

    [Test]
    public async Task get_ticks()
    {
        var rtn = await Client_.get_ticks("ZN2112.XSGE", ("2021/10/7").ToDateTime(),
            ("2021/10/9").ToDateTime()); // 主力连续合约
    }

    [Test]
    public async Task get_future_contracts()
    {
        var rtn2 = await Client_.get_future_contracts("AU", DateTime.Parse("2021/8/20"));
    }

    [Test]
    public async Task get_dominant_future()
    {
        var rtn2 = await Client_.get_dominant_future("AU");
    }
}
}