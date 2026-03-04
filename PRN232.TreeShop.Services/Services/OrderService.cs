using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRN232.LaptopShop.Repo.Entities;
using PRN232.LaptopShop.Repo.Repository;
using PRN232.LaptopShop.Services.Commons.Results;
using PRN232.LaptopShop.Services.Request;
using PRN232.LaptopShop.Services.Response;
using Repository.Utils;

namespace PRN232.LaptopShop.Services.Services
{
    public class OrderService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;
        private readonly IMapper _mapper;

        private static readonly decimal StandardShippingFee = 15000m;
        private static readonly decimal ExpressShippingFee = 30000m;

        public OrderService(UnitOfWork unitOfWork, ILogger<OrderService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Customer tạo đơn hàng mới
        /// </summary>
        public async Task<Result<OrderResponse>> CreateOrder(int userId, OrderRequest request)
        {
            try
            {
                // 1. Validate products exist and have enough stock
                var orderDetails = new List<OrderDetail>();
                decimal itemsTotal = 0;

                foreach (var item in request.Items)
                {
                    var product = await _unitOfWork.ProductRepo.FindAsync(p => p.Id == item.ProductId && p.Status == true);
                    if (product == null)
                    {
                        return Result<OrderResponse>.Failure(null, 404, $"Product with ID {item.ProductId} not found");
                    }

                    if (product.StockQuantity < item.Quantity)
                    {
                        return Result<OrderResponse>.Failure(null, 400, $"Product '{product.ProductName}' only has {product.StockQuantity} items in stock");
                    }

                    // Snapshot product info
                    orderDetails.Add(new OrderDetail
                    {
                        ProductId = product.Id,
                        ProductName = product.ProductName,
                        Price = product.Price,
                        Quantity = item.Quantity,
                        ImageUrl = product.ImageUrl
                    });

                    itemsTotal += product.Price * item.Quantity;

                    // Deduct stock
                    product.StockQuantity -= item.Quantity;
                    _unitOfWork.ProductRepo.Update(product);
                }

                // 2. Calculate shipping fee
                var shippingMethod = request.ShippingMethod ?? "Standard";
                var shippingFee = shippingMethod == "Express" ? ExpressShippingFee : StandardShippingFee;
                var totalPrice = itemsTotal + shippingFee;

                // 3. Create order
                var order = new Order
                {
                    UserId = userId,
                    ReceiverName = request.ReceiverName,
                    ReceiverPhone = request.ReceiverPhone,
                    ShippingAddress = request.ShippingAddress,
                    ShippingMethod = shippingMethod,
                    ShippingFee = shippingFee,
                    TotalPrice = totalPrice,
                    Status = "Pending",
                    PaymentMethod = request.PaymentMethod ?? "COD",
                    Note = request.Note,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.OrderRepo.Add(order);
                await _unitOfWork.SaveChangesAsync();

                // 4. Create order details
                foreach (var detail in orderDetails)
                {
                    detail.OrderId = order.Id;
                    await _unitOfWork.OrderDetailRepo.Add(detail);
                }
                await _unitOfWork.SaveChangesAsync();

                // 5. Return response
                var createdOrder = await GetOrderEntityById(order.Id);
                var response = _mapper.Map<OrderResponse>(createdOrder);

                return Result<OrderResponse>.Success(response, 201);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while creating an order");
                return Result<OrderResponse>.Failure(null, 500, "An error occurred while creating an order");
            }
        }

        /// <summary>
        /// Xem chi tiết đơn hàng
        /// </summary>
        public async Task<Result<OrderResponse>> GetOrderById(int orderId, int userId, string userRole)
        {
            try
            {
                var order = await GetOrderEntityById(orderId);
                if (order == null)
                {
                    return Result<OrderResponse>.Failure(null, 404, "Order not found");
                }

                // Authorization: Customer can only see their own orders
                if (userRole == "User" && order.UserId != userId)
                {
                    return Result<OrderResponse>.Failure(null, 403, "You can only view your own orders");
                }

                // Shipper can only see orders assigned to them
                if (userRole == "Shipper" && order.ShipperId != userId)
                {
                    return Result<OrderResponse>.Failure(null, 403, "This order is not assigned to you");
                }

                var response = _mapper.Map<OrderResponse>(order);
                return Result<OrderResponse>.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while retrieving the order");
                return Result<OrderResponse>.Failure(null, 500, "An error occurred while retrieving the order");
            }
        }

        /// <summary>
        /// Customer xem đơn hàng của mình
        /// </summary>
        public async Task<Result<List<OrderResponse>>> GetMyOrders(int userId)
        {
            try
            {
                var orders = await _unitOfWork.OrderRepo.FindAllAsync(
                    o => o.UserId == userId,
                    q => q.Include(o => o.User)
                          .Include(o => o.Shipper)
                          .Include(o => o.OrderDetails));

                var response = _mapper.Map<List<OrderResponse>>(orders.OrderByDescending(o => o.CreatedAt).ToList());
                return Result<List<OrderResponse>>.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while retrieving orders");
                return Result<List<OrderResponse>>.Failure(null, 500, "An error occurred while retrieving orders");
            }
        }

        /// <summary>
        /// Admin xem tất cả đơn hàng (có phân trang)
        /// </summary>
        public async Task<Result<BasePaginatedList<OrderResponse>>> GetAllOrders(
            int pageIndex, int pageSize, string? status)
        {
            try
            {
                var query = _unitOfWork.OrderRepo.Entity
                    .Include(o => o.User)
                    .Include(o => o.Shipper)
                    .Include(o => o.OrderDetails)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(o => o.Status == status);
                }

                query = query.OrderByDescending(o => o.CreatedAt);

                var count = await query.CountAsync();
                var items = await query
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var mapped = _mapper.Map<List<OrderResponse>>(items);
                var result = new BasePaginatedList<OrderResponse>(mapped, count, pageIndex, pageSize);

                return Result<BasePaginatedList<OrderResponse>>.Success(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while retrieving all orders");
                return Result<BasePaginatedList<OrderResponse>>.Failure(null, 500, "An error occurred while retrieving all orders");
            }
        }

        /// <summary>
        /// Admin/Shipper cập nhật trạng thái đơn hàng
        /// </summary>
        public async Task<Result<OrderResponse>> UpdateOrderStatus(int orderId, int userId, string userRole, UpdateOrderStatusRequest request)
        {
            try
            {
                var order = await GetOrderEntityById(orderId);
                if (order == null)
                {
                    return Result<OrderResponse>.Failure(null, 404, "Order not found");
                }

                // Validate status transition
                var validationError = ValidateStatusTransition(order.Status!, request.Status, userRole, order, userId);
                if (validationError != null)
                {
                    return Result<OrderResponse>.Failure(null, 400, validationError);
                }

                order.Status = request.Status;
                order.UpdatedAt = DateTime.UtcNow;

                // Shipper marks as Delivered → save proof image
                if (request.Status == "Delivered")
                {
                    order.DeliveryImageUrl = request.DeliveryImageUrl;
                    order.DeliveryTimestamp = DateTime.UtcNow;
                }

                // Customer cancels → restore stock
                if (request.Status == "Cancelled")
                {
                    await RestoreStock(order);
                }

                _unitOfWork.OrderRepo.Update(order);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<OrderResponse>(order);
                return Result<OrderResponse>.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while updating order status");
                return Result<OrderResponse>.Failure(null, 500, "An error occurred while updating order status");
            }
        }

        /// <summary>
        /// Admin giao đơn cho shipper
        /// </summary>
        public async Task<Result<OrderResponse>> AssignShipper(int orderId, AssignShipperRequest request)
        {
            try
            {
                var order = await GetOrderEntityById(orderId);
                if (order == null)
                {
                    return Result<OrderResponse>.Failure(null, 404, "Order not found");
                }

                if (order.Status != "Confirmed")
                {
                    return Result<OrderResponse>.Failure(null, 400, "Can only assign shipper to Confirmed orders");
                }

                // Validate shipper exists and has Shipper role
                var shipper = await _unitOfWork.AccountRepo.FindAsync(a => a.Id == request.ShipperId && a.Role == "Shipper");
                if (shipper == null)
                {
                    return Result<OrderResponse>.Failure(null, 404, "Shipper not found");
                }

                order.ShipperId = request.ShipperId;
                order.Status = "Shipping";
                order.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.OrderRepo.Update(order);
                await _unitOfWork.SaveChangesAsync();

                // Re-fetch to include shipper navigation property
                order = await GetOrderEntityById(orderId);
                var response = _mapper.Map<OrderResponse>(order);
                return Result<OrderResponse>.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while assigning shipper");
                return Result<OrderResponse>.Failure(null, 500, "An error occurred while assigning shipper");
            }
        }

        /// <summary>
        /// Shipper xem đơn được giao cho mình
        /// </summary>
        public async Task<Result<List<OrderResponse>>> GetShipperOrders(int shipperId)
        {
            try
            {
                var orders = await _unitOfWork.OrderRepo.FindAllAsync(
                    o => o.ShipperId == shipperId,
                    q => q.Include(o => o.User)
                          .Include(o => o.Shipper)
                          .Include(o => o.OrderDetails));

                var response = _mapper.Map<List<OrderResponse>>(orders.OrderByDescending(o => o.CreatedAt).ToList());
                return Result<List<OrderResponse>>.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while retrieving shipper orders");
                return Result<List<OrderResponse>>.Failure(null, 500, "An error occurred while retrieving shipper orders");
            }
        }

        // ==================== Private helpers ====================

        private async Task<Order?> GetOrderEntityById(int orderId)
        {
            return await _unitOfWork.OrderRepo.FindAsync(
                o => o.Id == orderId,
                q => q.Include(o => o.User)
                      .Include(o => o.Shipper)
                      .Include(o => o.OrderDetails));
        }

        private string? ValidateStatusTransition(string currentStatus, string newStatus, string userRole, Order order, int userId)
        {
            // Define allowed transitions per role
            switch (newStatus)
            {
                case "Confirmed":
                    if (currentStatus != "Pending") return "Can only confirm Pending orders";
                    if (userRole != "Admin") return "Only Admin can confirm orders";
                    break;

                case "Shipping":
                    if (currentStatus != "Confirmed") return "Can only set Shipping for Confirmed orders";
                    if (userRole != "Admin") return "Only Admin can set Shipping status";
                    break;

                case "Delivered":
                    if (currentStatus != "Shipping") return "Can only mark Delivered for Shipping orders";
                    if (userRole != "Shipper") return "Only Shipper can mark as Delivered";
                    if (order.ShipperId != userId) return "This order is not assigned to you";
                    break;

                case "Completed":
                    if (currentStatus != "Delivered") return "Can only complete Delivered orders";
                    break;

                case "Cancelled":
                    if (currentStatus != "Pending" && currentStatus != "Confirmed")
                        return "Can only cancel Pending or Confirmed orders";
                    if (userRole == "User" && currentStatus != "Pending")
                        return "Customer can only cancel Pending orders";
                    if (userRole == "User" && order.UserId != userId)
                        return "You can only cancel your own orders";
                    break;

                default:
                    return $"Invalid status: {newStatus}";
            }

            return null; // Valid transition
        }

        private async Task RestoreStock(Order order)
        {
            foreach (var detail in order.OrderDetails)
            {
                var product = await _unitOfWork.ProductRepo.GetByIdAsync(detail.ProductId);
                if (product != null)
                {
                    product.StockQuantity = (product.StockQuantity ?? 0) + detail.Quantity;
                    _unitOfWork.ProductRepo.Update(product);
                }
            }
        }
    }
}
