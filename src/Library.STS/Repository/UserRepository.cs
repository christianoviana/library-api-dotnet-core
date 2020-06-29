using Library.STS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.STS.Repository
{
    public static class UserRepository
    {
        public static User Get(string username, string password)
        {
            var users = new List<User>();
            users.Add(new User { Id = Guid.NewGuid(), Username = "christiano", Password = "123456", Role = "manager" });
            users.Add(new User { Id = Guid.NewGuid(), Username = "steve", Password = "123456", Role = "employee" });
            users.Add(new User { Id = Guid.NewGuid(), Username = "admin", Password = "123456", Role = "admin" });

            return users.Where(x => x.Username.ToLower() == username.ToLower() && x.Password == password).FirstOrDefault();
        }
    }
}
