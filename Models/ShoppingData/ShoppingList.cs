using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using ShoppingListServer.Models.ShoppingData;

namespace ShoppingListServer.Models
{
    public class ShoppingList
    {
        public ShoppingList()
        {
        }

        // Unique Identity of the ShoppingList
        // Functions as bridge between Json files, Database, and API calls.
        [Key]
        public string Id { get; set; }

        [NotMapped]
        public string Name { get; set; }

        [NotMapped]
        public string Category { get; set; }

        [NotMapped]
        public List<GenericProduct> ProductList { get; set; }

        // The first access right is always the one of the owner.
        [JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
        public virtual List<ShoppingListPermission> ShoppingListPermissions { get; set; }
    }
}