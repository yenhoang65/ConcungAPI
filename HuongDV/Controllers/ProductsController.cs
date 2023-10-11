using HuongDV.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HuongDV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbcontext context;

        public ProductsController(ApplicationDbcontext context)
        {
            this.context = context;
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
    }
}
