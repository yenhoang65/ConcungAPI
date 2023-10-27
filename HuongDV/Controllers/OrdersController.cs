using Azure;
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
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbcontext context;

        public OrdersController(ApplicationDbcontext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetOrders(int? page) 
        {
            int userId = JwtReader.GetUserId(User);
            string role = context.Users.Find(userId)?.Role ??"";

            IQueryable<Order> query = context.Orders.Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product);
            if(role != "admin")
            {
                query = query.Where(o => o.UserId == userId);
            }

            query = query.OrderByDescending(o => o.Id);


            //thực hiện chức năng phân trang
            if (page == null || page < 1)
            {
                page = 1;
            }

            int pageSize = 5;
            int totalPages = 0;

            decimal count = query.Count();
            totalPages = (int)Math.Ceiling(count / pageSize);

            query = query.Skip((int)(page - 1) * pageSize)
                .Take(pageSize);


            // đọc đơn hàng
            var orders = query.ToList();

            foreach(var order in orders)
            {
                //thoát khỏi
                foreach(var item in order.OrderItems)
                {
                    item.Order = null;
                }

                order.User.Password = "";
            }

            var response = new
            {
                Orders = orders,
                TotalPages = totalPages,
                PageSize = pageSize,
                Page = page
            };

            return Ok(response);
        }



        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            int userId = JwtReader.GetUserId(User);
            string role = context.Users.Find(userId)?.Role ?? ""; // JwtReader.GetUserRole(User);

            Order? order = null;

            if (role == "admin")
            {
                order = context.Orders.Include(o => o.User)
                    .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                    .FirstOrDefault(o => o.Id == id);
            }
            else
            {
                order = context.Orders.Include(o => o.User)
                    .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                    .FirstOrDefault(o => o.Id == id && o.UserId == userId);
            }

            if (order == null)
            {
                return NotFound();
            }


            // get rid of the object cycle
            foreach (var item in order.OrderItems)
            {
                item.Order = null;
            }


            // hide the user password
            order.User.Password = "";


            return Ok(order);
        }


        [Authorize]
        [HttpPost]
        public IActionResult CreateOrder(OrderDTO orderDTO)
        {
            //check if the payment method is valid or not
            if (!Orderhelper.PTThanhToan.ContainsKey(orderDTO.PTThanhToan))
            {
                ModelState.AddModelError("Phương thức thanh toán", "Vui lòng chọn Phương thức thanh toán hợp lệ");
                return BadRequest(ModelState);
            }

            int userId = JwtReader.GetUserId(User);
            if (User == null)
            {
                ModelState.AddModelError("Đơn hàng", "Không thể tạo đơn hàng");
                return BadRequest(ModelState);
            }

            var productDictionary = Orderhelper.GetProductDictionary(orderDTO.LayChuoiSP);

            //create a new order
            Order order = new Order();
            order.UserId = userId;
            order.CreatedDate = DateTime.Now;
            order.Ship = Orderhelper.ShippingFee;
            order.DiaChiGH = orderDTO.DiachiGH;
            order.PTThanhToan = orderDTO.PTThanhToan;
            order.TinhTrangTT = Orderhelper.TinhTrangTT[0];
            order.TTDatHang = Orderhelper.TTDatHang[0];

            foreach (var pair in productDictionary)
            {
                int producId = pair.Key;
                var product = context.products.Find(producId);
                if (product == null)
                {
                    ModelState.AddModelError("Sản Phẩm", "Sản phẩm có Id" + producId + " Không có sẵn");
                    return BadRequest(ModelState);
                }

                var orderItem = new OrderItem();
                orderItem.ProductId = producId;
                orderItem.SoLuong = pair.Value;
                orderItem.UnitPrice = product.Gia;

                order.OrderItems.Add(orderItem);
            }

            if (order.OrderItems.Count < 1)
            {
                ModelState.AddModelError("Đơn hàng", "Không thể tạo đơn hàng");
                return BadRequest(ModelState);
            }
            // save the order in the database
            context.Orders.Add(order);
            context.SaveChanges();


            // get rid of the object cycle
            foreach (var item in order.OrderItems)
            {
                item.Order = null;
            }

            // hide the user password
            //order.User.Password = "";

            return Ok(order);
        }



        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, string? TinhTrangTT, string? TTDatHang)
        {
            if (TinhTrangTT == null && TTDatHang == null)
            {
                // we have nothing to do
                ModelState.AddModelError("Cập nhật đơn hàng", "Không có gì để cập nhật");
                return BadRequest(ModelState);
            }


            if (TinhTrangTT != null && !Orderhelper.TinhTrangTT.Contains(TinhTrangTT))
            {
                // the payment status is not valid
                ModelState.AddModelError("Trạng thái thanh toán", "Trạng thái thanh toán không hợp lệ");
                return BadRequest(ModelState);
            }


            if (TTDatHang != null && !Orderhelper.TTDatHang.Contains(TTDatHang))
            {
                // the order status is not valid
                ModelState.AddModelError("Trạng thái đơn hàng", "Trạng thái đơn hàng không hợp lệ");
                return BadRequest(ModelState);
            }


            var order = context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            if (TinhTrangTT != null)
            {
                order.TinhTrangTT = TinhTrangTT;
            }

            if (TTDatHang != null)
            {
                order.TTDatHang = TTDatHang;
            }


            context.SaveChanges();

            return Ok(order);
        }



        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var order = context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            context.Orders.Remove(order);
            context.SaveChanges();

            return Ok();
        }
    }
}
