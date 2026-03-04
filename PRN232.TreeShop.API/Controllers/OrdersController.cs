using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LaptopShop.Services.Commons.Results;
using PRN232.LaptopShop.Services.Request;
using PRN232.LaptopShop.Services.Response;
using PRN232.LaptopShop.Services.Services;
using Repository.Utils;

namespace PRN232.LaptopShop.API.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        private string GetUserRole() => User.FindFirst(ClaimTypes.Role)?.Value ?? "User";


        /// <summary>
        /// Customer tạo đơn hàng mới
        /// </summary>
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            var userId = GetUserId();
            var result = await _orderService.CreateOrder(userId, request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<OrderResponse>.Ok(result.Value!, "Order created successfully"));
        }


        /// <summary>
        /// Xem chi tiết đơn hàng (tất cả role — authorization trong service)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById([FromRoute] int id)
        {
            var userId = GetUserId();
            var userRole = GetUserRole();
            var result = await _orderService.GetOrderById(id, userId, userRole);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<OrderResponse>.Ok(result.Value!, "Order retrieved successfully"));
        }


        /// <summary>
        /// Customer xem đơn hàng của mình
        /// </summary>
        [Authorize(Roles = "User")]
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();
            var result = await _orderService.GetMyOrders(userId);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<List<OrderResponse>>.Ok(result.Value!, "Orders retrieved successfully"));
        }


        /// <summary>
        /// Admin xem tất cả đơn hàng (phân trang, filter status)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null)
        {
            var result = await _orderService.GetAllOrders(pageIndex, pageSize, status);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<BasePaginatedList<OrderResponse>>.Ok(result.Value!, "Orders retrieved successfully"));
        }


        /// <summary>
        /// Admin/Shipper/User cập nhật trạng thái đơn hàng
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int id, [FromBody] UpdateOrderStatusRequest request)
        {
            var userId = GetUserId();
            var userRole = GetUserRole();
            var result = await _orderService.UpdateOrderStatus(id, userId, userRole, request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<OrderResponse>.Ok(result.Value!, "Order status updated successfully"));
        }


        /// <summary>
        /// Admin giao đơn cho shipper (chuyển status → Shipping)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/assign-shipper")]
        public async Task<IActionResult> AssignShipper([FromRoute] int id, [FromBody] AssignShipperRequest request)
        {
            var result = await _orderService.AssignShipper(id, request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<OrderResponse>.Ok(result.Value!, "Shipper assigned successfully"));
        }


        /// <summary>
        /// Shipper xem đơn được giao cho mình
        /// </summary>
        [Authorize(Roles = "Shipper")]
        [HttpGet("shipper-orders")]
        public async Task<IActionResult> GetShipperOrders()
        {
            var shipperId = GetUserId();
            var result = await _orderService.GetShipperOrders(shipperId);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<List<OrderResponse>>.Ok(result.Value!, "Shipper orders retrieved successfully"));
        }
    }
}
