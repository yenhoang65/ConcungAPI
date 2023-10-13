using HuongDV.models;
using HuongDV.services;
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
        public IActionResult GetProducts() 
        {
            var products = context.products.ToList();
            return Ok(products);
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
