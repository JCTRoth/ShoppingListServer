using System;
using System.Collections.Generic;

namespace ShoppingListServer.Models
{
    public class ShoppingList
    {
        public string Name { get; set; }

        // Unique Identity of the ShoppingList
        // App pulls the SyncID from the Server.
        // Is non when no list was loaded from storage.
        public string SyncID { get; set; } = "non";

        public string Category { get; set; }

        public List<GenericProduct> ProductList { get; set; }

        public string OwnerID { get; set; }
        
        public List<String> AcessIDs { get; set; }
    }
}