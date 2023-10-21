using System.ComponentModel.DataAnnotations;

namespace HuongDV.models
{
    public class TienichDTO
    {
        [Required,MaxLength(100)]
        public string Name { get; set; } = "";


        public IFormFile? ImageFileName { get; set; }

    }
}
