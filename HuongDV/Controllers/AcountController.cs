using HuongDV.models;
using HuongDV.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
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


        //[Authorize]
        //[HttpGet("GetTokenClaims")]
        //public IActionResult GetTokenClaims()
        //{
        //    var identity = User.Identity as ClaimsIdentity;
        //    if (identity != null)
        //    {
        //        Dictionary<string, string> claims = new Dictionary<string, string>();

        //        foreach (Claim claim in identity.Claims)
        //        {
        //            claims.Add(claim.Type, claim.Value);
        //        }

        //        return Ok(claims);
        //    }

        //    return Ok();
        //}


        [Authorize]
        [HttpGet("Profile")]
        public IActionResult GetProfile()
        {
            int id = JwtReader.GetUserId(User);

            var user = context.Users.Find(id);
            if (user == null)
            {
                return Unauthorized();
            }

            var userProfileDTO = new UserProfileDTO()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            return Ok(userProfileDTO);
        }

        [Authorize]
        [HttpPut("UpdateProfile")]
        public IActionResult UpdateProfile(UserProfileUpdateDTO userProfileUpdateDTO)
        {
            int id = JwtReader.GetUserId(User);
            var user = context.Users.Find(id);
            if (user == null)
            {
                return Unauthorized();
            }

            //update the user profile
            user.FirstName = userProfileUpdateDTO.FirstName;
            user.LastName = userProfileUpdateDTO.LastName;
            user.Email = userProfileUpdateDTO.Email;
            user.Phone = userProfileUpdateDTO.Phone ?? "";
            user.Address = userProfileUpdateDTO.Address;

            context.SaveChanges();


            var userProfileDTO = new UserProfileDTO()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            return Ok(userProfileDTO);
        }

        [Authorize]
        [HttpPut("UpdatePassword")]
        public IActionResult UpdatePassword([Required, MinLength(8),MaxLength(100)]string password)
        {
            int id = JwtReader.GetUserId(User);

            var user = context.Users.Find(id);
            if(user == null)
            {
                return Unauthorized();
            }

            // encrypt password
            var passwordHasher = new PasswordHasher<User>();
            string encryptedPassword = passwordHasher.HashPassword(new User(), password);


            // update the user password
            user.Password = encryptedPassword;

            context.SaveChanges();

            return Ok();
        }


  
       



        //[Authorize]
        //[HttpGet("AuthorizeAuthenticatedUsers")]
        //public IActionResult AuthorizeAuthenticatedUsers()
        //{
        //    return Ok("Bạn được ủy quyền");
        //}

        //[Authorize(Roles ="admin")]
        //[HttpGet("AuthorizeAdmin")]
        //public IActionResult AuthorizeAdmin()
        //{
        //    return Ok("Bạn được ủy quyền");
        //}

        //[Authorize(Roles ="admin, seller")]
        //[HttpGet("AuthorizeAdminAndseller")]
        //public IActionResult AuthorizeAdminAndSeller()
        //{
        //    return Ok("Bạn được ủy quyền");
        //}

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
