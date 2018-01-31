// <copyright file="Plan.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console.Models
{
    using System.Globalization;

    /// <summary>
    /// A trade entrance plan.
    /// </summary>
    public class Plan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plan"/> class.
        /// </summary>
        public Plan()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plan"/> class.
        /// </summary>
        /// <param name="current">The current price in BTC.</param>
        /// <param name="bid">The bid price in BTC.</param>
        /// <param name="sell">The sell price in BTC.</param>
        /// <param name="investmentAmount">InvestmentAmount of BTC to be used in the trade.</param>
        public Plan(double current, double bid, double sell, double investmentAmount)
        {
            this.Current = current;

            this.Bid = bid;

            this.Sell = sell;

            this.InvestmentAmount = investmentAmount;
        }

        /// <summary>
        /// Gets or sets current price.
        /// </summary>
        public double Current { get; protected internal set; }

        /// <summary>
        /// Gets or sets bid price.
        /// </summary>
        public double Bid { get; protected internal set; }

        /// <summary>
        /// Gets or sets sell price.
        /// </summary>
        public double Sell { get; protected internal set; }

        /// <summary>
        /// Gets or sets the amount of BTC to be used in the trade.
        /// </summary>
        public double InvestmentAmount { get; protected internal set; }

        /// <summary>
        /// Gets or sets the amount of BTC to be used in the trade.
        /// </summary>
        public double QuantityToBuy => (this.InvestmentAmount / this.Bid) > 0 
            ? System.Math.Floor(this.InvestmentAmount / this.Bid) 
            : this.InvestmentAmount / this.Bid;
    }
}