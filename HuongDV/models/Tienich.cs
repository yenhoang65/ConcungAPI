using System.ComponentModel.DataAnnotations;

namespace HuongDV.models
{
    public class Tienich
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(100)]
        public string ImageFileName { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
