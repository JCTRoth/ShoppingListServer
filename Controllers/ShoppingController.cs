using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using ShoppingListServer.Entities;
using ShoppingListServer.Services;
using ShoppingListServer.Models;
using ShoppingListServer.Logic;
using ShoppingListServer.Database;
using ShoppingListServer.Models.Commands;
using ShoppingListServer.Models.ShoppingData;
using ShoppingListServer.Exceptions;

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

        [Authorize(Roles = Role.User)]
        [HttpGet("list/{syncID}")]
        public IActionResult GetList(string syncID)
        {
            string userID = HttpContext.User.Identity.Name;
            ShoppingList list = _shoppingService.GetList(userID, syncID);
            if (list != null)
                return Ok(list);
            else
                return BadRequest(new { message = "Not Found" });
        }

        [Authorize(Roles = Role.User)]
        [HttpPost("list")]
        public IActionResult AddList([FromBody] object shoppingList_json_object)
        {
            ShoppingList new_list_item = JsonConvert.DeserializeObject<ShoppingList>(shoppingList_json_object.ToString());
            bool added = _shoppingService.AddList(new_list_item, User.Identity.Name);

            if (added)
                return Ok(new_list_item);
            else
                return BadRequest("Adding failed. List already exists.");

        }

        [Authorize(Roles = Role.User)]
        [HttpDelete("list")]
        public IActionResult DeleteList([FromBody] string del_syncID)
        {
            string userID = HttpContext.User.Identity.Name;
            bool deleted = _shoppingService.DeleteList(userID, del_syncID);
            if (deleted)
                return Ok();
            else
                return BadRequest(new { message = "Deleting of list failed. List is already removed." });
        }

        //
        // TODO ADDITEM IS MISSING
        //

        [Authorize(Roles = Role.User)]
        [HttpPatch("listupdate")]
        public IActionResult Update_Item_In_List([FromBody] object update_request_json)
        {
            Update_Item updatelist_command = JsonConvert.DeserializeObject<Update_Item>(update_request_json.ToString());
            bool ok = _shoppingService.Update_Item_In_List(
                updatelist_command.OldItemName,
                updatelist_command.NewItem,
                User.Identity.Name,
                updatelist_command.ShoppingListId);

            if (ok)
                return Ok();
            else
                return BadRequest("Update of item failed. Item not found.");
        }

        [Authorize(Roles = Role.User)]
        [HttpDelete("listremove")]
        public IActionResult Remove_Item_In_List([FromBody] object update_request_json)
        {
            Remove_Item removeitem_command = JsonConvert.DeserializeObject<Remove_Item>(update_request_json.ToString());   
            bool ok = _shoppingService.Remove_Item_In_List(
                removeitem_command.ItemName,
                User.Identity.Name,
                removeitem_command.ShoppingListId);

            if (ok)
                return Ok();
            else
                return BadRequest("Remove of item failed. Item not found.");
        }

    }

}
