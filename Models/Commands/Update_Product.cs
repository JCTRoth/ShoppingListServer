using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListServer.Models.Commands
{
    public class Update_Product
    {
        // SyncID of List
        public string ShoppingListId { get; set; }

        public GenericProduct NewProduct { get; set; }
    }
}
