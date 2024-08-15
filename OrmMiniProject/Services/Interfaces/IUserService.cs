using OrmMiniProject.DTOs.Order;
using OrmMiniProject.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Services.Interfaces
{
    public interface IUserService
    {
        Task RegisterUserAsync(CreateUserDTO userDto);
        Task<UserDTO> GetUserByIdAsync(int id);
        Task UpdateUserInfoAsync(UpdateUserDTO userDto);
        Task ExportUserOrdersToExcelAsync(int userId);
        Task<List<OrderDTO>> GetUserOrdersAsync(int userId);
    }
}
