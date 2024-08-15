using OrmMiniProject.Models;
using OrmMiniProject.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
    }
}
