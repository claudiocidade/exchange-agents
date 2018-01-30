// <copyright file="TradeManager.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console
{
    using System.Threading;
    using System.Threading.Tasks;
    using AutoTrader.Console.Exchanges;
    using AutoTrader.Console.Models;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A trade entrance/exit manager.
    /// </summary>
    public class TradeManager 
    {
        /// <summary>
        /// An instance of the <see cref="ILogger"/> used for execution verbosity.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// An instance of the <see cref="IExchangeClient"/> class used to manipulate the exchange API.
        /// </summary>
        private readonly IExchangeClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeManager"/> class.
        /// </summary>
        /// <param name="client">An instance of the <see cref="IExchangeClient"/> class.</param>
        /// <param name="logger">An instance of the <see cref="ILogger"/> class.</param>
        public TradeManager(IExchangeClient client, ILogger logger)
        {
            this.client = client;

            this.logger = logger;
        }

        /// <summary>
        /// Executes a automatic trade on an specific symbol. 
        /// </summary>
        /// <param name="symbol">Symbol to be traded.</param>
        /// <param name="amount">Amount of BTC to invest.</param>
        /// <returns>An instance of the <see cref="Task"/> class.</returns>
        public async Task ExecuteTrade(string symbol, double amount)
        {
            Plan plan = this.CreateTradePlan(symbol, amount);

            long buyOrderId = await this.client.CreateOrder(symbol, plan.Bid, amount / plan.Bid, OrderSide.Buy);

            if (this.EnsureOrderFilled(symbol, buyOrderId, plan.Bid, 1.2))
            {
                await this.client.CreateOrder(symbol, plan.Bid, plan.Sell, OrderSide.Sell);

                if (this.EnsureOrderFilled(symbol, buyOrderId, plan.Bid, 1.1))
                {
                }
            }
        }

        /// <summary>
        /// Ensures that the order is processed and filled before reaching the specified treshold percentage.
        /// </summary>
        /// <param name="symbol">Name of the asset symbol to trade.</param>
        /// <param name="orderId">Order identification number.</param>
        /// <param name="bidPrice">The price to bid for this order.</param>
        /// <param name="percentageTreshold">The total percentage.</param>
        /// <returns>Whether or not the order has been filled within the estabilished criteria.</returns>
        private bool EnsureOrderFilled(string symbol, long orderId, double bidPrice, double percentageTreshold)
        {
            OrderStatus status = this.client.CheckOrderStatus(symbol, orderId).GetAwaiter().GetResult();

            double currentPrice = this.client.GetAssetPrice(symbol).GetAwaiter().GetResult();

            while (status != OrderStatus.Filled && (currentPrice < bidPrice * percentageTreshold && currentPrice > bidPrice * percentageTreshold))
            {
                Thread.Sleep(2000);

                status = this.client.CheckOrderStatus(symbol, orderId).GetAwaiter().GetResult();

                currentPrice = this.client.GetAssetPrice(symbol).GetAwaiter().GetResult();
            }

            return status == OrderStatus.Filled;
        }

        /// <summary>
        /// Creates a plan for this trade by evaluating and calculating the entrance and exit prices
        /// to benefit from sudden spikes in price.
        /// </summary>
        /// <param name="symbol">Name of the asset symbol to trade.</param>
        /// <param name="amount">Amount to invest in BTC.</param>
        /// <returns>An instance of the <see cref="Plan"/> representing the trade plan.</returns>
        private Plan CreateTradePlan(string symbol, double amount)
        {
            this.logger.LogInformation($"Creating the trade plan for {symbol} using {amount:F8} BTC");

            double currentPrice = this.client.GetAssetPrice(symbol).GetAwaiter().GetResult();

            this.logger.LogInformation($"Current price for {symbol} is {currentPrice:F8}");

            double bidPrice = currentPrice * 1.05;

            this.logger.LogInformation($"Bid price is {bidPrice:F8} (5% from current) for the amount of {(amount / bidPrice):F8}");

            double sellPrice = bidPrice * 1.7;

            this.logger.LogInformation($"Selling when {sellPrice:F8} (70% profit)");

            return new Plan(currentPrice, bidPrice, sellPrice, amount);
        }
    }
}