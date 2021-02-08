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
    public interface ILoggingService
    {
        // int GetID();

        // Result GetList(string userID, int syncID);


    }

    public class LoggingService : ILoggingService
    {
        private readonly AppSettings _appSettings;

        public LoggingService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        // Functions here
    }
}