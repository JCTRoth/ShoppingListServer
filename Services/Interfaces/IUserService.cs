using System.Collections.Generic;
using ShoppingListServer.Entities;
using ShoppingListServer.Models;

namespace ShoppingListServer.Services.Interfaces
{
    public interface IUserService
    {
        Result Authenticate(string id, string email, string password);

        bool AddUser(User new_user, string password);

        IEnumerable<User> GetAll();

        User GetById(string id);

        User GetByEMail(string email);
    }
}
