using Personal_Finance_Management.Application.Interfaces.IRepositories;
using Personal_Finance_Management.Domain.Entities;
using Personal_Finance_Management.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personal_Finance_Management.Infrastructure.Repositories
{
    public class CategoryRepository :Repository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context) :base(context)
        {
            _context = context;
        }
    }
}
