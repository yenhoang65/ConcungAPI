using System.ComponentModel.DataAnnotations;

namespace HuongDV.models
{
    public class ContactDTO
    {
        [MaxLength(100)]
        public string FirstName { get; set; } = "";

        [MaxLength(100)]
        public string LastName { get; set; } = "";

        [MaxLength(100)]
        public string Email { get; set; } = "";

        [MaxLength(100)]
        public string Phone { get; set; } = "";

        public int SubjectId { get; set; }

        [Required, MinLength(5), MaxLength(4000)]
        public string Message { get; set; } = "";
    }
}
