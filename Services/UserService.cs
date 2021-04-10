using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShoppingListServer.Database;
using ShoppingListServer.Entities;
using ShoppingListServer.Helpers;
using ShoppingListServer.Logic;
using ShoppingListServer.Models;

namespace ShoppingListServer.Services
{
    public interface IUserService
    {
        Result Authenticate(string id, string email, string password);

        bool AddUser(User new_user);

        IEnumerable<User> GetAll();

        User GetById(string id);
    }

    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly AppDb _db;

        public UserService(IOptions<AppSettings> appSettings, AppDb db)
        {
            _appSettings = appSettings.Value;
            _db = db;
        }

        public bool AddUser(User new_user)
        {
            // When Email address was set, than check if valid
            if(! string.IsNullOrEmpty(new_user.EMail))
            {
                if (! Tools.Is_Valid_Email(new_user.EMail))
                {
                    // Not Valid
                    return false;
                }
            }

            // Check if user in list
            // ID
            bool id = _db.Users.Any(user => user.Id == new_user.Id);
            // EMail
            bool email = _db.Users.Any(user => user.EMail == new_user.EMail);

            if(id || email)
            {
                // User is already in List
                return false; 
            }

            // Add User to list
            if (new Folder().Create_User_Folder(new_user.Guid))
            {
                _db.Users.Add(new_user);
                _db.SaveChanges();
                return true;
            }

            return false;
        }

        public Result Authenticate(string id, string email, string password)
        {
            Result result = new Result();
            User user;

            // Valid
            // Id = "123", Email == null, password == null
            // Id == null, Email == "abc@def.g", password = "123" 
            //
            // Invalid
            // Email is set but no pw
            //

            if (! string.IsNullOrEmpty(id))
            {
                user = FindUser_ID(id, password);
            }
            else
            {
                if (! string.IsNullOrEmpty(email))
                {
                    return FindUser_EMail(email, password);
                }
                else
                {
                    result.WasFound = false;
                    return result;
                }
            }


            // return null if user not found
            if (user == null)
            {
                result.WasFound = false;
                return result;
            }


            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Guid),
                    new Claim(ClaimTypes.Email, user.EMail),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                // TODO 
                // Expires = DateTime.UtcNow.AddDays(99999),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            _db.SaveChanges();

            result.WasFound = true;
            result.ReturnValue = user.WithoutPassword();
            return result;
        }

        // Returns null when user not found
        // Users without password set will have pw null
        // Requests without password field will have value null 
        private dynamic FindUser_ID(string id, string password)
        {
            var user = _db.Users.SingleOrDefault(x => x.Guid == id && x.Password == password);

            return user;
        }

        // Returns null when user not found
        // Email only has to have pw in request
        private Result FindUser_EMail(string email, string password)
        {
            Result result = new Result();

            if(! string.IsNullOrEmpty(password))
            {
                result.WasFound = true;
                result.ReturnValue = _db.Users.SingleOrDefault(x => x.EMail == email && x.Password == password);
                return result;
            }
            else
            {
                result.WasFound = false;
                return result;
            }
        }

        public IEnumerable<User> GetAll()
        {
            return _db.Users.WithoutPasswords();
        }

        public User GetById(string id) 
        {
            var user = _db.Users.FirstOrDefault(x => x.Guid == id);
            return user.WithoutPassword();
        }
    }
}