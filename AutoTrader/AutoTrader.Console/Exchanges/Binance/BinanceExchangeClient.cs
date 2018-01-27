// <copyright file="BinanceExchangeClient.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console.Exchanges.Binance
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoTrader.Console.Configuration;
    using AutoTrader.Console.Models;
    using Newtonsoft.Json;
    using RestSharp;

    /// <summary>
    /// An exchange manipulation client.
    /// </summary>
    public class BinanceExchangeClient : ExchangeClient, IBinanceExchangeClient
    {
        /// <summary>
        /// Gets current price information of an asset symbol.
        /// </summary>
        /// <param name="symbol">Name of the cryptocurrency asset symbol.</param>
        /// <returns>The current price information for the symbol.</returns>
        public override async Task<double> GetAssetPrice(string symbol)
        {
            RestRequest request = new RestRequest("ticker/price", Method.GET);

            request.AddQueryParameter("symbol", symbol);

            TaskCompletionSource<IRestResponse> taskCompletionSource = new TaskCompletionSource<IRestResponse>();

            Client.ExecuteAsync(request, handle => taskCompletionSource.SetResult(handle));

            RestResponse response = (RestResponse)(await taskCompletionSource.Task);

            dynamic result = JsonConvert.DeserializeObject(response.Content);

            return result.price;
        }

        /// <summary>
        /// Creates a new trade order.
        /// </summary>
        /// <param name="symbol">Name of the cryptocurrency asset symbol.</param>
        /// <param name="bidPrice">The price to bid for this order.</param>
        /// <param name="amount">The amount of assets to be traded.</param>
        /// <param name="type">Type of the order that will be created.</param>
        /// <returns>The order identification number.</returns>
        public override async Task<long> CreateOrder(string symbol, double bidPrice, double amount, OrderType type)
        {
            RestRequest request = new RestRequest("order/test", Method.POST);

            request.AddHeader("X-MBX-APIKEY", ApplicationConstants.AppKey);

            request.Parameters.AddRange(this.CreateOrderParameterList(symbol, bidPrice, amount, type));

            TaskCompletionSource<IRestResponse> taskCompletionSource = new TaskCompletionSource<IRestResponse>();

            Client.ExecuteAsync(request, handle => taskCompletionSource.SetResult(handle));

            RestResponse response = (RestResponse)(await taskCompletionSource.Task);
            
            dynamic result = JsonConvert.DeserializeObject(response.Content);

            return result.OrderId;
        }

        /// <summary>
        /// Creates a new trade order parameter list to be included in the request.
        /// </summary>
        /// <param name="symbol">Name of the cryptocurrency asset symbol.</param>
        /// <param name="bidPrice">The price to bid for this order.</param>
        /// <param name="amount">The amount of assets to be traded.</param>
        /// <param name="type">Type of the order that will be created.</param>
        /// <returns>The order identification number.</returns>
        private IList<Parameter> CreateOrderParameterList(string symbol, double bidPrice, double amount, OrderType type)
        {
            IList<Parameter> parameters = new List<Parameter>();

            string timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds().ToString();

            parameters.Add(new Parameter() { Name = "symbol", Value = symbol, Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter() { Name = "quantity", Value = $"{amount:F0}", Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter() { Name = "price", Value = $"{bidPrice:F8}", Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter() { Name = "timeInForce", Value = "GTC", Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter() { Name = "type", Value = "LIMIT", Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter() { Name = "timestamp", Value = timestamp, Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter() { Name = "side", Value = type.ToString().ToUpper(), Type = ParameterType.GetOrPost });

            string signatureMessage = string.Join("&", parameters.Select(item => $"{item.Name}={item.Value}"));

            parameters.Add(new Parameter() { Name = "signature", Value = this.GetSignature(signatureMessage), Type = ParameterType.GetOrPost });

            return parameters;
        }
    }
}