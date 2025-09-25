using System.ComponentModel.DataAnnotations;

namespace ECRDemo.MiniAPI.Models
{
    public class ProductDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
