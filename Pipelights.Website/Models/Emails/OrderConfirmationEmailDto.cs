using Pipelights.Website.BusinessService.Models;

namespace Pipelights.Website.Models.Emails
{
    public class OrderConfirmationEmailDto
    {
        public string OrderNumber { get; set; }
        public Order Order { get; set; }
        public Cart Cart { get; set; }
        public string VoucherName { get; set; }
    }
}
