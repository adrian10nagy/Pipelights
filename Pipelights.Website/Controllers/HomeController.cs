using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pipelights.Website.BusinessService;
using Pipelights.Website.BusinessService.Models;
using Pipelights.Website.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Pipelights.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILampService _lampsService;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, ILampService lampsService, IEmailService emailService)
        {
            _logger = logger;
            _lampsService = lampsService;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            var latestProducts = _lampsService.GetMultiple(false, 4).ToList();

            return View(latestProducts);
        }


        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("emailSended")))
            {
                ViewBag.ShowModalScript = "showModal();";
                HttpContext.Session.Remove("emailSended");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public bool sendTheMessage(EmailDto model)
        {
            var emailSent = false;
            if (model != null && model.email != null && model.subject != null && model.message != null)
            {
              emailSent = _emailService.SendEmail("serox.pipelights@gmail.com",
                  model.subject + "Primit de la [" + model.name + "], email: " + model.email, model.message);

            }

            return emailSent;
        }

        [HttpPost]
        public IActionResult SendMessage(EmailDto model)
        {
            var emailSent = sendTheMessage(model);

            if (emailSent)
            {
                HttpContext.Session.SetString("emailSended", "Emailul a fost trimis cu succes! Iti vom raspunde in cel mai scurt timp.");
            }

            return RedirectToAction("Contact", "Home");
        }

        public IActionResult Suggestions()
        {
            string myId = TempData["Id"].ToString();
            List<ProductDetailsDto> list = new List<ProductDetailsDto>();

            var latestProducts = _lampsService.GetSuggestions(false, 100);

            foreach (var product in latestProducts)
            {
                string p = product.Id.ToString();
                if (myId != p)
                {
                    list.Add(product);
                }
            }
            var displayedProducts = list.OrderBy(x => Guid.NewGuid()).Take(4).ToList();

            return PartialView("_SuggestionsPartial", displayedProducts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
