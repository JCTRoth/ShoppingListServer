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
using System.Collections.Generic;

namespace ShoppingListServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ShoppingController : ControllerBase
    {
        protected IShoppingService _shoppingService;
        protected IUserService _userService;
        protected User_Access _user_access;

        public ShoppingController(IShoppingService shoppingService, IUserService userService)
        {
            _shoppingService = shoppingService;
            _userService = userService;
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
        [HttpGet("lists")]
        public IActionResult GetLists()
        {
            string userID = HttpContext.User.Identity.Name;
            List<ShoppingList> lists = _shoppingService.GetLists(userID, ShoppingListPermissionType.Read);
            if (lists != null)
                return Ok(lists);
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
        [HttpDelete("list/{syncId}")]
        public IActionResult DeleteList(string syncId)
        {
            string userID = HttpContext.User.Identity.Name;
            bool deleted = _shoppingService.DeleteList(userID, syncId);
            if (deleted)
                return Ok();
            else
                return BadRequest(new { message = "Deleting of list failed. List is already removed." });
        }

        [HttpPatch("list")]
        public IActionResult UpdateList([FromBody] object listJson)
        {
            ShoppingList listIn = JsonConvert.DeserializeObject<ShoppingList>(listJson.ToString());
            bool success = _shoppingService.UpdateList(listIn, User.Identity.Name);
            if (success)
                return Ok();
            else
                return BadRequest("Update failed.");
        }

        //
        // TODO ADDITEM IS MISSING
        //

        [Authorize(Roles = Role.User)]
        [HttpPatch("item")]
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
        [HttpDelete("item")]
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

        [Authorize(Roles = Role.User)]
        [HttpPatch("product")]
        public IActionResult Update_Product_In_List([FromBody] object update_request_json)
        {
            Update_Product updatelist_command = JsonConvert.DeserializeObject<Update_Product>(update_request_json.ToString());
            bool ok = _shoppingService.Add_Or_Update_Product_In_List(
                updatelist_command.NewProduct,
                User.Identity.Name,
                updatelist_command.ShoppingListId);

            if (ok)
                return Ok();
            else
                return BadRequest("Update of item failed. Item not found.");
        }

        // Returns the permissions of all users of a list:
        // \return List<Tuple<User, string>> => List<Tuple<User, ShoppingListPermissionType>>
        [Authorize(Roles = Role.User)]
        [HttpGet("listpermission/{listId}")]
        public IActionResult GetListPermissions(string listId)
        {
            string userID = HttpContext.User.Identity.Name;
            List<Tuple<string, ShoppingListPermissionType>> userPermissions = _shoppingService.GetListPermissions(listId);
            if (userPermissions != null)
            {
                List<Tuple<User, string>> permissions = new List<Tuple<User, string>>();
                foreach (Tuple<string, ShoppingListPermissionType> tuple in userPermissions)
                {
                    permissions.Add(Tuple.Create(_userService.GetById(tuple.Item1), tuple.Item2.ToString()));
                }
                return Ok(permissions);
            }
            else
            {
                return BadRequest(new { message = "Not Found" });
            }
        }

        // Returns the permissions that this user has to any list.
        // \return List<Tuple<string, string>> => List<Tuple<ShoppingListId, ShoppingListPermissionType>>
        [Authorize(Roles = Role.User)]
        [HttpGet("listpermission")]
        public IActionResult GetUserListPermissions()
        {
            string thisUserId = HttpContext.User.Identity.Name;
            List<Tuple<string, ShoppingListPermissionType>> listPermissions =
                _shoppingService.GetUserListPermissions(thisUserId);

            if (listPermissions != null)
            {
                List<Tuple<string, string>> permissions = new List<Tuple<string, string>>();
                foreach (Tuple<string, ShoppingListPermissionType> tuple in listPermissions)
                {
                    permissions.Add(Tuple.Create(tuple.Item1, tuple.Item2.ToString()));
                }
                return Ok(permissions);
            }
            else
            { 
                return BadRequest(new { message = "Not Found" });
            }
        }

        // Returns the permission that a user has to a list:
        // Attention: This method required the userId to be sent, not the user e-mail.
        //            This should be either changed to email or this method should be removed altogether.
        // \return string => ShoppingListPermissionType
        [Authorize(Roles = Role.User)]
        [HttpGet("listpermission/{listId}/{userId}")]
        public IActionResult GetUserListPermission(string listId, string userId)
        {
            string thisUserId = HttpContext.User.Identity.Name;
            ShoppingListPermissionType permission = _shoppingService.GetUserListPermission(listId, thisUserId, userId);
            if (permission != ShoppingListPermissionType.Undefined)
                return Ok(permission.ToString());
            else
                return BadRequest(new { message = "Not Found" });
        }

        // Adds or if already existent updates the permission that target user has to target list
        // \param listPermission - Tuple<string, string, string> => TargetUserEMail, ShoppingListId, PermissionType
        [Authorize(Roles = Role.User)]
        [HttpPut("listpermission")]
        public IActionResult AddOrUpdateListPermission([FromBody] object listPermission)
        {
            Tuple<string, string, string> tupel =
                JsonConvert.DeserializeObject<Tuple<string, string, string>>(listPermission.ToString());

            string thisUserId = HttpContext.User.Identity.Name;
            string targetUserEMail = tupel.Item1;
            User user = _userService.GetByEMail(targetUserEMail);
            if (user != null)
            {
                string targetUserId = _userService.GetByEMail(targetUserEMail).Id;
                string shoppingListId = tupel.Item2;
                ShoppingListPermissionType permission =
                    (ShoppingListPermissionType)Enum.Parse(typeof(ShoppingListPermissionType), tupel.Item3, true);

                bool success = _shoppingService.AddOrUpdateListPermission(thisUserId, targetUserId, shoppingListId, permission);
                if (success)
                    return Ok();
            }
            return BadRequest(new { message = "Permission Change not possible." });
        }

        [Authorize(Roles = Role.User)]
        [HttpDelete("listpermission/{listId}/{userEMail}")]
        public IActionResult RemoveListPermission(string listId, string userEMail)
        {
            string thisUserId = HttpContext.User.Identity.Name;
            string targetUserId = _userService.GetByEMail(userEMail).Id;
            bool success = _shoppingService.RemoveListPermission(thisUserId, targetUserId, listId);
            if (success)
                return Ok();
            else
                return BadRequest(new { message = "Deleting of permission not possible." });
        }

    }

}
