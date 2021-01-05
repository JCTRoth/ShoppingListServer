using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShoppingListServer.Entities;
using Newtonsoft.Json;

namespace ShoppingListServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ShoppingController : ControllerBase
    {
        public ShoppingController()
        {
        }

        // Used to register users with id only or full users
        [AllowAnonymous] // TO DO Change to restricted
        [HttpPost("list")]
        public IActionResult AddList([FromBody] object shoppingList_json_object)
        {
            ShoppingList new_list = JsonConvert.DeserializeObject<ShoppingList>(shoppingList_json_object.ToString());

            // TO DO get user that posted list, add info to ShoppingList Object

            return BadRequest(new { message = "To be implemented" });

        }

    }
}
