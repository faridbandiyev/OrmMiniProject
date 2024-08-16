using OrmMiniProject.DTOs.Order;
using OrmMiniProject.DTOs.OrderDetailDTO;
using OrmMiniProject.Enums;
using OrmMiniProject.Exceptions;
using OrmMiniProject.Models;
using OrmMiniProject.Repositories.Generic;
using OrmMiniProject.Repositories.Implementations;
using OrmMiniProject.Repositories.Interfaces;
using OrmMiniProject.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderDetail> _orderDetailRepository;
        private readonly IRepository<Product> _productRepository;

        public OrderService(IRepository<Order> orderRepository, IRepository<OrderDetail> orderDetailRepository, IRepository<Product> productRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
        }

        public async Task CreateOrderAsync(CreateOrderDTO createOrderDTO)
        {
            if (createOrderDTO.TotalAmount <= 0)
            {
                throw new InvalidOrderException("Order total amount must be greater than zero!");
            }

            var order = new Order
            {
                UserId = createOrderDTO.UserId,
                TotalAmount = createOrderDTO.TotalAmount,
                Status = createOrderDTO.Status,
                OrderDate = DateTime.UtcNow
            };

            await _orderRepository.CreateAsync(order);
            await _orderRepository.SaveChangesAsync();
        }

        public async Task CancelOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetSingleAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new NotFoundException("Order not found!");
            }

            if (order.Status == OrderStatus.Cancelled)
            {
                throw new OrderAlreadyCancelledException("Order has already been cancelled!");
            }

            order.Status = OrderStatus.Cancelled;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();
        }

        public async Task CompleteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetSingleAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new NotFoundException("Order not found!");
            }

            if (order.Status == OrderStatus.Completed)
            {
                throw new OrderAlreadyCompletedException("Order has already been completed!");
            }

            order.Status = OrderStatus.Completed;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();
        }

        public async Task<List<OrderDTO>> GetOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(o => new OrderDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status
            }).ToList();
        }

        public async Task AddOrderDetailAsync(int orderId, CreateOrderDetailDTO createOrderDetailDTO)
        {
            var product = await _productRepository.GetSingleAsync(p => p.Id == createOrderDetailDTO.ProductId);
            if (product == null)
            {
                throw new NotFoundException("Product not found.");
            }

            if (createOrderDetailDTO.Quantity > product.Stock)
            {
                throw new InvalidOrderDetailException("Cannot order more than available stock.");
            }

            var orderDetail = new OrderDetail
            {
                OrderId = orderId,
                ProductId = createOrderDetailDTO.ProductId,
                Quantity = createOrderDetailDTO.Quantity,
                PricePerItem = product.Price
            };

            product.Stock -= createOrderDetailDTO.Quantity;
            _productRepository.Update(product);
            await _orderDetailRepository.CreateAsync(orderDetail);
            await _orderDetailRepository.SaveChangesAsync();
        }

        public async Task<List<OrderDetailDTO>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            var orderDetails = await _orderDetailRepository.GetAllByPredicateAsync(od => od.OrderId == orderId);

            if (orderDetails == null || !orderDetails.Any())
            {
                throw new OrderNotFoundException("No order details found for the given order ID.");
            }

            return orderDetails.Select(od => new OrderDetailDTO
            {
                Id = od.Id,
                OrderId = od.OrderId,
                ProductId = od.ProductId,
                Quantity = od.Quantity,
                PricePerItem = od.PricePerItem
            }).ToList();
        }

        public async Task<List<OrderDTO>> GetUserOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetAllByPredicateAsync(o => o.UserId == userId);
            return orders.Select(o => new OrderDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status
            }).ToList();
        }
    }

}
