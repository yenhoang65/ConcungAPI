using HuongDV.models;
using HuongDV.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public IActionResult GetContact()
        {
            var contacts = context.contacts.ToList();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public IActionResult Getcontact(int id)
        {
            var contact = context.contacts.Find(id);
            if(contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        [HttpPost]
        public IActionResult CreateContact(ContactDTO contactDTO)
        {
            contact contact = new contact()
            {
                FirstName = contactDTO.FirstName,
                LastName = contactDTO.LastName,
                Email = contactDTO.Email,
                Phone = contactDTO.Phone ?? "",
                Subject = contactDTO.Subject,
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
            var contact = context.contacts.Find(id);
            if(contact == null)
            {
                return NotFound();
            }

            contact.FirstName = contactDTO.FirstName;
            contact.LastName = contactDTO.LastName;
            contact.Email = contactDTO.Email;
            contact.Phone = contactDTO.Phone ??"";
            contact.Subject = contactDTO.Subject;
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
                var contact = new contact() { Id = id };
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
