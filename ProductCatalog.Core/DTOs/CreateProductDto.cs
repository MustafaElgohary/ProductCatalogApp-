using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Core.DTOs
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public int DurationInDays { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string ImagePath { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
