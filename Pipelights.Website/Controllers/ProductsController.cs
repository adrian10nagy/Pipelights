using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pipelights.Website.BusinessService;

namespace Pipelights.Website.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly ILampService _lampService;

        public ProductsController(ILogger<ProductsController> logger, ILampService lampService)
        {
            _logger = logger;
            _lampService = lampService;
        }

        public IActionResult Index()
        {
            var productsDto = _lampService.GetMultiple("SELECT * FROM c");


            return View(productsDto);
        }

        public IActionResult Details(string id)
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


            return View(lamp);
        }
    }
}
