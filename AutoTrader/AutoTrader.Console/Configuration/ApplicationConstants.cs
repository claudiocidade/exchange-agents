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
        public static string AppKey => "";

        /// <summary>
        /// Gets the secret key.
        /// </summary>
        public static string SecretKey => "";
    }
}
