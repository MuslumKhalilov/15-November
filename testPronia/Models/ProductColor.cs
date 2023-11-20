namespace testPronia.Models
{
    public class ProductColor
    {
        public int id { get; set; }
        public int ProductId { get; set; }
        public int ColorId { get; set; }

        public Product Product { get; set; }
        public Color Color { get; set; }
    }
}
