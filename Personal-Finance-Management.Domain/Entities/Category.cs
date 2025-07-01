using Personal_Finance_Management.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personal_Finance_Management.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required CategoryType Type { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
