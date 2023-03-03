using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pipelights.Database.Models;
using Pipelights.Website.BusinessService.Models;
using Pipelights.Website.BusinessService;
using System;
using System.Collections.Generic;
using System.Text;
using Pipelights.Database.Services;
using System.Threading.Tasks;
using Pipelights.Website.Services.Interfaces;
using System.Linq;

namespace Pipelights.Tests.UnitTests
{
    [TestClass]
    public class LampServiceUnitTests
    {
        [TestMethod]
        public void GetByID_ValidId_ReturnsNotNull()
        {
            //arrange
             var lamp = new LampEntity
            {
                id = "1",
                Name = "pipeman",
                Category = "lamps",
                Description= "beautiful",
                Price = "100",
                PriceReduced= "90",
                IsInactive= false,
                IsOnStock= true,
                TechnicalData= "100cm"
            };

            var mockedDbLampService = new Mock<ILampDbService>();
            mockedDbLampService.Setup(x => x.GetAsync("1")).Returns(Task.FromResult(lamp));

            var blobService = new Mock<IBlobService>();
            blobService.Setup(x => x.GetAllResourcesFromFolder("1")).Returns(new List<string>());

            var lampService = new LampService(mockedDbLampService.Object, blobService.Object);
          
            //act

            var result = lampService.GetById("1");

            //assert

            Assert.IsNotNull(result);
            Assert.AreEqual(lamp.Name, result.Name);
            Assert.AreEqual(lamp.id, result.Id);
            Assert.AreEqual(lamp.Category, result.Categories);
            Assert.AreEqual(lamp.Description, result.Description);
            Assert.AreEqual(lamp.Price, result.Price);
            Assert.AreEqual(lamp.TechnicalData, result.TechnicalData);
            Assert.AreEqual(lamp.PriceReduced, result.PriceReduced);
            Assert.AreEqual(lamp.IsOnStock, result.IsOnStock);
            Assert.AreEqual(lamp.IsInactive, result.IsInactive);

        }

        [TestMethod]
        public void AddAsync_ValidContent_ReturnsTrue()
        {
            //arrange
            var lamp = new LampEntity();

            var mockedLampDbService = new Mock<ILampDbService>();
            mockedLampDbService.Setup(x => x.AddAsync(lamp));

            var lampService = new LampService(mockedLampDbService.Object, null);

            //act
            var result = lampService.AddAsync(lamp);

            //assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddAsync_InValidContent_ReturnsFalse()
        {
            //arrange
            var lamp = new LampEntity();

            var mockedLampDbService = new Mock<ILampDbService>();
            mockedLampDbService.Setup(x => x.AddAsync(lamp)).Throws(new Exception());

            var lampService = new LampService(mockedLampDbService.Object, null);

            //act
            var result = lampService.AddAsync(lamp);

            //assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetMultiple_NoResults_ReturnsEmptyList()
        {
            //arrange

            var mockedLampDbService = new Mock<ILampDbService>();
            mockedLampDbService.Setup(x => x.GetMultipleAsync("")).Returns(Task.FromResult(Enumerable.Empty<LampEntity>()));

            //var blobService = new Mock<IBlobService>();
            //blobService.Setup(x => x.GetAllResourcesFromFolder(lamp.id)).Returns(new List<string>());

            var lampService = new LampService(mockedLampDbService.Object, null);

            //act
            var result = lampService.GetMultiple("", true);

            //assert

            Assert.AreEqual(result.ToList().Count, 0);
                
        }

        [TestMethod]
        public void GetMultiple_OneResultFromDb_ReturnsOneObject()
        {
            //arrange
            var list = new List<LampEntity>();
            var lamp = new LampEntity
            {
                id = "1",
                Name = "pipeman",
                Category = "lamps",
                Description = "beautiful",
                Price = "100",
                PriceReduced = "90",
                IsInactive = false,
                IsOnStock = true,
                TechnicalData = "100cm"
            };

            list.Add(lamp);
            var e = list.AsEnumerable();

            var mockedLampDbService = new Mock<ILampDbService>();
            mockedLampDbService.Setup(x => x.GetMultipleAsync("")).Returns(Task.FromResult(e));

            var blobService = new Mock<IBlobService>();
            blobService.Setup(x => x.GetAllResourcesFromFolder(It.IsAny<string>())).Returns(new List<string>());

            var lampService = new LampService(mockedLampDbService.Object, blobService.Object);

            //act
            var result = lampService.GetMultiple("", true);

            //assert

            Assert.AreEqual(result.ToList().Count, 1);

        }

        //update
    }
}
