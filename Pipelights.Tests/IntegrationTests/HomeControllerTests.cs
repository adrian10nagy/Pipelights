using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pipelights.Database.Models;
using Pipelights.Database.Services;
using Pipelights.Website.BusinessService;
using Pipelights.Website.BusinessService.Models;
using Pipelights.Website.Controllers;
using Pipelights.Website.Models;
using Pipelights.Website.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Pipelights.Tests.IntegrationTests
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void AboutAction_ReturnsView()
        {
            //arrange
            var homeController = new HomeController(null, new LampService(null, null), new EmailService());

            //act
            var result = homeController.About();

            //assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SendMessage_ValidContent_SendsTheMessage()
        {
            //arrange
            var homeController = new HomeController(null, new LampService(null, null), new EmailService());
            var emailDtoObject = new EmailDto()
            {
                name = "Adisor",
                email = "s3rgiumoldova@yahoo.com",
                message = "salutMessage",
                subject = "nimic subject"
            };

            //act
            var sendMessage = homeController.sendTheMessage(emailDtoObject);

            //assert
            Assert.IsTrue(sendMessage);
        }

        [TestMethod]
        public void SendMessage_NullContent_DoesntSendEmail()
        {
            //arrange
            var homeController = new HomeController(null, new LampService(null, null), new EmailService());
            EmailDto email = null;

            //act
            var sendEmail = homeController.sendTheMessage(email);

            //assert
            Assert.IsFalse(sendEmail);
        }

        [TestMethod]
        public void SendMessage_NullSubject_DoesntSendEmail()
        {
            //arrange
            var homeController = new HomeController(null, new LampService(null, null), new EmailService());
            EmailDto email = new EmailDto();

            //act
            var sendEmail = homeController.sendTheMessage(email);

            //assert
            Assert.IsFalse(sendEmail);
        }

      
    }

}
