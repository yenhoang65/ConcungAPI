using HuongDV.models;
using HuongDV.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HuongDV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbcontext context;

        public CartController(ApplicationDbcontext context) 
        {
            this.context = context;
        }


        [HttpGet("PaymentMethods")]
        public IActionResult GetPaymentMethods()
        {
            return Ok(Orderhelper.PTThanhToan);
        }

        [HttpGet]
        public IActionResult GetCart(string productIdentifiers)
        {
            CartDTO cartDTO = new CartDTO();
            cartDTO.CartItems = new List<CartitemDTO>();
            cartDTO.SubTotal = 0;
            cartDTO.ShippingFee = Orderhelper.ShippingFee;
            cartDTO.TotalPrice = 0;

            var productDictionary = Orderhelper.GetProductDictionary(productIdentifiers);

            foreach (var pair in productDictionary)
            {
                int productId = pair.Key;
                var product = context.products.Find(productId);
                if (product == null)
                {
                    continue;
                }

                var cartItemDTO = new CartitemDTO();
                cartItemDTO.product = product;
                cartItemDTO.SoLuong = pair.Value;

                cartDTO.CartItems.Add(cartItemDTO);
                cartDTO.SubTotal += product.Gia * pair.Value;
                cartDTO.TotalPrice = cartDTO.SubTotal + cartDTO.ShippingFee;
            }

            return Ok(cartDTO);
        }
    }
}
