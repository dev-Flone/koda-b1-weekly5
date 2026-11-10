using EWallet.Core.DTOs.Requests;
using EWallet.Core.DTOs.Responses;
using EWallet.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController(ITransactionService transactionService) : ControllerBase
{
    private readonly ITransactionService _transactionService = transactionService;

    [HttpPost("/api/transfers")]
    public async Task<IActionResult> Transfer(TransferRequest request)
    {
        var isTransferSuccessful = await _transactionService.TransferAsync(request.SenderUserId, request.ReceiverUserId, request.Amount, request.Description);

        if (!isTransferSuccessful)
        {
            return NotFound(new
            {
                message = "Sender of receiver wallet was not found."
            });
        }

        return Ok(new
        {
            message = "Transfer Successful."
        });
    }

    [HttpGet("/api/users/{userId:int}/transactions")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var transactions = await _transactionService.GetByUserId(userId);

        var response = transactions.Select(transaction => new TransactionResponse
        {
            Id = transaction.Id,
            SenderWalletId = transaction.SenderWalletId,
            ReceiverWalletId = transaction.ReceiverWalletId,
            Type = transaction.Type.ToString(),
            Amount = transaction.Amount,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt
        });

        return Ok(response);
    }
}