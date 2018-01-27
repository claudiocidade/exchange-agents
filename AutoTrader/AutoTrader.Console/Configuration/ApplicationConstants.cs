//  <copyright file="ApplicationConstants.cs" company="ElmoLabs">
//  Copyright (c) ElmoLabs. All rights reserved.
//  </copyright>
namespace AutoTrader.Console.Configuration
{
    using System;

    /// <summary>
    /// Global application configuration keys and constant string values.
    /// </summary>
    public static class ApplicationConstants
    {
        /// <summary>
        /// Gets the exchange URI address.
        /// </summary>
        public static Uri ExchangeUri => new Uri("https://api.binance.com/api/v3/");

        /// <summary>
        /// Gets the application key.
        /// </summary>
        public static string AppKey => "3H4VEZMKKcfg7IUgNG9nz0pIS6YTuEvtLRljDCkhbDoyTdMa6f0MB8uqUJx0yv8v";

        /// <summary>
        /// Gets the secret key.
        /// </summary>
        public static string SecretKey => "nlfLlAMbmqExHsCApdWmF7NnGzChby7XEybB0QhF88UlMxn51wd5nuPV79Mb0TFZ";
    }
}