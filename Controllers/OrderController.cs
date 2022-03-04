using acme_order.Models;
using acme_order.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using acme_order.Response;

namespace acme_order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("add/{userid:length(24)}")]
        public ActionResult<OrderCreateResponse> Create(string userid, Order orderIn)
        {
            return _orderService.Create(userid, orderIn);
        }

        [HttpGet("all")]
        public ActionResult<List<Order>> Get() =>
            _orderService.Get();

        [HttpGet("{id:length(24)}", Name = "GetOrder")]
        public ActionResult<Order> Get(string id)
        {
            var order = _orderService.Get(id);

            if (order == null)
            {
                return NotFound();
            }
            return order;
        }
    }
}