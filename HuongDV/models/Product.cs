using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HuongDV.models
{
    public class Product
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(100)]
        public string Brand { get; set; } = "";

        [MaxLength(100)]
        public string DanhMuc { get; set; } = "";

        [Precision(16,2)]
        public decimal Gia { get; set; }

        public string MoTa { get; set; } = "";

        [MaxLength (100)]
        public string AnhSP { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
