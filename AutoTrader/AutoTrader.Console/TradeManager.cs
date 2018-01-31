// <copyright file="TradeManager.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console
{
    using System;
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
        /// <param name="amount">InvestmentAmount of BTC to invest.</param>
        /// <returns>An instance of the <see cref="Task"/> class.</returns>
        public async Task ExecuteTrade(string symbol, double amount)
        {
            Plan plan = await this.CreateTradePlan(symbol, amount);

            long buyOrderId = await this.client.CreateOrder(symbol, plan.Bid * .5, amount / plan.Bid, OrderSide.Buy);

            logger.LogInformation($"A {OrderSide.Buy} order was placed for the amount of {plan.QuantityToBuy:F0} {symbol} at {plan.Bid *.5}.");

            if (await this.EnsureOrderFilled(symbol, buyOrderId, plan.Bid, 1.2, 1))
            {
                await this.client.CreateOrder(symbol, plan.Sell, plan.QuantityToBuy, OrderSide.Sell);

                if (await this.EnsureOrderFilled(symbol, buyOrderId, plan.Bid, 1.1, 1))
                {
                    this.logger.LogInformation("SUCCESS: Trade executed succesfully");
                }
            }
        }

        /// <summary>
        /// Ensures that the order is processed and filled before reaching the specified treshold percentage.
        /// </summary>
        /// <param name="symbol">Name of the asset symbol to trade.</param>
        /// <param name="orderId">Order identification number.</param>
        /// <param name="bidPrice">The price to bid for this order.</param>
        /// <param name="percentageTreshold">The threshold percentage (above and below the price).</param>
        /// <param name="timeOut">Timeout for retries (in minutes).</param>
        /// <returns>Whether or not the order has been filled within the estabilished criteria.</returns>
        private async Task<bool> EnsureOrderFilled(string symbol, long orderId, double bidPrice, double percentageTreshold, int timeOut)
        {
            OrderStatus status = await this.client.CheckOrderStatus(symbol, orderId);

            double currentPrice = await this.client.GetAssetPrice(symbol);

            DateTime start = DateTime.Now;

            while (status != OrderStatus.Filled && (currentPrice < bidPrice * percentageTreshold || currentPrice > bidPrice * (percentageTreshold - 1)) && DateTime.Now < start.AddMinutes(timeOut))
            {
                Thread.Sleep(2000);

                status = await this.client.CheckOrderStatus(symbol, orderId);

                currentPrice = await this.client.GetAssetPrice(symbol);
            }

            if (status != OrderStatus.Filled)
            {
                await this.client.CancelOrder(symbol, orderId);

                this.logger.LogInformation("FAIL: Unfortunatelly the trade entrace timeout expired and this trade is no longer worth the risk");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a plan for this trade by evaluating and calculating the entrance and exit prices
        /// to benefit from sudden spikes in price.
        /// </summary>
        /// <param name="symbol">Name of the asset symbol to trade.</param>
        /// <param name="amount">InvestmentAmount to invest in BTC.</param>
        /// <returns>An instance of the <see cref="Plan"/> representing the trade plan.</returns>
        private async Task<Plan> CreateTradePlan(string symbol, double amount)
        {
            this.logger.LogInformation($"Creating the trade plan for {symbol} using {amount:F8} BTC");

            double currentPrice = await this.client.GetAssetPrice(symbol);

            double bidPrice = currentPrice * 1.05;

            double sellPrice = bidPrice * 1.7;

            Plan plan = new Plan(currentPrice, bidPrice, sellPrice, amount);

            this.logger.LogInformation($"Current price for {symbol} is {plan.Current:F8}");

            this.logger.LogInformation($"Bid price is {plan.Bid:F8} (5% from current) for the amount of {plan.QuantityToBuy:F8}");

            this.logger.LogInformation($"Selling when {plan.Sell:F8} (70% profit)");

            return plan;
        }
    }
}