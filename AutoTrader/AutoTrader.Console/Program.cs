// <copyright file="Program.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console
{
    using System;
    using System.Globalization;
    using System.Threading;
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
        /// Dependency injection container.
        /// </summary>
        private static readonly TradeManager TradeManager;

        /// <summary>
        /// Initializes static members of the <see cref="Program"/> class.
        /// </summary>
        static Program()
        {
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole());

            // add StructureMap
            Container container = new Container();

            container.Configure(config =>
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
            
            TradeManager = new TradeManager(container.GetInstance<IBinanceExchangeClient>(), serviceProvider.GetService<ILoggerFactory>().CreateLogger("Info"));
        }

        /// <summary>
        /// Runtime initialization method.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            string symbol = $"{(args.Length > 1 ? args[1].ToUpper() : "ADA")}BTC";

            double amount = double.Parse(args.Length > 2 ? args[2].ToUpper() : "0.01000000", NumberStyles.AllowDecimalPoint);

            TradeManager.ExecuteTrade(symbol, amount).GetAwaiter().GetResult();

            Console.ReadKey();
        }
    }
}