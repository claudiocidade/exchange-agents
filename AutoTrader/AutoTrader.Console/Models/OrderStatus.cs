// <copyright file="OrderStatus.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The types of trade orders.
    /// </summary>
    [DataContract]
    public enum OrderStatus
    {
        /// <summary>
        /// Just created order.
        /// </summary>
        [EnumMember(Value = "NEW")]
        New,

        /// <summary>
        /// Only a portion of this order has been executed.
        /// </summary>
        [EnumMember(Value = "PARTIALLY_FILLED")]
        PartiallyFilled,

        /// <summary>
        /// Order has been executed.
        /// </summary>
        [EnumMember(Value = "FILLED")]
        Filled,

        /// <summary>
        /// Order has been canceled.
        /// </summary>
        [EnumMember(Value = "CANCELED")]
        Canceled,

        /// <summary>
        /// Whatever happened to this order is unknown.
        /// </summary>
        Undefined
    }
}