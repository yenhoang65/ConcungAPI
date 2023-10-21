using HuongDV.models;
using HuongDV.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HuongDV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly ApplicationDbcontext context;
        private readonly IWebHostEnvironment env;

        public BannerController(ApplicationDbcontext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;
        }

        [HttpGet]
        public IActionResult GetBanner()
        {
            var banner = context.Banners.ToList();
            return Ok(banner);
        }

        [HttpGet("{id}")]
        public IActionResult GetBanner(int id)
        {
            var banner = context.Banners.Find(id);

            if (banner == null)
            {
                return NotFound();
            }

            return Ok(banner);
        }

        [HttpPost]
        public IActionResult CreateBanner([FromForm] BannerDTO bannerDTO)
        {
            if (bannerDTO.AnhBanner == null)
            {
                ModelState.AddModelError("ImageFile", "Tệp hình ảnh là bắt buộc");
                return BadRequest(ModelState);
            }

            // lưu ảnh vào máy chủ
            string imageFileName = DateTime.Now.ToString("yyyyMMđHHmmssfff");
            imageFileName += Path.GetExtension(bannerDTO.AnhBanner.FileName);

            string imagesFoder = env.WebRootPath + "/images/Banner/";
            using (var stream = System.IO.File.Create(imagesFoder + imageFileName))
            {
                bannerDTO.AnhBanner.CopyTo(stream);
            }

            // lưu tienich vào database
            Tienich tienich = new Tienich()
            {
                ImageFileName = imageFileName,
                CreatedAt = DateTime.Now
            };
            context.Tienichs.Add(tienich);
            context.SaveChanges();
            return Ok(tienich);
        }

        //[HttpPut("{id}")]
        //public IActionResult UpdateBanner(int id, [FromForm] Banner bannerDTO)
        //{
        //    var banner = context.Banners.Find(id);
        //    if (banner == null)
        //    {
        //        return NotFound();
        //    }

        //    string imageFileName = banner.AnhBanner;
        //    if(bannerDTO.AnhBanner != null)
        //    {
        //        // lưu ảnh vào máy chủ

        //        imageFileName = DateTime.Now.ToString("yyyMMddssfff");
        //        imageFileName += Path.GetExtension(bannerDTO.AnhBanner.FileName);

        //        string imagesFolder = env.WebRootPath + "/images/products/";

        //        using (var stream = System.IO.File.Create(imagesFolder + imageFileName))
        //        {
        //            bannerDTO.AnhBanner.CopyTo(stream);
        //        }

        //        //

        //        System.IO.File.Delete(imagesFolder + banner.AnhBanner);
        //    }

        //    //up

        //    banner.AnhBanner = banner.AnhBanner;
        //    context.SaveChanges();  
        //    return Ok();
        //}

        [HttpDelete("{id}")]
        public IActionResult DeleteBanner(int id)
        {
            var banner = context.Banners.Find(id);
            if (banner == null)
            {
                return NotFound();
            }

            //xóa ảnh trên máy chủ

            string imagesFolder = env.WebRootPath + "/images/products/";
            System.IO.File.Delete(imagesFolder + banner.AnhBanner);

            //xóa sản phẩm khỏi cơ sở dữ liệu
            context.Banners.Remove(banner);
            context.SaveChanges();
            return Ok();

        }
    }
}
