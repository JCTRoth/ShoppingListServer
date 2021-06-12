using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListServer.Models.ShoppingData
{
    // Fine flags with 1 << x: https://stackoverflow.com/a/1030115
    // Combine flags: https://stackoverflow.com/a/1030103
    [Flags]
    public enum ShoppingListPermissionType
    {
        Undefined = 0,
        Read = 1 << 0,
        Write = 1 << 1 | Read,
        Delete = 1 << 2,
        ModifyAccess = 1 << 3,
        All = Write | Delete | ModifyAccess
    }

}
