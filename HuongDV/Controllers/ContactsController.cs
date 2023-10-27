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

        [HttpGet("subjects")]
        public IActionResult GetSubjects()
        {
            var listSubjects = context.subjects.ToList();
            return Ok(listSubjects);
        }

        [HttpPost("subjects")]
        public IActionResult CreateSubject(SubjectDTO subjectDTO)
        {
            
            Subject subject = new Subject()
            {
                Name = subjectDTO.Name
            };

            context.subjects.Add(subject);
            context.SaveChanges();
            return Ok(subject);
        }

   
        [Authorize(Roles ="admin")]
        [HttpGet]
        public IActionResult GetContacts(int? page, 
            string? name,
            string? phone,
            string? email)
        {
            IQueryable<contact> query = context.contacts;

            
            if (name != null)
            {
                query = query.Where(p => p.FirstName.Contains(name) || p.FirstName.Contains(name));
            }

            if (phone != null)
            {
                query = query.Where(p => p.Phone == phone);
            }

            if (email != null)
            {
                query = query.Where(p => p.Email.Contains(email));
            }



            if (page == null || page < 1) 
            {
                page = 1;
            }

            int pageSize = 5;
            int totalPages = 0;

           
            decimal count = query.Count();
            totalPages = (int) Math.Ceiling(count / pageSize);
            
            var contacts = query
                .Include(c=>c.Subject)
                .OrderByDescending(c => c.Id)
                .Skip((int)(page -1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                Contacts = contacts,
                TotalPages = totalPages,
                PageSize = pageSize,
                Page = page
            };

            return Ok(response);
        }

        
        [Authorize(Roles ="admin")]
        [HttpGet("{id}")]
        public IActionResult Getcontact(int id)
        {   
            var contact = context.contacts.Include(c => c.Subject).FirstOrDefault(c => c.Id == id);
            if(contact == null)
            {
                return NotFound();
            }
            
            return Ok(contact);
        }

        [HttpPost]
        public IActionResult CreateContact(ContactDTO contactDTO)
        {

            var subject = context.subjects.Find(contactDTO.SubjectId);
            if(subject == null)
            {
                ModelState.AddModelError("Subject", "chọn một chủ đề phù hợp");
                return BadRequest(ModelState);
            }
            
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

        
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            
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
