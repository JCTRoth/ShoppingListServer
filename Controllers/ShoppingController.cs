using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShoppingListServer.Entities;
using Newtonsoft.Json;
using System;
using System.Linq;

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

        // Returns a List ID to the App
        // The App can than Upload a new List with this UID
        [AllowAnonymous] // TO DO Change to restricted
        // [Authorize(Roles = Role.User)]
        [HttpGet("id")]
        public IActionResult GetID([FromBody] object shoppingList_json_object)
        {
            // TO DO 
            // Implement Queue the processes one request after another.

            // Count 1 up
            int highest_id = Program._syncIDs.Last();
            int new_id = highest_id + 1;

            Program._syncIDs.Add(new_id);

            return Ok(new_id);
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
                // int pos_key = Program._users.IndexOf(User => User.Token == );

                // TO DO Replace by DB
                // Add to list of shoppingLists
                bool is_in_list = Program._shoppingLists.Any(ShoppingList => ShoppingList.SyncID == new_list_item.SyncID);

                if (is_in_list)
                {
                    // already in List
                    return BadRequest(new { message = "List was added before" });
                }

                return Ok();
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
                bool is_in_list = Program._shoppingLists.Any(ShoppingList => ShoppingList.SyncID == new_list_item.SyncID);

                if (! is_in_list)
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

            // TO DO Check if user is allowed

            int del_index = Program._shoppingLists.FindIndex(ShoppingList => ShoppingList.SyncID == del_syncID);
            if(del_index != -1)
            {
                Program._shoppingLists.RemoveAt(del_index);
                return Ok();
            }

            return BadRequest(new { message = "JSON Error" });
        }

    }

}
