using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ShoppingListServer.Entities
{
    public class User
    {
        [Required]
        public string Id { get; set; }
        
        public string EMail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        [JsonIgnore]
        // Allow only users to register via the API
        // Value send form request will be ignored -> Default value is applyed
        public string Role { get; set; } = "user";

        [JsonIgnore]
        public string Token { get; set; }
    }
}