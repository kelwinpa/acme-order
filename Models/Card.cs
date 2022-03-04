namespace acme_order.Models
{
    public class Card
    {
        public Card(string type, string number, string expMonth, string expYear, string ccv)
        {
            Type = type;
            Number = number;
            ExpMonth = expMonth;
            ExpYear = expYear;
            Ccv = ccv;
        }

        public string Type { get; set; }
        public string Number { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string Ccv { get; set; }
    }
}