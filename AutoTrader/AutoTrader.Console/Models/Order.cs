// <copyright file="Order.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console.Models
{
    /// <summary>
    /// An exchange buy/sell order.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        public Order()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        /// <param name="symbol">Symbol of the cryptocurrency this order applies to.</param>
        /// <param name="price">Bid price in BTC.</param>
        /// <param name="amount">Amount to be bought/sold.</param>
        /// <param name="type">Type of the trade order.</param>
        public Order(string symbol, double price, double amount, Order.Type type)
        {
            this.Symbol = symbol;

            this.Price = price;

            this.Amount = amount;

            this.OrderType = type;
        }

        /// <summary>
        /// Gets or sets asset symbol.
        /// </summary>
        public string Symbol { get; protected internal set; }

        /// <summary>
        /// Gets or sets bid price.
        /// </summary>
        public double Price { get; protected internal set; }

        /// <summary>
        /// Gets or sets the amount of BTC to be used in the trade.
        /// </summary>
        public double Amount { get; protected internal set; }

        /// <summary>
        /// Gets or sets the type of the trade order.
        /// </summary>
        public Order.Type OrderType { get; protected internal set; }

        /// <summary>
        /// Types of trade order.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Buy order.
            /// </summary>
            Buy,

            /// <summary>
            /// Sell order.
            /// </summary>
            Sell
        }
    }
}