using EWallet.Core.Models;

namespace EWallet.Mvc.ViewModel;

public class TransactionHistoryViewModel
{
    public int UserId { get; set; }
    public string Filter { get; set; } = "All";
    public List<TransactionHistoryItem> Transactions { get; set; } = [];
}

public class TransactionHistoryItem
{
    public Transaction Transactions { get; set; } = null!;
    public bool IsIncome { get; set; }
    public string Direction => IsIncome ? "Income" : "Expense";
}