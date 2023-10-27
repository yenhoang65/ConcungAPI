using HuongDV.models;
using HuongDV.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HuongDV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ApplicationDbcontext context;


        public ContactsController(ApplicationDbcontext context)
        {
            this.context = context;
        }

        //attribute để xác thực và phân quyền người dùng
        //lấy danh sách các chủ đề từ sql trả về dưới dạng danh sách json

        [HttpGet("subjects")]
        public IActionResult GetSubjects()
        {
            var listSubjects = context.subjects.ToList();
            return Ok(listSubjects);
        }

        [HttpPost("subjects")]
        public IActionResult CreateSubject(SubjectDTO subjectDTO)
        {
            // Tạo đối tượng liên hệ và thêm vào cơ sở dữ liệu.
            Subject subject = new Subject()
            {
                Name = subjectDTO.Name
            };

            context.subjects.Add(subject);
            context.SaveChanges();
            return Ok(subject);
        }

        // Phương thức GET để lấy danh sách các liên hệ (contacts) từ cơ sở dữ liệu.
        // Sử dụng phân trang và trả về dữ liệu dưới dạng JSON.
        [Authorize(Roles ="admin")]
        [HttpGet]
        public IActionResult GetContacts(int? page, string? name,
            string? phone,
            string? email)
        {
            IQueryable<contact> query = context.contacts;// tạo một truy vấn sử dụng Iq..cho phép mở rộng truy vấn trên các đk tìm kiếm

            // chức năng tìm kiếm
            if (name != null)
            {
                query = query.Where(p => p.FirstName.Contains(name) || p.FirstName.Contains(name));// kiểm tra name hoặc mô tả có chứa trường name hay kh 
            }

            if (phone != null)
            {
                query = query.Where(p => p.Phone == phone);
            }

            if (email != null)
            {
                query = query.Where(p => p.Email.Contains(email));
            }



            if (page == null || page < 1) // Kiểm tra trang hiện tại, nếu null hoặc nhỏ hơn 1 thì gán giá trị là 1.
            {
                page = 1;
            }

            // Số lượng items trên mỗi trang và tổng số trang.
            int pageSize = 5;
            int totalPages = 0;

            //lấy tổng số lượng liên hệ từ sql thông qua context.. để đếm số lg bản ghi trong bảng r trả về giá trị decimal
            decimal count = query.Count();
            //sau khi có đc tổng số lh thì kết quả sẽ chia là một số thập phân sau đó
           //math.ceiling để làm trong số thấp phan nhỏ hơn 1 
            totalPages = (int) Math.Ceiling(count / pageSize);
            

            // Lấy danh sách liên hệ sử dụng phân trang và include thông tin về chủ đề của liên hệ.
            var contacts = query
                .Include(c=>c.Subject)//sử dụng include để kết hợp dữ lieuejt ừ bảng conteac và sub
                .OrderByDescending(c => c.Id)//sắp xeepsdanh sách liên hệ theo thứ tự giảm dần của trường id
                .Skip((int)(page -1) * pageSize)//skip để bỏ qua các liên hệ trên các trang tước đó, dựa trên só trang hiện tịa
                .Take(pageSize)// xđ số lượng lh mà cta muốn lấy ra từ cơ sở dữ liệu 
                .ToList();// thực hiện truy vấn và chuyển kết quả thành danh sách các liên hệ

            //tạo đối tượng mới chứa dữ liệu lh quân trang
            var response = new
            {
                Contacts = contacts,
                TotalPages = totalPages,
                PageSize = pageSize,
                Page = page
            };

            return Ok(response);
        }

        // Phương thức GET để lấy một liên hệ dựa trên ID từ cơ sở dữ liệu.
        // Trả về dữ liệu dưới dạng JSON.
        [Authorize(Roles ="admin")]
        [HttpGet("{id}")]
        public IActionResult Getcontact(int id)
        {   
            //sử dụng truy vấn linQ để lấy một liên hệ từ sql
            //sử dụng phương thức Include để kết hợp thông tin chủ đề sub vào lh
            //sau đó sử dụng first để lấy liên hệ đầu tiên mà id của n khớp vs id truyền vào
            var contact = context.contacts.Include(c => c.Subject).FirstOrDefault(c => c.Id == id);
            if(contact == null)
            {
                return NotFound();
            }
            //để kiểm tra xem lh có đc tìm thấy kh nếu kh thấy contact là null thì sẽ báo lỗi
            return Ok(contact);
        }


        // Phương thức POST để tạo mới một liên hệ dựa trên dữ liệu từ yêu cầu.
        // Trả về dữ liệu liên hệ vừa được tạo dưới dạng JSON.
        [HttpPost]
        public IActionResult CreateContact(ContactDTO contactDTO)
        {

            // Tìm chủ đề liên quan đến liên hệ từ cơ sở dữ liệu.
            // Nếu không tìm thấy, trả về lỗi BadRequest.
            var subject = context.subjects.Find(contactDTO.SubjectId);
            if(subject == null)
            {
                ModelState.AddModelError("Subject", "Vui lòng chọn một chủ đề phù hợp");
                return BadRequest(ModelState);
            }
            // Tạo đối tượng liên hệ và thêm vào cơ sở dữ liệu.
            contact contact = new contact()
            {
                FirstName = contactDTO.FirstName,
                LastName = contactDTO.LastName,
                Email = contactDTO.Email,
                Phone = contactDTO.Phone ?? "",
                Subject = subject,
                Message = contactDTO.Message,
                CreatedAt = DateTime.Now
            };

            context.contacts.Add(contact);
            context.SaveChanges();
            return Ok(contact);
        }

        // Phương thức DELETE để xóa một liên hệ dựa trên ID từ cơ sở dữ liệu.
        // Trả về thành công nếu xóa thành công, hoặc NotFound nếu không tìm thấy liên hệ.
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            //Cách 1
            var contact = context.contacts.Find(id);
            if (contact == null)
            {
                return NotFound();
            }

            context.contacts.Remove(contact);
            context.SaveChanges();
            return Ok();

        }
    }
}
