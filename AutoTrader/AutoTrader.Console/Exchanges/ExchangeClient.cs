// <copyright file="ExchangeClient.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console.Exchanges
{
    using System;
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
        /// An instance of the <see cref="RestClient"/> class used to manipulate the exchange API.
        /// </summary>
        protected readonly RestClient Client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeClient"/> class.
        /// </summary>
        protected ExchangeClient()
        {
            this.Client = new RestClient(ApplicationConstants.ExchangeUri);
        }

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
        /// <param name="type">Type of the order that will be created.</param>
        /// <param name="amount">The amount of assets to be traded.</param>
        /// <returns>The order identification number.</returns>
        public abstract Task<long> CreateOrder(string symbol, double bidPrice, double amount, Order.Type type);

        /// <summary>
        /// Gets an HMACSHA256 signature to authorize requests to the exchange API.
        /// </summary>
        /// <param name="message">Request body message.</param>
        /// <returns>An HMACSHA256 signature.</returns>
        protected string GetSignature(string message)
        {
            HMACSHA256 hash = new HMACSHA256(System.Text.Encoding.Default.GetBytes(ApplicationConstants.SecretKey));
            
            byte[] signatureBytes = hash.ComputeHash(System.Text.Encoding.Default.GetBytes(message));

            return BitConverter.ToString(signatureBytes).Replace("-", "");
        }
    }
}