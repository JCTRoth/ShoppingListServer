﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShoppingListServer.Entities;
using Newtonsoft.Json;
using ShoppingListServer.Models;
using ShoppingListServer.Helpers;
using ShoppingListServer.Models.Commands;
using ShoppingListServer.Services.Interfaces;

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

        // Is used to check if the server is reachable.
        [AllowAnonymous]
        [HttpHead("test")]
        public ActionResult TestMe()
        {
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]Authenticate model)
        {
            Result user = _userService.Authenticate(model.Id, model.Email, model.Password);

            if (user.WasFound == false)
                return BadRequest(new { message = "Id or password is incorrect" });

            return Ok(user.ReturnValue);
        }

        // Used to register users with id only or full users
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] object user_json_object)
        {
            RegisterRequest registerRequest = JsonConvert.DeserializeObject<RegisterRequest>(user_json_object.ToString());

            User new_user = new User
            {
                Id = Guid.NewGuid().ToString(),
                EMail = registerRequest.EMail,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Username = registerRequest.Username
            };

            if (_userService.AddUser(new_user, registerRequest.Password))
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

            return Ok(user.WithoutPassword());
        }
    }
}
