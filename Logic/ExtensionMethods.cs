using System.Collections.Generic;
using System.Linq;
using ShoppingListServer.Entities;

namespace ShoppingListServer.Helpers
{
    public static class ExtensionMethods
    {
        // Creates a copies of the given users with password == null.
        public static IEnumerable<User> WithoutPasswords(this IEnumerable<User> users) 
        {
            if (users == null)
                return null;

            return users.Select(x => x.WithoutPassword());
        }

        // Creates a copy of the given user with password == null.
        public static User WithoutPassword(this User user) 
        {
            if (user == null)
                return null;

            user = user.Copy();
            user.PasswordHash = null;
            user.Salt = null;
            return user;
        }
    }
}