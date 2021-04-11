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
        Read = 1 << 0,
        Write = 1 << 1,
        Delete = 1 << 2,
        ModifyAccess = 1 << 3,
        ReadWrite = Read | Write,
        All = ReadWrite | Delete | ModifyAccess
    }
}
