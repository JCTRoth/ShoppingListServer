using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListServer.Models.Commands
{
    public class Update_ShoppingListPermission
    {
        public string ListId { get; set; }
        public string Username { get; set; }
        public string Permission { get; set; }
    }
}
