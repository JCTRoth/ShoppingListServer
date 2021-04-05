using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ShoppingListServer.Entities
{
    public class User
    {
        // The unique id. Must be part of each entity, see https://docs.microsoft.com/en-us/ef/core/modeling/keys?tabs=data-annotations.
        public int Id { get; set; }

        [Required]
        public string Guid { get; set; }

        public string EMail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        [JsonIgnore]
        // Allow only users to register via the API
        // Value send form request will be ignored -> Default value is applied
        public string Role { get; set; } = "user";

        [JsonIgnore]
        public string Token { get; set; }

    }
}