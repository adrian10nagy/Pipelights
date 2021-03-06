using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pipelights.Database.Models;
using Pipelights.Database.Services;
using Pipelights.Website.BusinessService.Models;
using Pipelights.Website.Models;

namespace Pipelights.Website.BusinessService
{
    public interface IOrderService
    {
        string SaveOrder(Order order, Cart cart);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderDbService _orderDbService;

        public OrderService(IOrderDbService orderDbService)
        {
            _orderDbService = orderDbService;
        }

        public string SaveOrder(Order order, Cart cart)
        {
            try
            {
                var orderId = $"{DateTime.Now:yyMMdd-HHmmss}";

                var addOrderTask = _orderDbService.AddAsync(new OrderEntity
                {

                    Name = order.name,
                    Surname = order.surname,
                    Address = order.address,
                    Email = order.email,
                    Judete = order.judete,
                    Localitate = order.localitate,
                    Payment = order.payment,
                    Telephone = order.telephone,
                    PlacedDate = DateTime.Now,
                    Subtotal = 0,
                    CartProducts = GetCartProducts(cart),
                    id = orderId

                });
                var task = Task.Run(async () => await addOrderTask);
                task.GetAwaiter().GetResult();
                
                return orderId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private List<CartProductsEntity> GetCartProducts(Cart cart)
        {
            var result = new List<CartProductsEntity>();

            if (cart == null || cart.ProductsForCart == null)
            {
                return result;
            }

            foreach (var product in cart.ProductsForCart)
            {
                result.Add(new CartProductsEntity
                {
                    Price = !string.IsNullOrEmpty(product.ProductDetails.PriceReduced) ? product.ProductDetails.PriceReduced : product.ProductDetails.Price,
                    Quantity = product.Quantity,
                    LampId = product.ProductDetails.Id
                });
            }

            return result;
        }
    }
}
