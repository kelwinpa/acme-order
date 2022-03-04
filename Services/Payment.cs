using System.Text.Json.Serialization;

namespace acme_order.Models
{
    public class Payment
    {
        public string Amount  { get; set; }
        public string Message  { get; set; }
        public string Success { get; set; }
        [JsonPropertyName("transactionID")]
        public string TransactionId { get; set; }
    }
}