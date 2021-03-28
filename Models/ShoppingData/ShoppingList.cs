using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ShoppingListServer.Entities
{
    public class ShoppingList
    {
        public string Name { get; set; }

        // Unique Identity of the ShoppingList
        // App pulls the SyncID from the Server.
        public string SyncID { get; set; }

        public string Category { get; set; }

        public List<GenericProduct> ProductList { get; set; }

        public string OwnerID { get; set; }
        
        public List<String> AcessIDs { get; set; }
    }
}