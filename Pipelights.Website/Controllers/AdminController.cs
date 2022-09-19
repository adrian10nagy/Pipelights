using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pipelights.Database.Models;
using Pipelights.Website.BusinessService;
using Pipelights.Website.BusinessService.Models;
using Pipelights.Website.Services.Interfaces;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Pipelights.Website.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILampService _lampService;
        private readonly ICategoryService _categoryService;
        private readonly IBlobService _blobService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILampService lampService, ILogger<AdminController> logger, IBlobService blobService, ICategoryService categoryService)
        {
            _lampService = lampService;
            _logger = logger;
            _blobService = blobService;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public IActionResult Login(string username, string password)
        {
            if (username.Equals("parola") && password.Equals("admin"))
            {
                HttpContext.Session.SetString("username", "Admin");
            }

            return RedirectToAction("Dashboard", "Admin");
        }

        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return View(null);
            }
            var productDetailsDto = _lampService.GetById(id);

            return View(productDetailsDto);
        }

        public IActionResult RemovePhoto(string url, string id)
        {
            var x =
            _blobService.RemoveFile(url, "lamps", _logger).Result;

            return RedirectToAction("Edit", "Admin", new { id = id });
        }


        [HttpPost]
        public IActionResult Edit(ProductDetailsDto model)
        {
            var isNew = false;
            if (string.IsNullOrEmpty(model.Id))
            {
                isNew = true;
                model.Id = model.Name.Replace(" ", string.Empty);
            }

            List<int> ints = new List<int>();

            model.Categories.Split(',').ToList().ForEach(x => ints.Add(int.Parse(x)));

            var dbModel = new LampEntity
            {
                id = model.Id,
                Name = model.Name,
                Description = model.Description,
                TechnicalData = model.TechnicalData,
                Price = model.Price,
                PriceReduced = model.PriceReduced,
                IsInactive = model.IsInactive,
                Categories = ints,
            };


            if (isNew)
            {
                var result = _lampService.AddAsync(dbModel);
            }
            else
            {
                var result = _lampService.UpdateAsync(dbModel.id, dbModel);
            }

            return RedirectToAction("Edit", "Admin", new { id = model.Id });
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult ProductsDashboard()
        {
            IEnumerable<ProductDetailsDto> productsDto = _lampService.GetMultiple("SELECT * FROM c", true).OrderBy(l => l.IsInactive);

            return View(productsDto);
        }

        public IActionResult CategoriesDashboard()
        {
            IEnumerable<CategoryEntity> productsDto = _categoryService.GetMultiple("SELECT * FROM c");

            return View(productsDto);
        }

        [HttpPost]
        public IActionResult UploadImage(List<IFormFile> postedFiles, string id)
        {

            List<string> uploadedFiles = new List<string>();
            foreach (IFormFile postedFile in postedFiles)
            {
                var stream = postedFile.OpenReadStream();
                var result = _blobService.SaveFileToBlobStorage(stream, $"{id.ToLower()}/{postedFile.FileName}", "lamps", _logger).Result;
            }

            return RedirectToAction("Edit", "Admin", new { id = id });
        }


    }
}
