using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        IEnumerable<OrderEntity> GetMultiple(string query);
        IEnumerable<OrderEntity> GetMultiple(int max = 1000);
        OrderEntity GetById(string id);
       
        void InsertOrder(OrderEntity model);
        void EditOrder(OrderEntity model);
        OrderEntity DeleteOrder(string id);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderDbService _orderDbService;

        public OrderService(IOrderDbService orderDbService)
        {
            _orderDbService = orderDbService;
        }

        public OrderEntity GetById(string id)
        {
            return _orderDbService.GetAsync(id).Result;
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
                    id = orderId,
                    Voucher = cart.voucher
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

        public IEnumerable<OrderEntity> GetMultiple(string query)
        {

            var dbResult = _orderDbService.GetMultipleAsync(query).Result;


            return dbResult;
        }

        public IEnumerable<OrderEntity> GetMultiple(int max = 1000)
        {
            var dbResult = GetMultiple("SELECT * FROM c order by c._ts DESC");

            var result = dbResult.Take(max);

            return result;
        }

        public OrderEntity DeleteOrder(string id)
        {
            OrderEntity order = GetById(id);
            var deletedOrder = _orderDbService.DeleteAsync(id);

            return order;
        }

        public void EditOrder(OrderEntity model)
        {
            _orderDbService.UpdateAsync(model);
        }

        public void InsertOrder(OrderEntity model)
        {
            model.PlacedDate= DateTime.Now;
            _orderDbService.AddAsync(model);
        }

        
    }
}
