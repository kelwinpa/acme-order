namespace acme_order.Models
{
    public class Paymentres
    {
        public Paymentres(string success, string message, string amount, string transactionId)
        {
            _success = success;
            _message = message;
            _amount = amount;
            TransactionId = transactionId;
        }

        private string _success;
        private string _message;
        private string _amount;
        public string TransactionId;
    }
}