// <copyright file="Program.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console
{
    using System;
    using System.Globalization;
    using System.Security.Cryptography;
    using AutoTrader.Console.Exchanges;
    using AutoTrader.Console.Exchanges.Binance;
    using AutoTrader.Console.Models;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using StructureMap;

    /// <summary>
    /// Program runtime initialization class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// An instance of the <see cref="ILogger"/> used for execution verbosity.
        /// </summary>
        private static readonly ILogger Logger;

        /// <summary>
        /// Dependency injection container.
        /// </summary>
        private static readonly IContainer Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        static Program()
        {
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole());
            
            // add StructureMap
            Container = new Container();

            Container.Configure(config =>
            {
                // Scan the project for conventions based auto-registration
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Program));
                    _.WithDefaultConventions();
                });

                // Populate the container using the service collection
                config.Populate(serviceCollection);
            });

            // add the framework services
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            Logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger("Info");
        }

        /// <summary>
        /// Runtime initialization method.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            string symbol = $"{(args.Length > 1 ? args[1].ToUpper() : "ADA")}BTC";
            
            double amount = Double.Parse(args.Length > 2 ? args[2].ToUpper() : "0.01000000", NumberStyles.AllowDecimalPoint);

            IExchangeClient client = Container.GetInstance<IBinanceExchangeClient>();

            Plan plan = CreateTradePlan(client, symbol, amount);

            long orderId = client.CreateOrder(symbol, plan.Bid, amount / plan.Bid, Order.Type.Buy).GetAwaiter().GetResult();

            Logger.LogInformation($"Buy order created {orderId}");

            Console.ReadKey();
        }

        private static Plan CreateTradePlan(IExchangeClient client, string symbolName, double amount)
        {
            Logger.LogInformation($"Creating the trade plan for {symbolName} using {amount:F8} BTC");

            double currentPrice = client.GetAssetPrice(symbolName).GetAwaiter().GetResult();

            Logger.LogInformation($"\tCurrent price for {symbolName} is {currentPrice:F8}");

            double bidPrice = currentPrice * 1.05;

            Logger.LogInformation($"\tBid price is {bidPrice:F8} (5% from current) for the amount of {(amount / bidPrice):F8}");

            double sellPrice = bidPrice * 1.7;

            Logger.LogInformation($"\tSelling when {sellPrice:F8} (70% profit)");

            return new Plan(currentPrice, bidPrice, sellPrice, amount);
        }
    }
}