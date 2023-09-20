using Newtonsoft.Json;
using Pipelights.Website.BusinessService.Models;
using Pipelights.Website.Models;
using System.Collections.Generic;
using System.Linq;

namespace Pipelights.Website.BusinessService
{
    public interface ICartService
    {
        Cart GetCartForSessionUser(string existingCart);
    }

    public class CartService: ICartService
    {
        private ILampService _lampService { get; }

        public CartService(ILampService lampService)
        {
            _lampService = lampService;
        }

        public Cart GetCartForSessionUser(string existingCart)
        {
            Cart cart = null;
            if (!string.IsNullOrEmpty(existingCart))
            {
                cart = JsonConvert.DeserializeObject<Cart>(existingCart);
                if (cart != null)
                {
                    cart.ProductsForCart = new List<ProductDetailsForCart>();
                }

                if (cart?.products?.Any() != null)
                {
                    foreach (var cartProduct in cart.products)
                    {
                        var lamp = _lampService.GetById(cartProduct.Key);
                        if (lamp != null)
                        {
                            cart.ProductsForCart.Add(new ProductDetailsForCart(lamp, cartProduct.Value));
                        }
                    }
                }
            }

            return cart;
        }
    }
}
