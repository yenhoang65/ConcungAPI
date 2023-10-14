using HuongDV.models;
using HuongDV.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        public IActionResult GetProducts(string? search, string? DanhMuc,
                int? minGia, int? maxGia,
                string? sort, string? DatHang,
                int? page)
        {
            IQueryable<Product> query = context.products;

            // chức năng tìm kiếm
            if(search != null) 
            {
                query = query.Where(p => p.Name.Contains(search) || p.MoTa.Contains(search));
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

            // sort functionality .. Chức năng sắp xếp

            if (sort == null) sort = "id";
            if (DatHang == null || DatHang != "asc") DatHang = "desc";

            if (sort.ToLower() == "name")
            {
                if (DatHang == "asc")
                {
                    query = query.OrderBy(p => p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Name);
                }
            }

            else if (sort.ToLower() == "brand")
            {
                if (DatHang == "asc")
                {
                    query = query.OrderBy(p => p.Brand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }

            else if (sort.ToLower() == "danhmuc")
            {
                if (DatHang == "asc")
                {
                    query = query.OrderBy(p => p.DanhMuc);
                }
                else
                {
                    query = query.OrderByDescending(p => p.DanhMuc);
                }
            }

            else if (sort.ToLower() == "gia")
            {
                if (DatHang == "asc")
                {
                    query = query.OrderBy(p => p.Gia);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Gia);
                }
            }

            else if (sort.ToLower() == "date")
            {
                if (DatHang == "asc")
                {
                    query = query.OrderBy(p => p.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatedAt);
                }
            }

            else
            {
                if (DatHang == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }

            // pagination functionality...chức năng phân trang
            if(page == null || page < 1) page =1;
            int pagesize = 5;
            int totalPage = 0;

            decimal count = query.Count();
            totalPage = (int)Math.Ceiling(count / pagesize);

            query = query.Skip((int) (page - 1) * pagesize).Take(pagesize);

            var products = query.ToList();

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

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult CrateProduct([FromForm] ProductDTO productDTO)
        {
            if (!listDanhMuc.Contains(productDTO.DanhMuc))
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

            using (var stream = System.IO.File.Create(imagesFolder + imageFileName))
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

            return Ok();
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
