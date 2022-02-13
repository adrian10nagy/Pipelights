using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pipelights.Website.BusinessService;
using Pipelights.Website.Models;
using System.Diagnostics;
using System.Linq;

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
            var latestProducts = _lampsService.GetMultiple(6).ToList();

            return View(latestProducts);
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendMessage(EmailDto model)
        {
            _emailService.SendEmail("serox.pipelights@gmail.com",
                model.subject + "Primit de la [" + model.name + "], email: " + model.email, model.message);

            return RedirectToAction("Contact", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
