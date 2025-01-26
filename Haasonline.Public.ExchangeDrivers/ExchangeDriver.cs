using System;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json.Linq;

public class ArbitrageBot
{
    private static string binanceAPI = "https://api.binance.com/api/v3/ticker/price?symbol=";
    private static string gateioAPI = "https://api.gateio.ws/api2/1/tickers/";

    // 跨所套利
    public static void CrossExchangeArbitrage()
    {
        List<string> pairs = new List<string> { "BTCUSDT", "ETHUSDT", "XRPUSDT", "SOLUSDT" };
        Dictionary<string, decimal> priceDifferences = new Dictionary<string, decimal>();

        foreach (var pair in pairs)
        {
            decimal priceBinance = GetPriceFromExchange(binanceAPI, pair);
            decimal priceGateio = GetPriceFromExchange(gateioAPI, pair);

            decimal priceDifference = (priceGateio - priceBinance) / priceBinance * 100; // 計算百分比差異

            priceDifferences[pair] = priceDifference;
        }

        // 根據價格差異選擇價差最大的幣對
        var bestPair = GetBestArbitragePair(priceDifferences);
        Console.WriteLine($"Best Arbitrage Pair: {bestPair.Key} with price difference of {bestPair.Value}%");

        if (bestPair.Value > 0.5m) // 設定套利門檻，例如 0.5%
        {
            ExecuteCrossExchangeArbitrage(bestPair.Key);
        }
    }

    // 資金費率套利
    public static void FundingRateArbitrage()
    {
        decimal fundingRateA = GetFundingRate("binance", "BTC/USDT");
        decimal fundingRateB = GetFundingRate("gateio", "BTC/USDT");

        Console.WriteLine($"Funding Rate on Binance: {fundingRateA}%");
        Console.WriteLine($"Funding Rate on Gate.io: {fundingRateB}%");

        if (fundingRateA > fundingRateB)
        {
            Console.WriteLine("Arbitrage Opportunity: Borrow on Gate.io, Lend on Binance.");
            // 在 Gate.io 借入，並在 Binance 放貸
        }
    }

    // 三角套利
    public static void TriangularArbitrage()
    {
        decimal priceBTC_USDT = GetPriceFromExchange(binanceAPI, "BTCUSDT");
        decimal priceETH_USDT = GetPriceFromExchange(binanceAPI, "ETHUSDT");
        decimal priceETH_BTC = GetPriceFromExchange(binanceAPI, "ETHBTC");

        decimal triangularProfit = (priceBTC_USDT / priceETH_BTC) * priceETH_USDT;

        Console.WriteLine($"Triangular Arbitrage Profit: {triangularProfit}%");

        if (triangularProfit > 0.5m) // 設定套利門檻，例如 0.5%
        {
            ExecuteTriangularArbitrage();
        }
    }

    // 期現套利
    public static void FuturesSpotArbitrage()
    {
        decimal spotPrice = GetPriceFromExchange(binanceAPI, "BTCUSDT");
        decimal futuresPrice = GetPriceFromExchange(binanceAPI, "BTCUSDT");

        if (futuresPrice > spotPrice)
        {
            Console.WriteLine($"Futures Price is higher than Spot Price. Arbitrage Opportunity Detected!");
            // 在現貨市場買入 BTC，並在期貨市場賣出
        }
    }

    // 自動選擇最有利的幣對進行套利
    public static void FindArbitrageOpportunities(List<string> pairs)
    {
        Dictionary<string, decimal> priceDifferences = new Dictionary<string, decimal>();

        foreach (var pair in pairs)
        {
            decimal priceBinance = GetPriceFromExchange(binanceAPI, pair);
            decimal priceGateio = GetPriceFromExchange(gateioAPI, pair);

            decimal priceDifference = (priceGateio - priceBinance) / priceBinance * 100; // 計算百分比差異

            priceDifferences[pair] = priceDifference;
        }

        // 根據價格差異選擇價差最大的幣對
        var bestPair = GetBestArbitragePair(priceDifferences);
        Console.WriteLine($"Best Arbitrage Pair: {bestPair.Key} with price difference of {bestPair.Value}%");

        if (bestPair.Value > 0.5m) // 設定套利門檻，例如 0.5%
        {
            ExecuteCrossExchangeArbitrage(bestPair.Key);
        }
    }

    private static decimal GetPriceFromExchange(string apiUrl, string pair)
    {
        var client = new RestClient(apiUrl + pair);
        var request = new RestRequest(Method.GET);
        var response = client.Execute(request);

        // 解析返回的價格數據
        var data = JObject.Parse(response.Content);
        decimal price = decimal.Parse(data["price"].ToString()); // 假設返回的是價格
        return price;
    }

    private static decimal GetFundingRate(string exchange, string pair)
    {
        // 模擬 API 請求來獲取資金費率
        return 0.01m; // 假設返回固定的資金費率
    }

    private static KeyValuePair<string, decimal> GetBestArbitragePair(Dictionary<string, decimal> priceDifferences)
    {
        decimal maxDiff = 0;
        string bestPair = string.Empty;

        foreach (var pair in priceDifferences)
        {
            if (pair.Value > maxDiff)
            {
                maxDiff = pair.Value;
                bestPair = pair.Key;
            }
        }

        return new KeyValuePair<string, decimal>(bestPair, maxDiff);
    }

    private static void ExecuteCrossExchangeArbitrage(string pair)
    {
        // 進行跨所套利交易
        Console.WriteLine($"Executing Cross-Exchange Arbitrage for {pair}...");
        // 這裡加入具體的交易邏輯
    }

    private static void ExecuteTriangularArbitrage()
    {
        // 進行三角套利交易
        Console.WriteLine("Executing Triangular Arbitrage...");
        // 這裡加入具體的交易邏輯
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        // 定義要監控的幣對
        List<string> tradingPairs = new List<string> { "BTCUSDT", "ETHUSDT", "XRPUSDT", "SOLUSDT" };

        // 執行跨所套利
        ArbitrageBot.CrossExchangeArbitrage();

        // 資金費率套利
        ArbitrageBot.FundingRateArbitrage();

        // 三角套利
        ArbitrageBot.TriangularArbitrage();

        // 期現套利
        ArbitrageBot.FuturesSpotArbitrage();
    }
}
