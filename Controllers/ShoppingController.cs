using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using ShoppingListServer.Entities;
using ShoppingListServer.Services;
using ShoppingListServer.Models;
using ShoppingListServer.Logic;

namespace ShoppingListServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ShoppingController : ControllerBase
    {
        protected IShoppingService _shoppingService;
        protected User_Access _user_access;

        public ShoppingController(IShoppingService shoppingService)
        {
            _shoppingService = shoppingService;
            _user_access = new User_Access();
        }

        // Returns a List ID to the App
        // The App can than Upload a new List with this UID
        [Authorize(Roles = Role.User)]
        [HttpGet("id")]
        public IActionResult GetID()
        {
            return Ok(_shoppingService.GetID());
        }

        [Authorize(Roles = Role.User)]
        [HttpGet("list")]
        public IActionResult GetList([FromBody] string syncID)
        {
            try
            {
                string userID = HttpContext.User.Identity.Name;
                Result result = _shoppingService.GetList(userID, syncID);

                if (result.WasFound == true)
                {
                    return Ok(result.ReturnValue);
                }

                return BadRequest(new { message = "Not Found" });
            }
            catch
            {
                Console.Error.WriteLine("GetList " + HttpContext.Request.Body.ToString());
                return BadRequest(new { message = "JSON Error" });
            }
        }

        // [AllowAnonymous] // TO DO Change to restricted
        [Authorize(Roles = Role.User)]
        [HttpPost("list")]
        public IActionResult AddList([FromBody] object shoppingList_json_object)
        {
            try
            {
                // Convert JSON to ShoppingList Object
                ShoppingList new_list_item = JsonConvert.DeserializeObject<ShoppingList>(shoppingList_json_object.ToString());

                // Don't allow other users to create a shopping list for other users
                new_list_item.OwnerID = User.Identity.Name;

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

        [Authorize(Roles = Role.User)]
        [HttpDelete("list")]
        public IActionResult DeleteList([FromBody] string del_syncID)
        {
            string userID = HttpContext.User.Identity.Name;

            // TO DO Check if user is allowed
            bool deleted = _shoppingService.DeleteList(userID, del_syncID);

            if (deleted)
            {
                return Ok();
            }

            return BadRequest(new { message = "JSON Error - Not Deleted" });
        }

        //
        // TODO ADDITEM IS MISSING
        //

        [AllowAnonymous] // TODO Change to restricted
        // [Authorize(Roles = Role.User)]
        [HttpPatch("listupdate")]
        public IActionResult Update_Item_In_List([FromBody] object update_request_json)
        {
            try
            {

                // Convert JSON to ShoppingList Object
                Update_Item updatelist_command = JsonConvert.DeserializeObject<Update_Item>(update_request_json.ToString());

                Result result = _shoppingService.GetList(User.Identity.Name, updatelist_command.SyncID);

                if (result.WasFound)
                {
                    if (_user_access.Is_User_Allowed_To_Edit(User.Identity.Name, result.ReturnValue))
                    {
                        // Add to list of shoppingLists
                        bool updated = _shoppingService.Update_Item_In_List(updatelist_command.OldItemName,
                                                                            updatelist_command.NewItem,
                                                                            result.ReturnValue);

                        if (!updated)
                        {
                            // already in List
                            return BadRequest(new { message = "List not updated " + update_request_json.ToString() });
                        }

                        return Ok();
                    }

                }

                return BadRequest(new { message = "List not found" });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Update_Item_In_List " + ex);
                return BadRequest(new { message = "JSON Error" });
            }
        }

        [Authorize(Roles = Role.User)]
        [HttpDelete("listupdate")]
        public IActionResult Remove_Item_In_List([FromBody] object update_request_json)
        {
            try
            {
                // Convert JSON to ShoppingList Object
                Remove_Item updatelist_command = JsonConvert.DeserializeObject<Remove_Item>(update_request_json.ToString());

                Result result = _shoppingService.GetList(User.Identity.Name, updatelist_command.SyncID);

                if (result.WasFound)
                {
                    if (_user_access.Is_User_Allowed_To_Edit(User.Identity.Name, result.ReturnValue))
                    {
                        // Add to list of shoppingLists
                        bool updated = _shoppingService.Remove_Item_In_List(updatelist_command.ItemName, result.ReturnValue);

                        if (!updated)
                        {
                            // already in List
                            return BadRequest(new { message = "List not updated " + update_request_json.ToString() });
                        }

                        return Ok();
                    }

                }

                return BadRequest(new { message = "List not found" });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Remove_Item_In_List " + ex);
                return BadRequest(new { message = "JSON Error" });
            }
        }

    }

}
