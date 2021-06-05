using ShoppingListServer.Models.ShoppingData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListServer.Models.Commands
{
    public class ShoppingListWithPermissionDTO
    {
        public ShoppingListWithPermissionDTO(ShoppingList list, string userId, ShoppingListPermissionType permission)
        {
            List = list;
            UserId = userId;
            Permission = permission;
        }

        public ShoppingList List { get; set; }
        public string UserId { get; set; }
        public ShoppingListPermissionType Permission { get; set; }
    }
}
