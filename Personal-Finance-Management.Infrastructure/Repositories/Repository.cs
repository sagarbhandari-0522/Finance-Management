using Microsoft.EntityFrameworkCore;
using Personal_Finance_Management.Application.Interfaces.IRepositories;
using Personal_Finance_Management.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Personal_Finance_Management.Infrastructure.Repositories
{
    public class Repository<T> :IRepository<T> where T: class
    {
        private readonly AppDbContext _context;
        public Repository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(T entity)
        {
            await _context.AddAsync(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = _context.Set<T>();
            return await query.AnyAsync(filter);
        }

        public async Task<T?> FindAsync(params object[] keyvalues)
        {
            return await _context.Set<T>().FindAsync(keyvalues);
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (filter is not null)
            {
                query = query.Where(filter);
            }
            if (include is not null)
            {
                query = include(query);
            }
            return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _context.Set<T>();

            query = query.Where(filter);
            if (include is not null)
            {
                query = include(query);
            }
            return await query.FirstOrDefaultAsync();
        }

        public void Remove(T entity)
        {
            _context.Remove(entity);
        }
    }
}
