using HuongDV.models;
using HuongDV.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace HuongDV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbcontext context;
        private readonly IWebHostEnvironment env;

        private readonly List<string> listDanhMuc = new List<string>()
        {
            // khởi tạo danh sách danh mục
            "Sữa","Bỉm","Tã","Quần Áo","Khác"
        };

        public ProductsController(ApplicationDbcontext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;
        }

        [HttpGet("DanhMuc")]
        public IActionResult GetDanhMuc()
        {
            return Ok(listDanhMuc);
        }


        [HttpGet]
        public IActionResult GetProducts(string? Name, string? DanhMuc,
                int? minGia, int? maxGia,
                int? page)// khai báo phương thức để tìm kiếm, lọc các sản phẩm
        {
            IQueryable<Product> query = context.products;// tạo một truy vấn sử dụng Iq..cho phép mở rộng truy vấn trên các đk tìm kiếm

            // chức năng tìm kiếm
            if(Name != null) 
            {
                query = query.Where(p => p.Name.Contains(Name) || p.MoTa.Contains(Name));// kiểm tra name hoặc mô tả có chứa trường name hay kh 
            }

            if(DanhMuc != null) 
            {
                query = query.Where(p => p.DanhMuc ==  DanhMuc);
            }

            if(minGia != null) 
            {
                query = query.Where(p => p.Gia >= minGia);
            }

            if (maxGia != null)
            {
                query = query.Where(p => p.Gia <= maxGia);
            }


            // pagination functionality...chức năng phân trang
            if(page == null || page < 1) page =1;//kiểm tra page xem có phải null hoặc nhỏ hơn 1 hay kh nếu đúng thì đc thiết lập page bằng 1
            int pagesize = 5;
            int totalPage = 0;

            decimal count = query.Count();
            totalPage = (int)Math.Ceiling(count / pagesize);

            query = query.Skip((int) (page - 1) * pagesize).Take(pagesize);

            var products = query.ToList();// chuyển đổi truy vấn linq thành các danh sách sản phẩm 

            var response = new
            {
                Products = products,
                TotalPages = totalPage,
                PageSize = pagesize,
                Page = page
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = context.products.Find(id);

            if(product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("Top 10")]
        public IActionResult GetTopProducts()
        {
            var top = context.OrderItems
            .GroupBy(o => o.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalQuantity = g.Sum(o => o.ProductId)
            })
            .OrderByDescending(p => p.TotalQuantity)
            .Take(10)
            .Join(
                context.products,
                orderItem => orderItem.ProductId,
                product => product.Id,
                (orderItem, product) => new
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductPrice = product.Gia,
                }
            )
            .ToList();
            return Ok(top);
        }


        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult CrateProduct([FromForm] ProductDTO productDTO)
        {
            if (!listDanhMuc.Contains(productDTO.DanhMuc))// kiểm tra thông tin đầu vào
            {
                ModelState.AddModelError("Danh Mục", "Vui lòng chọn danh mục hợp lệ");
                return BadRequest(ModelState);
            }

            if (productDTO.AnhSP == null)
            {
                ModelState.AddModelError("Tập tin hình ảnh", "Cần thiết");
                return BadRequest(ModelState);
            }

            // lưu hình ảnh trên máy chủ
            
            string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            imageFileName += Path.GetExtension(productDTO.AnhSP.FileName);

            string imagesFolder = env.WebRootPath + "/images/products/";

            using (var stream = System.IO.File.Create(imagesFolder + imageFileName))//File.Create() để tạo hoặc mở tệp hình ảnh mới để ghi dữ liệu.
            {
               productDTO.AnhSP.CopyTo(stream);
            }

            // lưu hình ảnh trên database

            Product product = new Product() 
            {
                Name = productDTO.Name,
                Brand = productDTO.Brand,
                DanhMuc = productDTO.DanhMuc,
                Gia = productDTO.Gia,
                MoTa = productDTO.MoTa ?? "",
                AnhSP = imageFileName,
                CreatedAt = DateTime.Now,
            };

            context.products.Add(product);
            context.SaveChanges();

            return Ok(product);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id,[FromForm]ProductDTO productDTO)
        {

            if (!listDanhMuc.Contains(productDTO.DanhMuc))
            {
                ModelState.AddModelError("Danh Mục", "Vui lòng chọn danh mục hợp lệ");
                return BadRequest(ModelState);
            }

            var product = context.products.Find(id);

            if(product == null) 
            {
                return NotFound();
            }

            string imageFileName = product.AnhSP;
            if(productDTO.AnhSP != null)
            {
                // lưu ảnh sản phẩm vào máy chủ
                imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                imageFileName += Path.GetExtension(productDTO.AnhSP.FileName);

                string imagesFolder = env.WebRootPath + "/images/products/";
                using (var stream = System.IO.File.Create(imagesFolder + imageFileName))
                {
                    productDTO.AnhSP.CopyTo(stream);
                }

                // xóa ảnh
                System.IO.File.Delete(imagesFolder + product.AnhSP);
            }

            // Cập nhập ảnh lên database
            product.Name = productDTO.Name;
            product.Brand = productDTO.Brand;
            product.DanhMuc = productDTO.DanhMuc;
            product.Gia = productDTO.Gia;
            product.MoTa = productDTO.MoTa ?? "";
            product.AnhSP = imageFileName;


            context.SaveChanges();

            return Ok(product);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = context.products.Find(id);
            if(product == null)
            {
                return NotFound();
            }

            //xóa ảnh trên máy chủ

            string imagesFolder = env.WebRootPath + "/images/products/";
            System.IO.File.Delete(imagesFolder + product.AnhSP);

            //xóa sản phẩm khỏi cơ sở dữ liệu
            context.products.Remove(product);
            context.SaveChanges();
            return Ok();

        }
    }
}
