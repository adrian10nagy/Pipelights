using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pipelights.Database.Models;
using Pipelights.Website.BusinessService.Models;
using Pipelights.Website.BusinessService;
using Pipelights.Website.Controllers;
using Pipelights.Website.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pipelights.Tests.UnitTests
{
    public class HomeControllerIntegrationTests
    {
        [TestMethod]
        public void SendMessage_ValidContentWithMockedSending_ReturnsSuccessSendTheMessage()
        {
            //arrange
            var mockedEmailService = new Mock<IEmailService>();
            mockedEmailService.Setup(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var homeController = new HomeController(null, new LampService(null, null), mockedEmailService.Object);
            var emailDtoObject = new EmailDto()
            {
                name = "Adisor",
                email = "s3rgiumoldova@yahoo.com",
                message = "salutMessage",
                subject = "nimic subject"
            };
            //act
            var sendEmail = homeController.sendTheMessage(emailDtoObject);

            //assert
            Assert.IsTrue(sendEmail);
        }



       
    }
}
