using ClosedXML.Excel;
using OrmMiniProject.DTOs.Order;
using OrmMiniProject.DTOs.User;
using OrmMiniProject.Exceptions;
using OrmMiniProject.Models;
using OrmMiniProject.Repositories.Interfaces;
using OrmMiniProject.Services.Interfaces;
using OrmMiniProject.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Address = u.Address
            }).ToList();
        }

        public async Task RegisterUserAsync(CreateUserDTO userDto)
        {
            var existingEmails = await _userRepository.GetAllEmailsAsync();

            if (!userDto.FullName.IsValidName())
            {
                throw new InvalidUserInformationException("User name cannot contain digits.");
            }

            if (!userDto.Email.IsValidEmail(existingEmails))
            {
                throw new InvalidUserInformationException("Email address is already in use.");
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
            var user = await GetUserByIdAsync(userId);
            var orders = await GetUserOrdersAsync(userId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Orders");

            worksheet.Cell(1, 1).Value = "User Details";
            worksheet.Cell(2, 1).Value = "Full Name:";
            worksheet.Cell(2, 2).Value = user.FullName;
            worksheet.Cell(3, 1).Value = "Email:";
            worksheet.Cell(3, 2).Value = user.Email;
            worksheet.Cell(4, 1).Value = "Address:";
            worksheet.Cell(4, 2).Value = user.Address;

            int startRow = 6;
            worksheet.Cell(startRow, 1).Value = "Order ID";
            worksheet.Cell(startRow, 2).Value = "Order Date";
            worksheet.Cell(startRow, 3).Value = "Total Amount";
            worksheet.Cell(startRow, 4).Value = "Status";

            int row = startRow + 1;
            foreach (var order in orders)
            {
                worksheet.Cell(row, 1).Value = order.Id;
                worksheet.Cell(row, 2).Value = order.OrderDate.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 3).Value = order.TotalAmount;
                worksheet.Cell(row, 4).Value = order.Status.ToString();
                row++;
            }

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = $"{path}\\User_{userId}_Orders.xlsx";
            workbook.SaveAs(filePath);
            Console.WriteLine($"Orders exported to {filePath}");
        }
    }
}
