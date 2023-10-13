using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HuongDV.models
{
    public class ProductDTO
    {
        [Required ,MaxLength(100)]
        public string Name { get; set; } = "";

        [Required, MaxLength(100)]
        public string Brand { get; set; } = "";

        [Required, MaxLength(100)]
        public string DanhMuc { get; set; } = "";

        [Required]
        public decimal Gia { get; set; }

        [MaxLength(2000)]
        public string? MoTa { get; set; }

        public IFormFile? AnhSP { get; set; }

    }
}
