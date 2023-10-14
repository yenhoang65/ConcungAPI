﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HuongDV.models
{
    [Index("Email", IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; } = "";

        [MaxLength(100)]
        public string LastName { get; set; } = "";

        [MaxLength(100)]
        public string Email { get; set; } = "";//Duy nhất trong cơ sở dữ liệu

        [MaxLength(20)]
        public string Phone { get; set; } = "";

        [MaxLength(100)]
        public string Address { get; set; } = "";

        [MaxLength(100)]
        public string Password { get; set; } = "";

        [MaxLength(20)]
        public string Role { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
