using ShoppingListServer.Models.ShoppingData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListServer.Exceptions
{
    public class NoShoppingListPermissionException : Exception
    {
        public NoShoppingListPermissionException(ShoppingListPermission _permission, ShoppingListPermissionType _expectedPermission)
        {
            Permission = _permission;
            ExpectedPermission = _expectedPermission;

            Console.Error.WriteLine("NoShoppingListPermissionException ShoppingListId " + _permission.ShoppingListId + "UserId " + _permission.UserId);
        }

        ShoppingListPermission Permission { get; set; }
        ShoppingListPermissionType ExpectedPermission { get; set; }
    }
}
