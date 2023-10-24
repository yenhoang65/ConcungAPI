using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HuongDV.models
{
    public class NPP
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(100)]
        public string Brand { get; set; } = "";

        [MaxLength(100)]
        public string Phone { get; set; } = "";

        [MaxLength(100)]
        public string Email { get; set; } = "";

        [MaxLength(100)]
        public string Address { get; set; } = ""; 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
