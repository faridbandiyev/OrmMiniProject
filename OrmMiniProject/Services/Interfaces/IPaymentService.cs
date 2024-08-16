using OrmMiniProject.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Services.Interfaces
{
    public interface IPaymentService
    {
        Task MakePaymentAsync(CreatePaymentDTO createPaymentDto, int userId);
        Task<List<PaymentDTO>> GetPaymentsAsync(int userId);
        Task<List<PaymentDTO>> GetPaymentsAsync();
    }
}
