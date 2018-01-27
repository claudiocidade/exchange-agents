// <copyright file="IExchangeClient.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console.Exchanges
{
    using System.Threading.Tasks;
    using AutoTrader.Console.Models;

    /// <summary>
    /// An implementation contract for an exchange manipulation client.
    /// </summary>
    public interface IExchangeClient
    {
        /// <summary>
        /// Gets current price information of an asset symbol.
        /// </summary>
        /// <param name="symbol">Name of the cryptocurrency asset symbol.</param>
        /// <returns>The current price information for the symbol.</returns>
        Task<double> GetAssetPrice(string symbol);

        /// <summary>
        /// Creates a new trade order.
        /// </summary>
        /// <param name="symbol">Name of the cryptocurrency asset symbol.</param>
        /// <param name="bidPrice">The price to bid for this order.</param>
        /// <param name="amount">The amount of assets to be traded.</param>
        /// <param name="type">Type of the order that will be created.</param>
        /// <returns>The order identification number.</returns>
        Task<long> CreateOrder(string symbol, double bidPrice, double amount, Order.Type type);
    }
}