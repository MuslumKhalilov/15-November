using testPronia.Models;

namespace testPronia.Areas.ViewModels
{
    public class UpdateProductVM
    {
       
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public int? CategoryId { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
