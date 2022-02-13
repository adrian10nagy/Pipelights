namespace Pipelights.Website.BusinessService.Models
{
    public class ProductDetailsForCart
    {
        public ProductDetailsDto ProductDetails { get; private set; }
        public int Quantity { get; set; }

        public ProductDetailsForCart(ProductDetailsDto product, int quantity)
        {
            ProductDetails = product;
            Quantity = quantity;
        }
    }
}
