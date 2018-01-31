// <copyright file="Program.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using AutoTrader.Console.Configuration;
    using AutoTrader.Console.Exchanges.Binance;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using StructureMap;
    using StructureMap.Pipeline;

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

                config.For<IRestClient>(new TransientLifecycle()).Add(new RestClient(ApplicationConstants.ExchangeUri));

                // Populate the container using the service collection
                config.Populate(serviceCollection);
            });

            // add the framework services
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            
            TradeManager = new TradeManager(container.GetInstance<ICryptopiaExchangeClient>(), serviceProvider.GetService<ILoggerFactory>().CreateLogger("Info"));
        }

        /// <summary>
        /// Runtime initialization method.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static async Task Main(string[] args)
        {
            string symbol = $"{(args.Length > 1 ? args[1].ToUpper() : "LTC")}BTC";

            double amount = double.Parse(args.Length > 2 ? args[2].ToUpper() : "0.01", NumberStyles.AllowDecimalPoint);

            await TradeManager.ExecuteTrade(symbol, amount);

            Console.ReadKey();
        }
    }
}