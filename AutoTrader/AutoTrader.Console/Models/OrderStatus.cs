// <copyright file="OrderStatus.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console.Models
{
    /// <summary>
    /// The types of trade orders.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Just created order.
        /// </summary>
        New,

        /// <summary>
        /// Only a portion of this order has been executed.
        /// </summary>
        PartiallyFilled,

        /// <summary>
        /// Order has been executed.
        /// </summary>
        Filled,

        /// <summary>
        /// Order has been canceled.
        /// </summary>
        Canceled,

        /// <summary>
        /// Whatever happened to this order is unknown.
        /// </summary>
        Undefined
    }
}