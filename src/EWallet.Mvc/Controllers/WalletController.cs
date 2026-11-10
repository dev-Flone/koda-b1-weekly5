using EWallet.Core.Exceptions;
using EWallet.Core.Interfaces.Services;
using EWallet.Mvc.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Mvc.Controllers;

public class WalletController(IUserService userService, IWalletService walletService, ITransactionService transactionService) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly IWalletService _walletService = walletService;
    private readonly ITransactionService _transactionService = transactionService;

    public async Task<IActionResult> Dashboard(int userId)
    {
        var user = await _userService.GetByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        var wallet = await _walletService.GetByUserIdAsync(userId);

        if (wallet is null)
        {
            return NotFound();
        }

        // Get All Transaction
        var transactions = await _transactionService.GetByUserId(userId);

        // Total income dan expense
        var totalIncome = transactions.Where(t => t.ReceiverWalletId == wallet.Id).Sum(t => t.Amount);
        var totalExpense = transactions.Where(t => t.SenderWalletId == wallet.Id).Sum(t => t.Amount);

        ViewBag.User = user;
        ViewBag.TotalIncome = totalIncome;
        ViewBag.TotalExpense = totalExpense;

        return View(wallet);
    }

    [HttpGet]
    public async Task<IActionResult> TopUp(int userId)
    {
        var user = await _userService.GetByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        var wallet = await _walletService.GetByUserIdAsync(userId);

        if (wallet is null)
        {
            return NotFound();
        }

        ViewBag.User = user;
        ViewBag.Wallet = wallet;

        return View(new TopUpViewModel
        {
            UserId = userId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TopUp(TopUpViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var isSuccessful = await _walletService.TopUpAsync(model.UserId, model.Amount, model.Description);

        if (!isSuccessful)
        {
            return NotFound();
        }

        return RedirectToAction(
            nameof(Dashboard),
            new { userId = model.UserId }
        );
    }

    [HttpGet]
    public async Task<IActionResult> Transfer(int userId)
    {
        var user = await _userService.GetByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        var wallet = await _walletService.GetByUserIdAsync(userId);

        if (wallet is null)
        {
            return NotFound();
        }

        var users = await _userService.GetAllAsync();

        var recipients = users.Where(u => u.Id != userId).ToList();

        ViewBag.User = user;
        ViewBag.Wallet = wallet;
        ViewBag.Recipients = recipients;

        return View(new TransferViewModel
        {
            SenderUserId = userId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Transfer(TransferViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadTransferViewData(model.SenderUserId);
            return View(model);
        }

        try
        {
            var isSuccessful = await _transactionService.TransferAsync(
            model.SenderUserId,
            model.ReceiverUserId,
            model.Amount,
            model.Description
        );

            if (!isSuccessful)
            {
                ModelState.AddModelError(string.Empty, "Sender or recipient wallet was not found.");

                await LoadTransferViewData(model.SenderUserId);
                return View(model);
            }

            return RedirectToAction(nameof(Dashboard), new { userId = model.SenderUserId });
        }
        catch (InsufficientBalanceException e)
        {
            ModelState.AddModelError(string.Empty, e.Message);

            await LoadTransferViewData(model.SenderUserId);
            return View(model);
        }
    }

    private async Task LoadTransferViewData(int userId)
    {
        var user = await _userService.GetByIdAsync(userId);
        var wallet = await _walletService.GetByUserIdAsync(userId);
        var users = await _userService.GetAllAsync();

        ViewBag.User = user;
        ViewBag.Wallet = wallet;
        ViewBag.Recipients = users.Where(u => u.Id != userId).ToList();
    }

    [HttpGet]
    public async Task<IActionResult> History(int userId, string filter = "All")
    {
        var user = await _userService.GetByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        var wallet = await _walletService.GetByUserIdAsync(userId);

        if (wallet is null)
        {
            return NotFound();
        }

        var transactions = await _transactionService.GetByUserId(userId);

        var historyItems = transactions.Select(transaction => new TransactionHistoryItem
        {
            Transactions = transaction,
            IsIncome = transaction.ReceiverWalletId == wallet.Id
        }).ToList();

        filter = filter.ToLowerInvariant();

        historyItems = filter switch
        {
            "income" => historyItems.Where(x => x.IsIncome).ToList(),
            "expense" => historyItems.Where(x => !x.IsIncome).ToList(),
            _ => historyItems
        };

        var model = new TransactionHistoryViewModel
        {
            UserId = userId,
            Filter = filter,
            Transactions = historyItems
        };

        return View(model);
    }
}