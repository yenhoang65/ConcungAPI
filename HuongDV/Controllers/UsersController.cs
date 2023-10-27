using Azure;
using HuongDV.models;
using HuongDV.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HuongDV.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbcontext context;

        public UsersController(ApplicationDbcontext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetUser(int? page, string? Name,string? Phone, string? Email)
        {
            IQueryable<User> query = context.Users; 
            if(Name != null)
            {
                query = query.Where(p => p.FirstName.Contains(Name)
                || p.LastName.Contains(Name));
            }

            if (Phone != null)
            {
                query = query.Where(p => p.Phone == Phone);
            }

            if (Email != null)
            {
                query = query.Where(p => p.Email.Contains(Email));
            }


            if (page == null || page < 1)
            {
                page = 1;
            }

            int pageSize = 5;
            int totalPages = 0;

            decimal count = query.Count();
            totalPages = (int)Math.Ceiling(count / pageSize);

            var users = query
                .OrderByDescending(u => u.Id)
                .Skip((int)(page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            List<UserProfileDTO> userProfiles = new List<UserProfileDTO>();
            foreach (var user in users)
            {
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

                userProfiles.Add(userProfileDTO);
            }

            var response = new
            {
                Users = userProfiles,
                TotalPages = totalPages,
                PageSize = pageSize,
                Page = page
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
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

        [Authorize(Roles = "admin")]
        [HttpPut]
        public IActionResult UpdateUserRole(int users, string role)
        {
            var user = context.Users.Find(users);
            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật quyền của người dùng
            user.Role = role;
            context.SaveChanges();

            return Ok("Đã cập nhập quyền thành công");
        }
    }
}
