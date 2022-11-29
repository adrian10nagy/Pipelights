using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pipelights.Database.Models;
using Pipelights.Website.BusinessService;
using Pipelights.Website.BusinessService.Models;

namespace Pipelights.Website.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly ILampService _lampService;
        private readonly ICategoryService _categoryService;
        public ProductsController(ILogger<ProductsController> logger, ILampService lampService)
        {
            _logger = logger;
            _lampService = lampService;
        }

        public IActionResult Index(string id)
        {
            

            if (id == "lampi")
            {
               
                var productsDtoLampi = _lampService.GetMultiple("SELECT * FROM c Where c.Category = 'lampi'", false);

                if (productsDtoLampi == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Categories = "Lampi";

                return View(productsDtoLampi);

            }
            else if(id == "oameni")
            {
                var category = new CategoryEntity();
                var productsDtoOameni = _lampService.GetMultiple("SELECT * FROM c Where c.Category = 'oameni'", false);

                if (productsDtoOameni == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Categories = "Omuleti";

                return View(productsDtoOameni);
            }
            else if (id == "cupru")
            {
                var category = new CategoryEntity();
                var productsDtoCupru = _lampService.GetMultiple("SELECT * FROM c Where c.Category = 'cupru'", false);

                if (productsDtoCupru == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Categories = "Cupru";

                return View(productsDtoCupru);
            }
            else if (id == "becuri")
            {
                var category = new CategoryEntity();
                var productsDtoCupru = _lampService.GetMultiple("SELECT * FROM c Where c.Category = 'becuri'", false);

                if (productsDtoCupru == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Categories = "Becuri";

                return View(productsDtoCupru);
            }
            else
            {
                var productsDto = _lampService.GetMultiple("SELECT * FROM c", false);

                ViewBag.Categories = "Toate Produsele";

                return View(productsDto);
            }
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
