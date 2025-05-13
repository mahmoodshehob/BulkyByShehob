using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkySh.Models.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Category Name")]
        [MaxLength(20, ErrorMessage = "The Category Name Very Long")]
        public string? Name { get; set; }

        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1 to 100")]
        public int DisplayOrder { get; set; }

        //public DateTime MyProperty { get; set; } = DateTime.Now;
    }
}
