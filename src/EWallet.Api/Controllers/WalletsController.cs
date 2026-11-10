using EWallet.Core.DTOs.Requests;
using EWallet.Core.DTOs.Responses;
using EWallet.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController(IWalletService walletService) : ControllerBase
{
    private readonly IWalletService _walletService = walletService;

    // Get by UserId
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var wallet = await _walletService.GetByUserIdAsync(userId);

        if (wallet is null)
        {
            return NotFound(new
            {
                message = $"Wallet for user ID {userId} was not found."
            });
        }

        var response = new WalletResponse
        {
            Id = wallet.Id,
            UserId = wallet.UserId,
            WalletNumber = wallet.WalletNumber,
            Balance = wallet.Balance
        };
        return Ok(response);
    }

    // Create
    [HttpPost("{userId:int}/topups")]
    public async Task<IActionResult> TopUp(int userId, TopUpRequest request)
    {
        var isTopUpSuccessful = await _walletService.TopUpAsync(userId, request.Amount, request.Description);

        if (!isTopUpSuccessful)
        {
            return NotFound(new
            {
                message = $"Wallet for user ID {userId} was not found."
            });
        }

        return Ok(new
        {
            message = "Top up Successful."
        });
    }
}