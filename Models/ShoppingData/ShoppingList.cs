using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ShoppingListServer.Entities
{
    public class ShoppingList
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public List<Item> ProductList { get; set; }

        [JsonIgnore]
        public string OwnerID { get; set; }
        
        [JsonIgnore]
        public List<String> AcessIDs { get; set; }
    }
}