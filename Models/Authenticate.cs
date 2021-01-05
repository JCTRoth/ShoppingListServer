using System.ComponentModel.DataAnnotations;

namespace ShoppingListServer.Entities
{
    public class Authenticate
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Password { get; set; }
    }
}