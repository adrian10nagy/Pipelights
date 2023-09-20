using Pipelights.Database.Models;
using Pipelights.Website.BusinessService.Models;
using System.Collections.Generic;

namespace Pipelights.Website.Models
{
    public class Cart
    {
        public string user;
        public Dictionary<string, int> products;
        public List<ProductDetailsForCart> ProductsForCart;
        public string address;
        public string telephone;
        public VoucherEntity voucher;
    }
}
