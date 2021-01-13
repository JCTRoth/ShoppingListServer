using System.ComponentModel.DataAnnotations;

namespace ShoppingListServer.Entities
{
    public class Authenticate
    {
        [Required]
        public string Id { get; set; }

        // Users without account should also access the api
        public string Password { get; set; }
    }
}