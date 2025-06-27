using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Personal_Finance_Management.Application.Interfaces.IRepositories
{
    public interface IRepository<T> where T: class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<T?> FindAsync(params object[] keyvalues);
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
        Task AddAsync(T entity);
        void Remove(T entity);
    }
}
