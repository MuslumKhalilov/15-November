using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testPronia.Models
{
    public class Slide
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(25)]
        public string Title { get; set; }
        [Required]
        [MaxLength(25)]
        public string SubTitle { get; set; }
        [Required]
        [MaxLength(25)]
        public string Description { get; set; }
        public string SlideImageUrl { get; set; }
        [Required]
        public int Order { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
