using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Pipelights.Database.Models;
using Pipelights.Website.Services.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pipelights.Website.BusinessService.Models
{
    public class ProductDetailsDto
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IBlobService _blobService;
        
        public ProductDetailsDto()
        {
        }

        public ProductDetailsDto(LampEntity lamp, IWebHostEnvironment webHostEnvironment, IBlobService blobService)
        {
            _webHostEnvironment = webHostEnvironment;
            _blobService = blobService;
            Id = lamp.id;
            Name = lamp.Name;
            Description = lamp.Description;
            Price = lamp.Price;
            PriceReduced = lamp.PriceReduced;
            Resources = GetImagesFromRoot(lamp.id).OrderByDescending(x => x.Contains("1.")).ToList();
            if (!Resources.Any())
            {
                Resources = new List<string> {"/lib/from-template/images/product-no-image.jpg"};
            }
            else
            {
                HasVideo = Resources.Any(x => x.Contains(".mov") || x.Contains(".mp4"));
            }

            MainImage = Resources.FirstOrDefault(x => x.Contains("1.")) ?? Resources.FirstOrDefault();

            
        }

        public List<string> Resources { get; set; }
        public string Id { get; set; }
        public bool HasVideo { get; set; }
        public string MainImage { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string PriceReduced { get; set; }
        public string VideoUrl { get; set; }

        public bool HasPricedReduced => string.IsNullOrEmpty(PriceReduced);

        private List<string> GetImagesFromRoot(string lampEntityId)
        {
            var images = _blobService.GetAllResourcesFromFolder(lampEntityId.ToLower());

            //var provider = new PhysicalFileProvider(_webHostEnvironment.WebRootPath);
            //var contents = provider.GetDirectoryContents(Path.Combine("lib", "images", "UploadedFiles", lampEntityId));
            //var objFiles = contents.OrderBy(m => m.LastModified);
            //var ImageList = new List<string>();
            //foreach (var item in objFiles.ToList())
            //{
            //    ImageList.Add(item.Name);
            //}

            return images;
        }
    }
}
