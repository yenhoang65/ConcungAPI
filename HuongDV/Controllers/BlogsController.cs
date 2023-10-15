using HuongDV.models;
using HuongDV.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;

namespace HuongDV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly ApplicationDbcontext context;
        private readonly IWebHostEnvironment enviroment;

        public BlogsController(ApplicationDbcontext context, IWebHostEnvironment enviroment)
        {
            this.context = context;
            this.enviroment = enviroment;
        }


        [HttpGet]
        public IActionResult GetBlogs(int? page)
        {
            if (page == null || page < 1)
            {
                page = 1;
            }

            int pageSize = 10;
            int totalPages = 0; //hiển thị các nút phân trang

            decimal countContacts = context.Blogs.Count();
            totalPages = (int)Math.Ceiling(countContacts / pageSize); //Số trang 

            var blogs = context.Blogs
                .OrderByDescending(b => b.Id) //Sắp xếp bài viết mới nhất - cũ nhất
                .Skip((int)(page - 1) * pageSize)//loại bỏ bài viết nhất định để gọi đến trang được yêu cầu
                .Take(pageSize) //Lấy bài viết tại trang được yêu cầu
                .ToList();

            var returns = new
            {
                Contacts = blogs,
                TotalPages = totalPages,
                Page = page,
                PageSize = pageSize
            };

            return Ok(returns);

        }

        [HttpGet("id")]
        public IActionResult GetBlogs(int id)
        {
            var blog = context.Blogs.Find(id);
            if (blog == null)
            {
                return NotFound();
            }
            return Ok(blog);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult CreateBlog([FromForm] BlogDTO blogDTO)
        {
            //Vì file hình ảnh là tùy chọn => check image: null or not null
            if (blogDTO.ImageFileName == null)
            {
                ModelState.AddModelError("Tệp hình ảnh", "Tệp hình ảnh là bắt buộc!");
                return BadRequest(ModelState);
            }

            //save the image on the server
            string imageFile = DateTime.Now.ToString("yyyyMMddHHmmssfff"); //Tên hình ảnh bằng năm ngày giờ.. hiện tại
            imageFile += Path.GetExtension(blogDTO.ImageFileName.FileName); //lấy đuôi mở rộng của hình ảnh trong productsDTO

            string iFolder = enviroment.WebRootPath + "/images/blogsImage/";

            using (var FolderNew = System.IO.File.Create(iFolder + imageFile))
            {
                blogDTO.ImageFileName.CopyTo(FolderNew);
            }

            //đọc thông tin người dùng từ Jwt
            int userId = JwtReader.GetUserId(User);
            var user = context.Users.Find(userId);

            //save blog in the database
            Blog blog = new Blog()
            {
                UserID = userId,
                Title = blogDTO.Title,
                Content = blogDTO.Content,
                ImageFileName = imageFile,
                CreatedAt = DateTime.Now
            };

            context.Blogs.Add(blog);
            context.SaveChanges();

            return Ok(blog);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("id")]
        public IActionResult UpdateBlog(int id, [FromForm] BlogDTO blogDTO)
        {
            var blog = context.Blogs.Find(id);

            if (blog == null)
            {
                return NotFound();
            }

            string imageFileName = blog.ImageFileName; //đọc tên tệp hình ảnh sản phẩm lấy trong csdl
            if (blogDTO.ImageFileName != null) //kiểm tra có tên tệp mới trong productDTO không -> khác rỗng -> có thêm 1 h/a mới
            {
                // save the image on the server
                imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff"); //Tên hình ảnh bằng năm ngày giờ.. hiện tại
                imageFileName += Path.GetExtension(blogDTO.ImageFileName.FileName); //lấy đuôi mở rộng của hình ảnh trong productsDTO

                string iFolder = enviroment.WebRootPath + "/images/blogsImage/";

                using (var FolderNew = System.IO.File.Create(iFolder + imageFileName))
                {
                    blogDTO.ImageFileName.CopyTo(FolderNew);
                }

                // delete the old image
                System.IO.File.Delete(iFolder + blog.ImageFileName); //cung cấp đường dẫn đầy đủ đến hình ảnh cũ
            }

            //update the product in the database
            blog.Title = blogDTO.Title;
            blog.Content = blogDTO.Content;
            blog.ImageFileName = imageFileName;

            context.SaveChanges();

            return Ok(blog);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("id")]
        public IActionResult DeleteBlog(int id)
        {
            var blog = context.Blogs.Find(id);

            if (blog == null)
            {
                return NotFound();
            }

            //delete the image on the server
            string iFolder = enviroment.WebRootPath + "/images/blogsImage/";
            System.IO.File.Delete(iFolder + blog.ImageFileName);

            //delete the product in the database
            context.Blogs.Remove(blog);
            context.SaveChanges();

            return Ok();
        }
    }
}
