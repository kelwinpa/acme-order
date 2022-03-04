using System;
using System.Text.Json.Serialization;
using acme_order.Models;

namespace acme_order.Response
{
    public class OrderCreateResponse
    {
        [JsonPropertyName("userid")]
        public string UserId { get; set; }
        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }
        public Payment Payment { get; set; }
    }
}