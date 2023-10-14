using HuongDV.models;
using HuongDV.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HuongDV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcountController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbcontext context;

        public AcountController(IConfiguration configuration, ApplicationDbcontext context)
        {
            this.configuration = configuration;
            this.context = context;
        }

        //[HttpGet("TestMaTB")]
        //public IActionResult TestMaTB()
        //{
        //    User user = new User() { Id = 2, Role = "admin" };
        //    string ma = CreateMaTB(user);
        //    var response = new {matb = ma};
        //    return Ok(response);
        //}

        [HttpPost("DangKy")]
        public IActionResult DangKy(UserDTO userDTO)
        {
            //kiểm tra xem địa chỉ email đã là người dùng hay chưa
            var emailCount = context.Users.Count(u => u.Email == userDTO.Email);
            if(emailCount > 0)
            {
                ModelState.AddModelError("Email", "Địa chỉ Email này đã được sử dụng");
                return BadRequest(ModelState);
            }
            //mã hóa mật khẩu
            var passwordHasher = new PasswordHasher<User>();
            var encryptedPassword = passwordHasher.HashPassword(new User(), userDTO.Password);

            //Tao tai khoan moi
            User user = new User()
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                Phone = userDTO.Phone ?? "",
                Address = userDTO.Address,
                Password = encryptedPassword,
                Role = "client",
                CreatedAt = DateTime.Now
            };
            context.Users.Add(user);
            context.SaveChanges();

            var ma = CreateMaTB(user);

            UserProfileDTO userProfileDTO = new UserProfileDTO()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = DateTime.Now
            };

            var response = new
            {
                MaTB = ma,
                User = userProfileDTO
            };
            return Ok(response);
        }

        [HttpPost("Login")]
        public IActionResult Login(string email, string password)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("Lỗi", "Email hoặc Mật khẩu không hợp lệ");
                return BadRequest(ModelState);
            }

            //xác minh mật khẩu

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(new User(), user.Password, password);
            if(result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("Mật khẩu", "Sai mật khẩu");
                return BadRequest(ModelState);
            }

            var ma = CreateMaTB(user);

            UserProfileDTO userProfileDTO = new UserProfileDTO()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = DateTime.Now
            };

            var response = new
            {
                MaTB = ma,
                User = userProfileDTO
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet("AuthorizeAuthenticatedUsers")]
        public IActionResult AuthorizeAuthenticatedUsers()
        {
            return Ok("Bạn được ủy quyền");
        }



        private string CreateMaTB(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("id", "" + user.Id),
                new Claim("role", user.Role)
            };

            string strkey = configuration["MaSettings:Key"]!;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(strkey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var matb = new JwtSecurityToken(
                issuer: configuration["MaSettings:Issuer"],
                audience: configuration["MaSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            var ma = new JwtSecurityTokenHandler().WriteToken(matb);

            return ma;
        }
    }
}
