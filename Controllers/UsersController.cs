using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShoppingListServer.Services;
using ShoppingListServer.Entities;
using Newtonsoft.Json;

namespace ShoppingListServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        protected IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]Authenticate model)
        {
            var user = _userService.Authenticate(model.Id, model.Email, model.Password);

            if (user == null)
                return BadRequest(new { message = "Id or password is incorrect" });

            return Ok(user);
        }

        // Used to register users with id only or full users
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] object user_json_object)
        {
            User new_user = JsonConvert.DeserializeObject<User>(user_json_object.ToString());

            new_user.Guid = Guid.NewGuid().ToString();

            // A Unique Id will be used as User Id:
            // TO DO Check if entry is already registered
            if (_userService.AddUser(new_user))
            {
                return Ok(new_user);
            }
            else
            {
                return BadRequest(new { message = "Not registered" });
            };

        }


        /*
        [Authorize(Roles = Role.User)]
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var users =  _userService.GetAll();
            return Ok(users);
        }
        */


        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            // only allow admin to access other user records
            var currentUserId = User.Identity.Name;

            if (id != currentUserId && !User.IsInRole(Role.User))
                return Forbid();

            var user =  _userService.GetById(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
