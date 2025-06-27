using Personal_Finance_Management.Application.Interfaces.IRepositories;
using Personal_Finance_Management.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personal_Finance_Management.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository CategoryRepository { get; }
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            CategoryRepository = new CategoryRepository(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
          return  await _context.SaveChangesAsync();
        }
    }
}
