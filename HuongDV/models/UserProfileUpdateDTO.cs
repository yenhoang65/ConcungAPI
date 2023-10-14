using System.ComponentModel.DataAnnotations;

namespace HuongDV.models
{
    public class UserProfileUpdateDTO
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = "";

        [Required, MaxLength(100)]
        public string LastName { get; set; } = "";

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = "";//Duy nhất trong cơ sở dữ liệu

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required, MaxLength(100)]
        public string Address { get; set; } = "";
    }
}
