using Microsoft.AspNetCore.Mvc;
using Services.Services;
using System.Net;

namespace Marketplace.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> CreateUser(int id)
        {
            _ = await _userService.CreateUserIfNotExists(id);
            return Created();
        }
    }
}