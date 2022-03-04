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

        private static readonly Random Random = new Random();

        public OrderService(IMongoClient mongoClient, IOrderDatabaseSettings settings)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _orders = database.GetCollection<Order>(settings.OrdersCollectionName);
        }

        public OrderCreateResponse Create(string userid, Order orderIn)
        {
            var order = new Order
            {
                Date = DateTime.UtcNow.ToString(CultureInfo.CurrentCulture),
                Paid = "pending",
                Userid = userid,
                Firstname = orderIn.Firstname,
                Lastname = orderIn.Lastname,
                Total = orderIn.Total,
                Address = orderIn.Address,
                Email = orderIn.Email,
                Delivery = orderIn.Delivery,
                Card = orderIn.Card,
                Cart = orderIn.Cart
            };

            _orders.InsertOne(order);

            const string transactionId = "pending";

            var paymentLoad = new PaymentLoad
            (orderIn.Card,
                orderIn.Firstname,
                orderIn.Lastname,
                orderIn.Address,
                orderIn.Total);

            var payment = MakePayment(paymentLoad);

            var response = new OrderCreateResponse();
            if (!string.IsNullOrEmpty(order.Id))
            {
                var orderFound = _orders.Find(orderDb => orderDb.Id == order.Id).FirstOrDefault();
                if (string.Equals(transactionId, payment.TransactionId)) return response;
                orderFound.Paid = payment.TransactionId;
                Update(orderFound.Id, orderFound);
                response.UserId = userid;
                response.OrderId = orderFound.Id;
                response.Payment = payment;
            }

            return response;
        }

        public List<OrderResponse> Get()
        {
            var orderList = _orders.Find(order => true).ToList();

            return FromOrderToOrderResponse(orderList);
        }

        public List<OrderResponse> Get(string userId)
        {
            var orderList = _orders.Find(order => order.Userid == userId).ToList();

            return FromOrderToOrderResponse(orderList);
        }

        private void Update(string id, Order orderIn) =>
            _orders.ReplaceOne(order => order.Id == id, orderIn);

        private static Payment MakePayment(PaymentLoad paymentLoad)
        {
            return new Payment
            {
                Success = "true",
                Message = "Payment processed",
                Amount = paymentLoad.Total,
                TransactionId = RandomTransactionId()
            };
        }

        private struct PaymentLoad
        {
            public PaymentLoad(Card card, string firstname, string lastname, Address address, string total)
            {
                _card = card;
                _firstname = firstname;
                _lastname = lastname;
                _address = address;
                Total = total;
            }

            private Card _card;
            private string _firstname;
            private string _lastname;
            private Address _address;
            public string Total { get; }
        }

        private static string RandomTransactionId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        private static List<OrderResponse> FromOrderToOrderResponse(List<Order> orderList)
        {
            return orderList.Select(order =>
                new OrderResponse
                {
                    Userid = order.Userid,
                    Firstname = order.Firstname,
                    Lastname = order.Lastname,
                    Address = order.Address,
                    Email = order.Email,
                    Delivery = order.Delivery,
                    Card = order.Card,
                    Cart = order.Cart,
                    Total = order.Total
                }).ToList();
        }
    }
}