namespace Rocky_Models.ViewModels
{
    public class ProductUserVM
    {
        public ProductUserVM()
        {
            ProductList= new List<Product>();
        }

        public AppUser ApplicationUser { get; set; }
        public IList<Product> ProductList { get; set; }
    }
}
