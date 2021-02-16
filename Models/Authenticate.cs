using System.ComponentModel.DataAnnotations;

namespace ShoppingListServer.Entities
{
    public class Authenticate
    {
        public string Id { get; set; }

        public string Email { get; set; }

        // Users without account should also access the api
        public string Password { get; set; }
    }
}