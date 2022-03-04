namespace acme_order.Models
{
    public class Cart
    {
        public Cart(string id, string description, string quantity, string price)
        {
            Id = id;
            Description = description;
            Quantity = quantity;
            Price = price;
        }
        public string Id { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
    }
}