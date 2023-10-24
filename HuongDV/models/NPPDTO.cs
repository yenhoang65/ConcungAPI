using System.ComponentModel.DataAnnotations;

namespace HuongDV.models
{
    public class NPPDTO
    {
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

    }
}
