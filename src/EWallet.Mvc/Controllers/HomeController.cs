using EWallet.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Mvc.Controllers;

public class HomeController(IUserService userService) : Controller
{
    private readonly IUserService _userService = userService;

    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllAsync();

        return View(users);
    }
}