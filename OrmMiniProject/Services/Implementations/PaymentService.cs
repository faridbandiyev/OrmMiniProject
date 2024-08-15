using OrmMiniProject.DTOs.Payment;
using OrmMiniProject.Exceptions;
using OrmMiniProject.Models;
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

        public PaymentService(IPaymentRepository paymentRepository, IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
        }

        public async Task MakePaymentAsync(CreatePaymentDTO createPaymentDto)
        {
            if (createPaymentDto.Amount <= 0)
            {
                throw new InvalidPaymentException("Payment amount must be greater than zero!");
            }

            var order = await _orderRepository.GetSingleAsync(o => o.Id == createPaymentDto.OrderId);

            if (order == null)
            {
                throw new NotFoundException("Order not found!");
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
