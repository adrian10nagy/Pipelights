using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pipelights.Database.Models;
using Pipelights.Website.BusinessService;
using Pipelights.Website.BusinessService.Models;
using Pipelights.Website.Models;
using System.Linq;

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
               
                var productsDtoLampi = _lampService.GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'lampi')", false);

                if (productsDtoLampi == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Categories = "Lampi";

                return View(productsDtoLampi);

            }
            else if(id == "oameni")
            {
               
                var productsDtoOameni = _lampService.GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'oameni')", false);

                if (productsDtoOameni == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Categories = "Omuleti";

                return View(productsDtoOameni);
            }
            else if (id == "leather")
            {
               
                var productsDtoCupru = _lampService.GetMultiple("SELECT * FROM c Where Array_Contains(c.Categories, 'leather')", false);

                if (productsDtoCupru == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Categories = "Pipe % Leather";

                return View(productsDtoCupru);
            }
            else if (id == "becuri")
            {
                var category = new CategoryEntity();
                var productsDtoBecuri = _lampService.GetMultiple("SELECT * FROM c Where c.Category = 'becuri')", false);

                if (productsDtoBecuri == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Categories = "Becuri";

                return View(productsDtoBecuri);
            }
            else
            {
                var productsDto = _lampService.GetMultiple("SELECT * FROM c", false);

                ViewBag.Categories = "Toate Produsele";

                return View(productsDto);
            }
        }

        [HttpPost]
        public IActionResult Search(SearchDto model)
        {
            var searchString = model.searchValue;

            if (searchString == null || searchString.Length <= 2)
            {
                return RedirectToAction("Index", "Products");
            }

            var query = "SELECT * FROM c WHERE LOWER(c.Name) LIKE '%" + searchString.ToLower() + "%'";

            var productsDtoSearched = _lampService.GetMultiple(query, false);

            if (productsDtoSearched == null || productsDtoSearched.Count() == 0)
            {
                ViewBag.Search = "Nu s-a gasit niciun rezultat";
            }
            else
            {
                ViewBag.Search = "Rezultatele cautarii";
            }

            return View(productsDtoSearched);
        }


        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Products");
            }

            var lamp = _lampService.GetById(id);

            TempData["Id"] = lamp.Id;

            var cat = lamp.CategoriesNew;
            var isOnStock = lamp.IsOnStock;

            if(cat != null && cat.Contains ("becuri"))
            {
                ViewBag.Becuri = "becuri";
            }

            if (isOnStock)
            {
                ViewBag.IsOnStock = "true";
            }

            if (lamp == null)
            {
                return RedirectToAction("Index", "Products");
            }


            return View(lamp);
        }

       
    }
}
