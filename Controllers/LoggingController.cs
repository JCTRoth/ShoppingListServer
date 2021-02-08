using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShoppingListServer.Entities;
using Newtonsoft.Json;
using ShoppingListServer.Services;
using ShoppingListServer.Models;

namespace ShoppingListServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class LoggingController : ControllerBase
    {
        protected ILoggingService _loggingService;

        public LoggingController(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        /*
        [AllowAnonymous] // TO DO Change to restricted
        // [Authorize(Roles = Role.User)]
        [HttpGet("id")]
        public IActionResult GetID()
        {
            return Ok(_shoppingService.GetID());
        }
        */

    }

}
