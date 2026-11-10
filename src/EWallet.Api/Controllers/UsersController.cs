using EWallet.Core.DTOs.Requests;
using EWallet.Core.DTOs.Responses;
using EWallet.Core.Interfaces.Services;
using EWallet.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    // Get All
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();

        var response = users.Select(user => new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone
        });

        return Ok(users);
    }

    // Get By Id
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound(new
            {
                message = $"User with ID {id} was not found."
            });
        }

        var response = new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone
        };

        return Ok(user);
    }

    // Create
    [HttpPost]
    public async Task<ActionResult> Create(CreateUserRequest request)
    {
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone
        };

        var userId = await _userService.CreateAsync(user);

        var createdUser = await _userService.GetByIdAsync(userId);

        var response = new UserResponse
        {
            Id = createdUser!.Id,
            Name = createdUser.Name,
            Email = createdUser.Email,
            Phone = createdUser.Phone
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = userId },
            createdUser
        );
    }

    // Update
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateUserRequest request)
    {
        var user = new User
        {
            Id = id,
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone
        };

        var isUpdated = await _userService.UpdateAsync(user);

        if (!isUpdated)
        {
            return NotFound(new
            {
                message = $"User with ID {id} was not found."
            });
        }

        return NoContent();
    }
}
