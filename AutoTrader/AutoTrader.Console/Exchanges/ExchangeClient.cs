// <copyright file="ExchangeClient.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console.Exchanges
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using AutoTrader.Console.Configuration;
    using AutoTrader.Console.Models;
    using RestSharp;

    /// <summary>
    /// An exchange manipulation client.
    /// </summary>
    public abstract class ExchangeClient : IExchangeClient
    {
        /// <summary>
        /// An instance of the <see cref="IRestClient"/> class used to manipulate the exchange API.
        /// </summary>
        protected readonly IRestClient Client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeClient"/> class.
        /// </summary>
        /// <param name="client">An instance of the <see cref="IRestClient"/> 
        /// to be used to manipulate the exchange API.</param>
        protected ExchangeClient(IRestClient client)
        {
            this.Client = client;
        }

        /// <summary>
        /// Gets the timestamp used to sign requests to the exchange.
        /// </summary>
        protected string Timestamp => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds().ToString();

        /// <summary>
        /// Gets current price information of an asset symbol.
        /// </summary>
        /// <param name="symbol">Name of the cryptocurrency asset symbol.</param>
        /// <returns>The current price information for the symbol.</returns>
        public abstract Task<double> GetAssetPrice(string symbol);

        /// <summary>
        /// Creates a new trade order.
        /// </summary>
        /// <param name="symbol">Name of the cryptocurrency asset symbol.</param>
        /// <param name="bidPrice">The price to bid for this order.</param>
        /// <param name="amount">The amount of assets to be traded.</param>
        /// <param name="side">Type of the order that will be created.</param>
        /// <returns>The order identification number.</returns>
        public abstract Task<long> CreateOrder(string symbol, double bidPrice, double amount, OrderSide side);

        /// <summary>
        /// Checks the order status.
        /// </summary>
        /// <param name="symbol">Name of the cryptocurrency asset symbol.</param>
        /// <param name="orderId">Order identification number.</param>
        /// <returns><see cref="OrderStatus"/>.</returns>
        public abstract Task<OrderStatus> CheckOrderStatus(string symbol, long orderId);

        /// <summary>
        /// Cancel a trade order.
        /// </summary>
        /// <param name="symbol">Name of the cryptocurrency asset symbol.</param>
        /// <param name="orderId">Order identification number.</param>
        /// <returns><see cref="OrderStatus"/>.</returns>
        public abstract Task CancelOrder(string symbol, long orderId);

        /// <summary>
        /// Gets an HMACSHA256 signature to authorize requests to the exchange API.
        /// </summary>
        /// <param name="message">Request body message.</param>
        /// <returns>An HMACSHA256 signature.</returns>
        protected string GetSignature(string message)
        {
            HMACSHA256 hash = new HMACSHA256(System.Text.Encoding.Default.GetBytes(ApplicationConstants.SecretKey));
            
            byte[] signatureBytes = hash.ComputeHash(System.Text.Encoding.Default.GetBytes(message));

            return BitConverter.ToString(signatureBytes).Replace("-", string.Empty);
        }

        /// <summary>
        /// Gets the result of a rest request response.
        /// </summary>
        /// <param name="request">An instance of the <see cref="IRestRequest"/> to be executed.</param>
        /// <returns>An instance of <see cref="IRestResponse"/>.</returns>
        protected async Task<IRestResponse> GetRequestResult(IRestRequest request)
        {
            TaskCompletionSource<IRestResponse> taskCompletionSource = new TaskCompletionSource<IRestResponse>();

            this.Client.ExecuteAsync(request, handle => taskCompletionSource.SetResult(handle));

            return (RestResponse)(await taskCompletionSource.Task);
        }  
    }
}