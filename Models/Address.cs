namespace acme_order.Models
{
    public class Address
    {
        public Address(string street, string city, string zip, string state, string country)
        {
            Street = street;
            City = city;
            Zip = zip;
            State = state;
            Country = country;
        }
        public string Street { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}