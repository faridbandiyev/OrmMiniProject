using Microsoft.EntityFrameworkCore;
using OrmMiniProject.Contexts;
using OrmMiniProject.Models;
using OrmMiniProject.Repositories.Generic;
using OrmMiniProject.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Repositories.Implementations
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }
    }
}
