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
        public List<int> TagIDs { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<int> SizeIDs { get; set; }
        public List<Size>? Sizes { get; set; }
        public List<int> ColorIDs { get; set; }
        public List<Color>? Colors { get; set; }
        public List<ProductImage> ProductImages { get; set; }
		public IFormFile? MainPhoto { get; set; }
		public IFormFile? HoverPhoto { get; set; }
		public List<IFormFile>? Photos { get; set; }
	}
}
