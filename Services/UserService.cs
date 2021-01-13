using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShoppingListServer.Entities;
using ShoppingListServer.Helpers;

namespace ShoppingListServer.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);

        bool Add_User(User new_user);

        IEnumerable<User> GetAll();

        User GetById(string id);
    }

    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public bool Add_User(User new_user)
        {
            if(Tools.False_If_Empty_Or_Null(new_user.EMail))
            {
                if (! Tools.Is_Valid_Email(new_user.EMail))
                {
                    // Not Valid
                    return false;
                }
            }

            // Check if user in list
            // ID
            bool id = Program._users.Any(user => user.Id == new_user.Id);
            // EMail
            bool email = Program._users.Any(user => user.EMail == new_user.EMail);

            if(id || email)
            {
                // User is already in List
                return false; 
            }

            // Add User to list
            Program._users.Add(new_user);
            return true;
        }

        public User Authenticate(string id, string password)
        {
            // Users without password set will have pw null
            // Requests without password field will have value null 
            var user = Program._users.SingleOrDefault(x => x.Id == id && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                // TO DO 
                Expires = DateTime.UtcNow.AddDays(99999),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user.WithoutPassword();
        }

        public IEnumerable<User> GetAll()
        {
            return Program._users.WithoutPasswords();
        }

        public User GetById(string id) 
        {
            var user = Program._users.FirstOrDefault(x => x.Id == id);
            return user.WithoutPassword();
        }
    }
}