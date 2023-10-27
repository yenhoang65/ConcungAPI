using System.ComponentModel.DataAnnotations;

namespace HuongDV.models
{
    public class OrderDTO
    {
        [Required]
        public string LayChuoiSP { get; set; } = "";

        [Required,MinLength(30),MaxLength(100)]
        public string DiachiGH { get; set; } = "";
        
        [Required]
        public string PTThanhToan { get; set; } = "";
    }
}
