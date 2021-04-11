using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using ShoppingListServer.Models.ShoppingData;

namespace ShoppingListServer.Entities
{
    public class User
    {

        [Key, Required]
        public string Id { get; set; }

        public string EMail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        [JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
        // Allow only users to register via the API
        // Value send from request will be ignored -> Default value is applied
        public string Role { get; set; } = "user";

        public string Token { get; set; }

        [JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
        public virtual List<ShoppingListPermission> ShoppingListPermissions { get; set; } = new List<ShoppingListPermission>();

    }
}