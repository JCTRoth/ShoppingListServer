using System.ComponentModel.DataAnnotations;

namespace ShoppingListServer.Entities
{
    public class User
    {
        [Required]
        public int Id { get; set; }
        
        public string EMail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        [Required]
        public string Role { get; set; }
        
        public string Token { get; set; }
    }
}