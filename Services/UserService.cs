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
using ShoppingListServer.Logic;
using ShoppingListServer.Models;

namespace ShoppingListServer.Services
{
    public interface IUserService
    {
        User Authenticate(string id, string email, string password);

        bool AddUser(User new_user);

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


        public bool AddUser(User new_user)
        {
            // When Email address was set, than check if valid
            if(Tools.Is_NOT_empty(new_user.EMail))
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
            if (Folder.Create_User_Folder(new_user.Id))
            {
                Program._users.Add(new_user);
                return true;
            }

            return false;
        }

        public User Authenticate(string id, string email, string password)
        {
            var user = new User();

            // Valid
            // Id = "123", Email == null, password == null
            // Id == null, Email == "abc@def.g", password = "123" 
            //
            // Invalid
            // Email is set but no pw
            //
            if (Tools.Is_NOT_empty(id))
            {
                user = FindUser_ID(id, password);
            }
            else
            {
                if (Tools.Is_NOT_empty(email))
                {
                    user = FindUser_EMail(email, password);
                }
                else
                {
                    return null;
                }
            }


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
                    new Claim(ClaimTypes.Name, user.Id),
                    new Claim(ClaimTypes.Email, user.EMail),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                // TODO 
                // Expires = DateTime.UtcNow.AddDays(99999),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user.WithoutPassword();
        }

        // Returns null when user not found
        // Users without password set will have pw null
        // Requests without password field will have value null 
        private dynamic FindUser_ID(string id, string password)
        {
            var user = Program._users.SingleOrDefault(x => x.Id == id && x.Password == password);

            return user;
        }

        // Returns null when user not found
        // Email only has to have pw in request
        private dynamic FindUser_EMail(string email, string password)
        {
            if(Tools.Is_NOT_empty(password))
            {
                return Program._users.SingleOrDefault(x => x.EMail == email && x.Password == password);
            }
            else
            {
                return false;
            }
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