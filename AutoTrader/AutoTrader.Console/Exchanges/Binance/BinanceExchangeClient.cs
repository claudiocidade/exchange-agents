// <copyright file="BinanceExchangeClient.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console.Exchanges.Binance
{
    using System.Collections.Generic;
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
        /// The string key of the header value for the API KEY information.
        /// </summary>
        private const string ApiKeyHeaderKey = "X-MBX-APIKEY";

        /// <summary>
        /// Initializes a new instance of the <see cref="BinanceExchangeClient"/> class.
        /// </summary>
        /// <param name="client">An instance of the <see cref="IRestClient"/> 
        /// to be used to manipulate the exchange API.</param>
        public BinanceExchangeClient(IRestClient client) : base(client)
        {
        }

        /// <summary>
        /// Gets current price information of an asset symbol.
        /// </summary>
        /// <param name="symbol">Name of the cryptocurrency asset symbol.</param>
        /// <returns>The current price information for the symbol.</returns>
        public override async Task<double> GetAssetPrice(string symbol)
        {
            RestRequest request = new RestRequest("ticker/price", Method.GET);

            request.AddQueryParameter("symbol", symbol);
            
            dynamic result = JsonConvert.DeserializeObject((await this.GetRequestResult(request)).Content);

            return result.price;
        }

        /// <summary>
        /// Creates a new trade order.
        /// </summary>
        /// <param name="symbol">Name of the cryptocurrency asset symbol.</param>
        /// <param name="bidPrice">The price to bid for this order.</param>
        /// <param name="amount">The amount of assets to be traded.</param>
        /// <param name="side">Type of the order that will be created.</param>
        /// <returns>The order identification number.</returns>
        public override async Task<long> CreateOrder(string symbol, double bidPrice, double amount, OrderSide side)
        {
            RestRequest request = new RestRequest("order/test", Method.POST);

            request.AddHeader(ApiKeyHeaderKey, ApplicationConstants.AppKey);

            IList<Parameter> parameters = new List<Parameter>();

            parameters.Add(new Parameter() { Name = "symbol", Value = symbol, Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter() { Name = "quantity", Value = $"{amount:F0}", Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter() { Name = "price", Value = $"{bidPrice:F8}", Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter() { Name = "timeInForce", Value = "GTC", Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter() { Name = "side", Value = "LIMIT", Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter() { Name = "timestamp", Value = this.Timestamp, Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter() { Name = "side", Value = side.ToString().ToUpper(), Type = ParameterType.GetOrPost });

            string signatureMessage = string.Join("&", parameters.Select(item => $"{item.Name}={item.Value}"));

            parameters.Add(new Parameter() { Name = "signature", Value = this.GetSignature(signatureMessage), Type = ParameterType.GetOrPost });

            request.Parameters.AddRange(parameters);

            dynamic result = JsonConvert.DeserializeObject((await this.GetRequestResult(request)).Content);

            return result.OrderId;
        }

        /// <summary>
        /// Checks the order status.
        /// </summary>
        /// <param name="symbol">Name of the cryptocurrency asset symbol.</param>
        /// <param name="orderId">Order identification number.</param>
        /// <returns><see cref="OrderStatus"/>.</returns>
        public override async Task<OrderStatus> CheckOrderStatus(string symbol, long orderId)
        {
            RestRequest request = new RestRequest("order", Method.GET);

            request.AddHeader(ApiKeyHeaderKey, ApplicationConstants.AppKey);

            IList<Parameter> parameters = new List<Parameter>();

            parameters.Add(new Parameter() { Name = "symbol", Value = symbol, Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter() { Name = "orderId", Value = orderId, Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter() { Name = "timestamp", Value = this.Timestamp, Type = ParameterType.GetOrPost });

            string signatureMessage = string.Join("&", parameters.Select(item => $"{item.Name}={item.Value}"));

            parameters.Add(new Parameter() { Name = "signature", Value = this.GetSignature(signatureMessage), Type = ParameterType.GetOrPost });

            request.Parameters.AddRange(parameters);

            dynamic result = JsonConvert.DeserializeObject((await this.GetRequestResult(request)).Content);

            switch (result.status)
            {
                case "NEW":
                    return OrderStatus.New;
                case "PARTIALLY_FILLED":
                    return OrderStatus.PartiallyFilled;
                case "FILLED":
                    return OrderStatus.Filled;
                case "CANCELED":
                    return OrderStatus.Canceled;
                default:
                    return OrderStatus.Undefined;
            }
        }

        /// <summary>
        /// Cancel a trade order.
        /// </summary>
        /// <param name="symbol">Name of the cryptocurrency asset symbol.</param>
        /// <param name="orderId">Order identification number.</param>
        /// <returns><see cref="OrderStatus"/>.</returns>
        public override async Task CancelOrder(string symbol, long orderId)
        {
            RestRequest request = new RestRequest("order", Method.DELETE);

            request.AddHeader(ApiKeyHeaderKey, ApplicationConstants.AppKey);

            IList<Parameter> parameters = new List<Parameter>();

            parameters.Add(new Parameter { Name = "symbol", Value = symbol, Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter { Name = "orderId", Value = orderId, Type = ParameterType.GetOrPost });
            parameters.Add(new Parameter { Name = "timestamp", Value = this.Timestamp, Type = ParameterType.GetOrPost });

            string signatureMessage = string.Join("&", parameters.Select(item => $"{item.Name}={item.Value}"));

            parameters.Add(new Parameter() { Name = "signature", Value = this.GetSignature(signatureMessage), Type = ParameterType.GetOrPost });

            request.Parameters.AddRange(parameters);

            await this.GetRequestResult(request);
        }
    }
}