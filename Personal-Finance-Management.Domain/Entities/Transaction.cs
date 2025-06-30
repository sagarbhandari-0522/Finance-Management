using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personal_Finance_Management.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        [Required]
        [Range(0.01, double.MaxValue,ErrorMessage="Amount must be greater than zero" )]
        public decimal Amount { get; set; }
        [Required]
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public Category Category { get; set; } = null!;
    }
}
