using Microsoft.AspNetCore.Mvc;
using Pipelights.Database.Services;

namespace Pipelights.Website.Controllers
{
    public class OrderController : Controller
    {
        private IOrderDbService _iOrderDbService { get; }

        public OrderController(IOrderDbService iOrderDbService)
        {
            _iOrderDbService = iOrderDbService;
        }

        public IActionResult Confirmation(string orderNr)
        {
            var order = _iOrderDbService.GetAsync(orderNr).Result;

            return View(order);
        }
    }
}
