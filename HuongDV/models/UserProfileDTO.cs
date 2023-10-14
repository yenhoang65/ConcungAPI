using System.ComponentModel.DataAnnotations;

namespace HuongDV.models
{
    public class UserProfileDTO
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string Email { get; set; } = "";//Duy nhất trong cơ sở dữ liệu

        public string Phone { get; set; } = "";

        public string Address { get; set; } = "";

        public string Role { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
