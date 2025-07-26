using Personal_Finance_Management.Domain.Entities;

namespace Personal_Finance_Management.Web.ViewModels
{
    public class TransactionListVM
    {
        public List<Transaction> Transactions { get; set; } = null!;
        public DateOnly? ToDate { get; set; }
        public DateOnly? FromDate { get; set; }
    }
}
