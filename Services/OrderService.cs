using System;
using acme_order.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using acme_order.Response;

namespace acme_order.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _orders;

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


        public OrderCreateResponse Create(string userid, Order orderIn)
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
            
            _orders.InsertOne(order);
            
            var transactionId = "pending";
            
            var paymentLoad = new PaymentLoad
                (orderIn.Card,
                orderIn.Firstname,
                orderIn.Lastname,
                orderIn.Address,
                orderIn.Total);
            
            Paymentres paymentres = makePayment(paymentLoad);

            if (string.IsNullOrEmpty(order.Id))
            {
               var orderFound = _orders.Find<Order>(orderDb => orderDb.Id == order.Id).FirstOrDefault();
               if (!String.Equals(transactionId, paymentres.TransactionId))
               {
                   orderFound.Paid = paymentres.TransactionId;
                   _orders.InsertOne(orderFound);
               }
            }

            return new OrderCreateResponse(userid, order.Id, paymentres);
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
        
        private static string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
    }
}