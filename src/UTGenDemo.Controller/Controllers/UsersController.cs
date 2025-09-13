using Microsoft.AspNetCore.Mvc;
using UTGenDemo.Service.Interfaces;
using UTGenDemo.Repository.Models;

namespace UTGenDemo.Controller.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found");
        }

        return Ok(user);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetActiveUsers()
    {
        var users = await _userService.GetActiveUsersAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdUser = await _userService.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<User>> UpdateUser(string id, [FromBody] User user)
    {
        if (user.Id != id)
        {
            return BadRequest("User ID mismatch");
        }

        var updatedUser = await _userService.UpdateUserAsync(user);
        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeactivateUser(string id)
    {
        var result = await _userService.DeactivateUserAsync(id);
        if (!result)
        {
            return NotFound($"User with ID {id} not found");
        }

        return NoContent();
    }
}