using HuongDV.models;
using HuongDV.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public IActionResult GetContacts()
        {
            var contacts = context.contacts.Include(c=>c.Subject).ToList();
            return Ok(contacts);
        }

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
                ModelState.AddModelError("Subject", "Vui lòng chọn một chủ đề phù hợp");
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

        [HttpPut("{id}")]
        public IActionResult UpdateContact (int id, ContactDTO contactDTO)
        {
            var subject = context.subjects.Find(contactDTO.SubjectId);
            if (subject == null)
            {
                ModelState.AddModelError("Subject", "Vui lòng chọn một chủ đề phù hợp");
                return BadRequest(ModelState);
            }

            var contact = context.contacts.Find(id);
            if(contact == null)
            {
                return NotFound();
            }

            contact.FirstName = contactDTO.FirstName;
            contact.LastName = contactDTO.LastName;
            contact.Email = contactDTO.Email;
            contact.Phone = contactDTO.Phone ??"";
            contact.Subject = subject;
            contact.Message = contactDTO.Message;
            context.SaveChanges();
            return Ok(contact);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            //Cách 1
            //var contact = context.contacts.Find(id);
            //if(contact == null)
            //{
            //    return NotFound();
            //}

            //context.contacts.Remove(contact);
            //context.SaveChanges();
            //return Ok();

            //cách 2:
            try 
            {
                var contact = new contact() { Id = id, Subject = new Subject()};
                context.contacts.Remove(contact);
                context.SaveChanges();
            }
            catch (Exception)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
