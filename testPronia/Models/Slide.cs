using System.ComponentModel.DataAnnotations.Schema;

namespace testPronia.Models
{
    public class Slide
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string SlideImageUrl { get; set; }
        public int Order { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
