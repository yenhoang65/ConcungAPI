using HuongDV.models;
using HuongDV.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HuongDV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TienichController : ControllerBase
    {
        private readonly ApplicationDbcontext context;
        private readonly IWebHostEnvironment env;

        public TienichController(ApplicationDbcontext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;
        }

        [HttpGet]
        public IActionResult GetTienich()
        {
            var tienich = context.Tienichs.ToList();
            return Ok (tienich);
        }

        [HttpGet("{id}")]
        public IActionResult GetTienich(int id)
        {
            var tienich = context.Tienichs.Find(id);

            if(tienich == null)
            {
                return NotFound();
            }

            return Ok(tienich);
        }

        [HttpPost]
        public IActionResult CreateTienich([FromForm] TienichDTO tienichDTO)
        {
            if(tienichDTO.ImageFileName == null)
            {
                ModelState.AddModelError("ImageFile", "Tệp hình ảnh là bắt buộc");
                return BadRequest(ModelState);
            }

            // lưu ảnh vào máy chủ
            string imageFileName = DateTime.Now.ToString("yyyyMMđHHmmssfff");
            imageFileName += Path.GetExtension(tienichDTO.ImageFileName.FileName);

            string imagesFoder = env.WebRootPath + "/images/Tienich/";
            using (var stream = System.IO.File.Create(imagesFoder + imageFileName)) 
            {
                tienichDTO.ImageFileName.CopyTo(stream);
            }

            // lưu tienich vào database
            Tienich tienich = new Tienich()
            {
                Name = tienichDTO.Name,
                ImageFileName = imageFileName,
                CreatedAt = DateTime.Now
            };
            context.Tienichs.Add(tienich);
            context.SaveChanges();
            return Ok(tienich);
        }

        //[HttpPut("{id}")]
        //public IActionResult UpdateTienich(int id, [FromForm] Tienich tienichDTO)
        //{
        //    var tienich = context.Tienichs.Find(id);
        //    if (tienich == null)
        //    {
        //        return NotFound();
        //    }

        //    string imageFileName = tienich.ImageFileName;
        //    if (tienich.ImageFileName != null)
        //    {
        //        // lưu ảnh vào máy chủ
        //        imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        //        imageFileName += Path.GetExtension(tienichDTO.ImageFileName.FileName);

        //        string imagesFoder = env.WebRootPath + "/images/Tienich";
        //        using (var stream = System.IO.File.Create(imagesFoder + imageFileName))
        //        {
        //            tienichDTO.ImageFileName.CopyTo(stream);
        //        }


        //        // Xóa hình ảnh cũ
        //        System.IO.File.Delete(imagesFoder + tienich.ImageFileName);
        //    }
        //    //update 
        //    tienich.Name = tienichDTO.Name;
        //    tienich.ImageFileName = imageFileName;

        //    context.SaveChanges();
        //    return Ok();
        //}

        [HttpDelete("{id}")]
        public IActionResult DeleteTienich(int id)
        {
            var tienich = context.Tienichs.Find(id);
            if (tienich == null)
            {
                return NotFound();
            }

            //xóa ảnh trên máy chủ

            string imagesFolder = env.WebRootPath + "/images/Tienich/";
            System.IO.File.Delete(imagesFolder + tienich.ImageFileName);

            //xóa khỏi cơ sở dữ liệu
            context.Tienichs.Remove(tienich);
            context.SaveChanges();
            return Ok();

        }
    }
}
