using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalog.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public string CreatedByUserId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        [Range(1, 365)]
        public int DurationInDays { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public string ImagePath { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        //  public DateTime EndDate => StartDate.AddDays(DurationInDays);
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedByUserId { get; set; }

    }
}
