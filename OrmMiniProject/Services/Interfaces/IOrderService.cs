using OrmMiniProject.DTOs.Order;
using OrmMiniProject.DTOs.OrderDetailDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Services.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrderAsync(CreateOrderDTO createOrderDTO);
        Task CancelOrderAsync(int orderId);
        Task CompleteOrderAsync(int orderId);
        Task<List<OrderDTO>> GetOrdersAsync();
        Task AddOrderDetailAsync(int orderId, CreateOrderDetailDTO createOrderDetailDTO);
        Task<List<OrderDetailDTO>> GetOrderDetailsByOrderIdAsync(int orderId);
    }
}
