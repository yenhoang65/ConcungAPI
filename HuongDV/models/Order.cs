using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HuongDV.models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId {  get; set; }
        public DateTime CreatedDate { get; set; }
        
        [Precision(16,2)]
        public decimal Ship {  get; set; }

        [MaxLength(100)]
        public string DiaChiGH { get; set; } = "";

        [MaxLength(30)]
        public string PTThanhToan { get; set; } = "";

        [MaxLength(30)]
        public string TinhTrangTT { get; set; } = "";

        [MaxLength(30)]
        public string TTDatHang { get; set; } = "";

        // navvigation properties
        public User User { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
