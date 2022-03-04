using System;
using acme_order.Models;

namespace acme_order.Response
{
    public class OrderCreateResponse
    {
        public OrderCreateResponse(string userId, string orderId, Paymentres paymentres)
        {
            UserId = userId;
            OrderId = orderId;
            Paymentres = paymentres;
        }
        
        public String UserId;
        public String OrderId;
        public Paymentres Paymentres;
    }
}