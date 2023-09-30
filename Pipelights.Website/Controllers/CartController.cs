using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pipelights.Website.BusinessService;
using Pipelights.Website.BusinessService.Models;
using Pipelights.Website.Models;
using Pipelights.Website.Models.Emails;
using RazorHtmlEmails.RazorClassLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pipelights.Website.Controllers
{
    public class CartController : Controller
    {
        private readonly ILampService _lampService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        private readonly IVoucherService _voucherService;

        public CartController(ILampService lampService, ICartService cartService, IOrderService orderService, 
            IEmailService emailService, IRazorViewToStringRenderer razorViewToStringRenderer, IVoucherService voucherService)
        {
            _lampService = lampService;
            _cartService = cartService;
            _orderService = orderService;
            _emailService = emailService;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _voucherService = voucherService;
        }

        [HttpPost]
        public IActionResult RemoveItem(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Cart");
            }

            var existingCart = HttpContext.Session.GetString("cart");
            if (string.IsNullOrEmpty(existingCart))
            {
                return RedirectToAction("Index", "Cart");
            }

            var cart = JsonConvert.DeserializeObject<Cart>(existingCart);
            if (cart.products.ContainsKey(id))
            {
                cart.products.Remove(id);
                var serializedCart = JsonConvert.SerializeObject(cart);
                HttpContext.Session.SetString("cart", serializedCart);
            }

            return RedirectToAction("Index", "Cart");
        }


        [HttpPost]
        public IActionResult GoToCheckout()
        {
            return RedirectToAction("Checkout", "Cart");
        }

        [HttpPost]
        public IActionResult EditQuantity(string id, int quantity)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Cart");
            }

            var existingCart = HttpContext.Session.GetString("cart");
            if (string.IsNullOrEmpty(existingCart))
            {
                return RedirectToAction("Index", "Cart");
            }

            var cart = JsonConvert.DeserializeObject<Cart>(existingCart);
            if (cart.products.ContainsKey(id))
            {
                cart.products[id] = quantity;
            }

            var serializedCart = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("cart", serializedCart);

            return RedirectToAction("Index", "Cart");

        }

        [HttpPost]
        public IActionResult AddProduct(string id, int quantity)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Products");
            }

            var lamp = _lampService.GetById(id);

            if (lamp == null)
            {
                return RedirectToAction("Index", "Products");
            }

            var existingCart = HttpContext.Session.GetString("cart");

            Cart cart;
            if (string.IsNullOrEmpty(existingCart))
            {
                cart = new Cart
                {
                    products = new Dictionary<string, int>()
                    {
                        { id , quantity }
                    }
                };
            }
            else
            {
                cart = JsonConvert.DeserializeObject<Cart>(existingCart);
                if (cart.products.ContainsKey(id))
                {
                    cart.products[id] = cart.products[id] + quantity;
                }
                else
                {
                    cart.products.Add(id, quantity);
                }
            }

            var serializedCart = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("cart", serializedCart);

            return RedirectToAction("Details", "Products", new { id = id });
        }


        public IActionResult Checkout()
        {
            var existingCart = HttpContext.Session.GetString("cart");

            var cart = _cartService.GetCartForSessionUser(existingCart);

            return View(cart);
        }

        public IActionResult Index()
        {
            var existingCart = HttpContext.Session.GetString("cart");

            var cart = _cartService.GetCartForSessionUser(existingCart);

            return View(cart);
        }

        [HttpPost]
        public IActionResult PlaceOrder(Order model)
        {
            // save to db
            var cartString = HttpContext.Session.GetString("cart");
            if (string.IsNullOrEmpty(cartString))
            {
                return RedirectToAction("Index", "Products");
            }
            
            var cart = _cartService.GetCartForSessionUser(cartString);

            var orderNr = _orderService.SaveOrder(model, cart);

            if (!string.IsNullOrWhiteSpace(orderNr))
            {
                // remove cart
                if (!string.IsNullOrEmpty(cartString))
                {
                    HttpContext.Session.Remove("cart");
                }

                var usedVoucher = cart.voucher?.Name;

                // send email
                string body = _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Shared/Emails/OrderConfirmation.cshtml",
                    new OrderConfirmationEmailDto
                    {
                        Order = model,
                        OrderNumber = orderNr,
                        Cart = cart,
                        VoucherName = usedVoucher,

                    }).Result;

                _emailService.SendEmail("serox.pipelights@gmail.com", $"A fost plasata o comanda! {orderNr}", body);

                // send email to client 
                string bodyForClient = _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Shared/Emails/OrderConfirmationForClient.cshtml",
                    new OrderConfirmationEmailDto
                    {
                        Order = model,
                        OrderNumber = orderNr,
                        Cart = cart,
                        VoucherName = usedVoucher,
                    }).Result;

                _emailService.SendEmail($"{model.email}", $"Salut. Ai plasat comanda {orderNr}", bodyForClient);

                if (cart?.voucher?.id != null && cart.voucher.isSingleUse)
                {

                    var voucher = _voucherService.GetById(cart?.voucher?.id);
                    voucher.isUsed = true;
                    _voucherService.UpdateAsync(voucher.id, voucher);
                }
            }
            else
            {
                _emailService.SendEmail("serox.pipelights@gmail.com", $"Eroare la salvarea unei comenzi! {orderNr}",
                    $"Comandat cu numarul {orderNr} a fost plasata de catre {model.name} {model.surname}.   <br><br>     Detalii: Nume: {model.name} {model.surname}, email {model.email}, telefon {model.telephone}." +
                    $"Adresa, Judet: {model.judete}, Localitate {model.localitate}. Plata aleasa: {model.payment}");
            }

            // redirect to TY page
            return RedirectToAction("Confirmation", "Order", new { orderNr = orderNr });
        }

        [HttpPost]
        public IActionResult PriceWithDiscount([FromBody] VoucherDto model)
        {
            var price = model.Price;

            if (model.VoucherString != string.Empty)
            {
                var discount = _voucherService.VouckerCheck(model.VoucherString);

                var discountedPrice = price * discount;

                price = discountedPrice;

                var cartString = HttpContext.Session.GetString("cart");
                if (!string.IsNullOrEmpty(cartString))
                {
                    Cart cart = _cartService.GetCartForSessionUser(cartString);
                    cart.voucher = _voucherService.GetById(model.VoucherString);

                    var serializedCart = JsonConvert.SerializeObject(cart);
                    HttpContext.Session.SetString("cart", serializedCart);

                }
            }

            return Json(price);

        }
    }
}