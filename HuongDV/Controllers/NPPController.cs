using HuongDV.models;
using HuongDV.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HuongDV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NPPController : ControllerBase
    {
        private readonly ApplicationDbcontext context;
        public NPPController(ApplicationDbcontext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetNPP()
        {
            var npp = context.NPPs.ToList();
            return Ok(npp);
        }

        [HttpGet("{id}")]
        public IActionResult GetNPP(int id)
        {
            var npp = context.NPPs.Find(id);

            if (npp == null)
            {
                return NotFound();
            }

            return Ok(npp);
        }

        [HttpPost]
        public IActionResult CreateContact(NPPDTO nppDTO)
        {
            if (nppDTO.Name == null)
            {
                ModelState.AddModelError("Name", "Vui lòng chọn một tên phù hợp");
                return BadRequest(ModelState);
            }

            NPP npp = new NPP()
            {
                Name = nppDTO.Name,
                Brand = nppDTO.Brand,
                Phone = nppDTO.Phone ?? "",
                Email = nppDTO.Email,
                Address = nppDTO.Address,
                CreatedAt = DateTime.Now
            };

            context.NPPs.Add(npp);
            context.SaveChanges();
            return Ok(npp);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateNPP(int id, [FromForm] NPPDTO nppDTO)
        {

            var npp = context.NPPs.Find(id);

            if (npp == null)
            {
                return NotFound();
            }

            // Cập nhập npp lên database
            npp.Name = nppDTO.Name;
            npp.Brand = nppDTO.Brand;
            npp.Phone = nppDTO.Phone;
            npp.Email = nppDTO.Email;
            npp.Address = nppDTO.Address;


            context.SaveChanges();

            return Ok(npp);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteNPP(int id)
        {
            var npp = context.NPPs.Find(id);
            if (npp == null)
            {
                return NotFound();
            }

            
            //xóa npp khỏi cơ sở dữ liệu
            context.NPPs.Remove(npp);
            context.SaveChanges();
            return Ok();

        }
    }
}
