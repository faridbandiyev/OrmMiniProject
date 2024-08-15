using OrmMiniProject.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrmMiniProject.Repositories.Generic
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<List<T>> GetAllAsync(params string[] includes);
        Task<List<T>> GetAllByPredicateAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate);
        Task CreateAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveChangesAsync();
    }
}
