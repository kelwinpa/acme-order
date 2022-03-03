using System;
using acme_order.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace acme_order.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _orders;
        
        private static readonly HttpClient client = new HttpClient();
        
        private static Random random = new Random();

        public OrderService(IMongoClient mongoClient, IOrderDatabaseSettings settings)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _orders = database.GetCollection<Order>(settings.OrdersCollectionName);
        }

        public List<Order> Get() =>
            _orders.Find(order => true).ToList();

        public Order Get(string id) =>
            _orders.Find<Order>(order => order.Id == id).FirstOrDefault();

        public Order Create(Order order)
        {
            _orders.InsertOne(order);
            return order;
        }

        public void Update(string id, Order orderIn) =>
            _orders.ReplaceOne(order => order.Id == id, orderIn);

        public void Remove(Order orderIn) =>
            _orders.DeleteOne(order => order.Id == orderIn.Id);

        public void Remove(string id) =>
            _orders.DeleteOne(order => order.Id == id);


        public void Create(string userid, Order orderIn)
        {
            Order order = new Order();
            order.Id = Guid.NewGuid().ToString();
            order.Date = DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
            order.Paid = "pending";
            order.Userid = userid;
            
            order.Firstname = orderIn.Firstname;
            order.Lastname = orderIn.Lastname;
            order.Total = orderIn.Total;
            order.Address = orderIn.Address;
            order.Email = orderIn.Email;
            order.Delivery = orderIn.Delivery;
            order.Card = orderIn.Card;
            order.Cart = orderIn.Cart;

            orderIn.Date = DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
            orderIn.Paid = "pending";
            var transactionId = "pending";
            var paymentLoad = new PaymentLoad
                (orderIn.Card,
                orderIn.Firstname,
                orderIn.Lastname,
                orderIn.Address,
                orderIn.Total);
            
            _orders.InsertOne(order);
            var paymentres = makePayment(paymentLoad);

            if (string.IsNullOrEmpty(order.Id))
            {
                
            }
            
        }

        private Paymentres makePayment(PaymentLoad paymentLoad)
        {
            var transactionId = RandomString();
            return new Paymentres("true", "payment successfully",paymentLoad.Total,transactionId);
        }
        
        private struct PaymentLoad
        {
            public PaymentLoad(string card, string firstname, string lastname, string address, string total)
            {
                _card = card;
                _firstname = firstname;
                _lastname = lastname;
                _address = address;
                Total = total;
            }

            private string _card;
            private string _firstname;
            private string _lastname;
            private string _address;
            public string Total{ get; }

        }
        
        
        private struct Paymentres
        {
            public Paymentres(string success, string message, string amount, string transactionId)
            {
                _success = success;
                _message = message;
                _amount = amount;
                _transactionId = transactionId;
            }

            private string _success;
            private string _message;
            private string _amount;
            private string _transactionId;
        }
        
        private static string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
    }


    
}