using System.ComponentModel.DataAnnotations;

namespace testPronia.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        public List<ProductTag>? ProductTags { get; set; }
    }
}
