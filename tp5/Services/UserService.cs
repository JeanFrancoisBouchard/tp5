using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using MongoDB.Driver;
using tp5.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace tp5.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _Users;
        private readonly byte[] _Salt = new byte[128 / 8];

        public UserService(IConfiguration config)
        {
            var dbClient = new MongoClient(config.GetConnectionString("db"));
            var db = dbClient.GetDatabase("tp5");
            _Users = db.GetCollection<User>("users");
            
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(_Salt);
            }
        }

        public bool create(User user)
        {
            if(!_Users.Find(tempuser => tempuser.username.ToLower() == user.username.ToLower()).Any())
            {
                _Users.InsertOne(user);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool login(User user)
        {
            user.password = encodePassword(user.password);

            if (_Users.Find(tempuser => tempuser.username.ToLower() == user.username.ToLower() && tempuser.password == user.password).Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string encodePassword(string password)
        {
            string encoded = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: _Salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return encoded;
        }
    }
}
