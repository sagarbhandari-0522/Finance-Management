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
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        private readonly AppDbContext _context;
        public TransactionRepository(AppDbContext context) :base(context)
        {
            _context = context;

        }
    }
}
