// <copyright file="Order.cs" company="ElmoLabs">
//  Copyright (c) All rights reserved.
// </copyright>
namespace AutoTrader.Console.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// A trade order.
    /// </summary>
    [DataContract]
    public class Order
    {
        /// <summary>
        /// Gets or sets order identification number.
        /// </summary>
        [DataMember(Name = "orderId")]
        public long Id { get; protected internal set; }

        /// <summary>
        /// Gets or sets cryptocurrency symbol.
        /// </summary>
        [DataMember(Name = "symbol")]
        public string Symbol { get; protected internal set; }

        /// <summary>
        /// Gets or sets price this order was bid on.
        /// </summary>
        [DataMember(Name = "price")]
        public double Price { get; protected internal set; }

        /// <summary>
        /// Gets or sets price this order was bid on.
        /// </summary>
        [DataMember(Name = "status")]
        public OrderStatus Status { get; protected internal set; }
    }
}