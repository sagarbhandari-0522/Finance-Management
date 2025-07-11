﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personal_Finance_Management.Application.Interfaces.IRepositories
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        public Task<int> SaveChangesAsync();
    }
}
