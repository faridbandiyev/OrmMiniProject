using ClosedXML.Excel;
using OrmMiniProject.DTOs.Order;
using OrmMiniProject.DTOs.User;
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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;

        public UserService(IUserRepository userRepository, IOrderRepository orderRepository)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            
        }

        public async Task RegisterUserAsync(CreateUserDTO userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.FullName) || string.IsNullOrWhiteSpace(userDto.Email) || string.IsNullOrWhiteSpace(userDto.Password))
            {
                throw new InvalidUserInformationException("User registration information is incomplete.");
            }

            var user = new User
            {
                FullName = userDto.FullName,
                Email = userDto.Email,
                Password = userDto.Password,
                Address = userDto.Address
            };

            await _userRepository.CreateAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetSingleAsync(u => u.Id == id);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Address = user.Address
            };
        }

        public async Task UpdateUserInfoAsync(UpdateUserDTO userDto)
        {
            var user = await _userRepository.GetSingleAsync(u => u.Id == userDto.Id);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            if (!string.IsNullOrWhiteSpace(userDto.FullName))
            {
                user.FullName = userDto.FullName;
            }

            if (!string.IsNullOrWhiteSpace(userDto.Email))
            {
                user.Email = userDto.Email;
            }

            if (!string.IsNullOrWhiteSpace(userDto.Address))
            {
                user.Address = userDto.Address;
            }

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<List<OrderDTO>> GetUserOrdersAsync(int userId)
        {
            var user = await _userRepository.GetSingleAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            var allOrders = await _orderRepository.GetAllByPredicateAsync(o => o.UserId == userId);

            var userOrders = allOrders.Select(o => new OrderDTO
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status
            }).ToList();

            return userOrders;
        }


        public async Task ExportUserOrdersToExcelAsync(int userId)
        {
            var orders = await GetUserOrdersAsync(userId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Orders");

            worksheet.Cell(1, 1).Value = "Order ID";
            worksheet.Cell(1, 2).Value = "Order Date";
            worksheet.Cell(1, 3).Value = "Total Amount";
            worksheet.Cell(1, 4).Value = "Status";

            int row = 2;
            foreach (var order in orders)
            {
                worksheet.Cell(row, 1).Value = order.Id;
                worksheet.Cell(row, 2).Value = order.OrderDate.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 3).Value = order.TotalAmount;
                worksheet.Cell(row, 4).Value = order.Status.ToString();
                row++;
            }


            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = $"{path}\\New folder\\User_{userId}_Orders.xlsx";
            workbook.SaveAs(filePath);
            Console.WriteLine($"Orders exported to {filePath}");

        }



    }
}
