using OrmMiniProject.DTOs.Payment;
using OrmMiniProject.Enums;
using OrmMiniProject.Exceptions;
using OrmMiniProject.Models;
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
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public PaymentService(IPaymentRepository paymentRepository, IOrderRepository orderRepository, IUserRepository userRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        public async Task MakePaymentAsync(CreatePaymentDTO createPaymentDto, string email, string password)
        {
            var user = await _userRepository.GetSingleAsync(u => u.Email == email && u.Password == password);
            if (user == null)
            {
                throw new UserAuthenticationException("Invalid email or password.");
            }

            var order = await _orderRepository.GetSingleAsync(o => o.Id == createPaymentDto.OrderId);
            if (order == null)
            {
                throw new NotFoundException("Order not found.");
            }

            if (createPaymentDto.Amount < order.TotalAmount)
            {
                order.Status = OrderStatus.Pending;
            }
            else
            {
                order.Status = OrderStatus.Completed;
            }

            var payment = new Payment
            {
                OrderId = createPaymentDto.OrderId,
                Amount = createPaymentDto.Amount,
                PaymentDate = DateTime.UtcNow
            };

            await _paymentRepository.CreateAsync(payment);
            await _paymentRepository.SaveChangesAsync();
        }

        public async Task<List<PaymentDTO>> GetPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();

            return payments.Select(p => new PaymentDTO
            {
                Id = p.Id,
                OrderId = p.OrderId,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate
            }).ToList();
        }
    }
}
