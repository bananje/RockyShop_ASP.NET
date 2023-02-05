using System.ComponentModel.DataAnnotations;

namespace Rocky_Models
{
    public class ApplicationType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
        [Required]
        public string Image { get; set; }
    }
}
