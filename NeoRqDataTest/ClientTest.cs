/*
License Key: eJzdV0l7o0YQ/UfzsZjMcIxsg0QsPMJSA33JxyKGpcEkGEnw6/O6JdkssnNODj646NpfvSrtO0vae79nz5n1B5E32dO9lYZmxP9f7fqVbGeW/m3fWbXvbbLn/FGx8bfeRt26L5pVSe6i+9Vv660Pmd/ZD5v++eWYhSYpY8jdzipjVyueypjFhi6HJZMiWc8D02ipsuPOpNA1mvBe62MTcnXNZRX1nISWRg5Z56kWi5ekC7MV/FHY2ECXSJ6SHkLTqcMy7j3Vl1bZMaMlaYOyuHzndrmO8F36ri2FqpVQLz2Gqi1HpfAPfYftDT0PFfkYKhoLK4fb64Q9Ycc+hJUNOXRk/RiVLKcv01gsFpb2K3Lpgk/fyGlYGsjNSmKF1FRJIWMtLX/wOIZ1yqOSFAFqEHkEzWDSnttiehK4mgT91ndPidA1yZ2nxixw49d4uZ7agQ1a401L76exEIkyPfc9+1Xk5y0OIWp3w4YUlUYbKUwKXL39JN4339VS2O0G3xH3KfEVHbqkjVTSi29L0sdLq6bezA9icRhV9G5PdGU96DVybbhupDpd7MqiP6I3Fc9LE1jxS72lhs5ty++9G3535fyTfvL+y9w2/HaIAf9H7xiM1AXzFVYGrp2gp33QIa7KeeMxPZVOGpuPH3g1Se+rVh0tnUPEdLZfLg7Iu5vhQLWgh1qodhIjqg9fjoa5+cC9gtkykZNnsY/Yaeqrwn4HHWBYG9mY1dtkvK6HaDY7o3pCz5CoGx+iqpj2ZVgf3tMDfyuwcKPOkevUAdF7xHGc41/MK0Psok/I4zqDQ3+t7/lTWRqK+hutqP+oljbeL/rA1EVdw7IZ1hOxvgEbRPJvx3Lljn7KHdO8EEsdL23UKYWPmI34QbWP1LVripn/lKsGcc5sLxF3ZfO+1uAfHncKGx8cNOa0xK/YlQcY3iZRRZpzrc9zBBs9dcGnnp0IPjrPO/iVvfEZm/VMXTSBt5nWvKPuCTglBZ8Xas76lMEHeFxOo3PNp5ipOU7At+faupi57OwvBHaox+CXHCPzpH1g+8JnHxj/fIYUo4+uHG8K7hC1u8zCbU5XeG9iRt274RuVuqwKlgPOH9QG/WroeFZv2p7q4a+8wauMz+fTeB8JXfSzwF563wtR99XeIl/MvPDJ8VGAs1LOI9F0ZgbY/4pHZ7XwLOxiwZs1sKRgP2m3aga8zjAGHHUh0aVQ1jkGkcNlp5UUe7Our3OOHsEmq4G9Yo7TyzxfZnD6HVjHrog5v8zxN8zRs/vzTjg1ocJrxA7z24LLSQV9bdifYV1vcHIeqhTzwBCfPYuP1+VWvd7jvsHRl32pjXF+98mO5nsDuXB+NQ0p/nrHog+LI/Kp/h2779gf2uIzcMHm7FsdVVYiMMnvBtwg8XK2V7BTNV6nGn2YcQQwjh5ZCr3u5BnvX3foLMfzHfl+Y05n5XRAXMAAudYKWKEHT3HSvWulUWbFeNsErlzz+xV3LzAS13g3lLWh4jBP0eTwBbcv7l0+g3jTxq6EG9nW+V2LnpRiJyinOnSZtMpf5XW+6Z4ffh3X93dHvMmBO24P+9T+e5XX31fMxozjzmZNhptKWzGpWRX2Ft/CFXq8fxHxGeTRn7w5/XQycW9jlxSZh5hedtqjez+W7UpyRC1HMqcikrgxxj5+4o4RWJvEgtuJsqjCfJWnkU7C736XtTEwBs7heW0jvu/Gdh92U//EMSdv1ltpmh97doxfYxnD/jN/TN5Rme9f7IuRj626EDfLNBbwtwKc4ZvVjHQ82MJ9t8cMPVdNtlMXh3hSy428nvgmu2kPto/6NDdrWzj+RPZCMWsTmRl75/to7EM/XvhqHAvwF5ekwzf89hrqyByL2OVG4/H7wsU84CYM/g/9qbBP8TuI3zXgiTfwiRzz3P/j/UoE9sCNLuTZSnp+KCQ7j7Jk801aP7RH1QiblfLXXZJvHkLY+tPvtOLhx/cl1YPiNZOTvedXu38AKuJ6sQ==
账号：15755006301
密码：Welcome123
 */

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
        //Client_ = new RqDataClient("15755006301", "Welcome123");

        Client_ = new RqDataClient("18621301957", "hello@123",
                                   "LM5xlOCD4vY06OAMcR3yFlbD95uV8nkBmdu7RC4yLhmF--FjE_P3w0v_hPKch1LFaDMM_ZdIBv4zP10XX-oaOh43Xm2c5vMdR4-4w6zi7IplJ_jTssuCJO1z_5iZcZWyuqrVZswe3qzOcOlEDVv3K4Z35zcaaUz3_QviC9T8b6Y=gkxCS1k7PDTWIOGNtA6UPrMaYILpjczvxT3USeAhON9QNjU2zyAk1gMaGjgaws-UUFcQIKFQEBaboQnCQwHpJpRPVy5oD2EVPo2cImoZ4Pnbm9wkgVNDGYR0wbkaHbsf0x4AAxTluyrC33yKW_tRk565CrurR83URsHKH1NpAnA=");
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