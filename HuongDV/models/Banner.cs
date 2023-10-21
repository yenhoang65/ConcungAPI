using System.ComponentModel.DataAnnotations;

namespace HuongDV.models
{
    public class Banner
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string AnhBanner { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
