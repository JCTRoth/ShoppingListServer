using System;
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
    public class ShoppingController : ControllerBase
    {
        protected IShoppingService _shoppingService;

        public ShoppingController(IShoppingService shoppingService)
        {
            _shoppingService = shoppingService;
        }

        // Returns a List ID to the App
        // The App can than Upload a new List with this UID
        [AllowAnonymous] // TO DO Change to restricted
        // [Authorize(Roles = Role.User)]
        [HttpGet("id")]
        public IActionResult GetID()
        {
            return Ok(_shoppingService.GetID());
        }

        [AllowAnonymous] // TO DO Change to restricted
        // [Authorize(Roles = Role.User)]
        [HttpGet("list")]
        public IActionResult GetList([FromBody] int syncID)
        {
            // TO DO GET USER ID FROM JWT
            string fakeID = "123";

            Result result = _shoppingService.GetList(fakeID, syncID);
            
            if (result.WasFound == true)
            {
                return Ok(result.ReturnValue);
            }

            return BadRequest(new { message = "Not Found" });
        }

        [AllowAnonymous] // TO DO Change to restricted
        // [Authorize(Roles = Role.User)]
        [HttpPost("list")]
        public IActionResult AddList([FromBody] object shoppingList_json_object)
        {
            try
            {
                // Convert JSON to ShoppingList Object
                ShoppingList new_list_item = JsonConvert.DeserializeObject<ShoppingList>(shoppingList_json_object.ToString());

                // TO DO Get User from Token
                // GET TOKEN FROM REQUEST
                // OR GET BY OWNER ID ON LIST??
                // int pos_key = Program._users.IndexOf(User => User.Token == );

                // TO DO Replace by DB
                // Add to list of shoppingLists
                bool added = _shoppingService.AddList(new_list_item);

                if (added)
                {
                    return Ok();
                }

                // already in List
                return BadRequest(new { message = "List was added before" });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("AddList " + ex);
                return BadRequest(new { message = "JSON Error" });
            }

        }

        [AllowAnonymous] // TO DO Change to restricted
        // [Authorize(Roles = Role.User)]
        [HttpPatch("list")]
        public IActionResult UpdateList([FromBody] object shoppingList_json_object)
        {
            try
            {
                // Convert JSON to ShoppingList Object
                ShoppingList new_list_item = JsonConvert.DeserializeObject<ShoppingList>(shoppingList_json_object.ToString());

                // TO DO Replace by DB
                // Add to list of shoppingLists
                bool updated = _shoppingService.UpdateList(new_list_item);

                if (! updated)
                {
                    // already in List
                    return BadRequest(new { message = "List not found" });
                }
                else
                {
                    // TO DO Check if user is allowed to edit list
                    // TO DO Implement UPDATE METHOD
                    return Ok(new { message = "Not Implemented!" });
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("UpdateList " + ex);
                return BadRequest(new { message = "JSON Error" });
            }

        }

        [AllowAnonymous] // TO DO Change to restricted
        // [Authorize(Roles = Role.User)]
        [HttpDelete("list")]
        public IActionResult DeleteList([FromBody] int del_syncID)
        {
            // TO DO GET USER ID FROM JWT
            string fake_id = "123";

            // TO DO Check if user is allowed
            bool deleted = _shoppingService.DeleteList(fake_id, del_syncID);

            if(deleted)
            {
                return Ok();
            }

            return BadRequest(new { message = "JSON Error - Not Deleted" });
        }

    }

}
