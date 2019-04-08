using System.Collections.Generic;
using MongoDB.Bson;

namespace Models
{
    public class User
    {
        public const int DefaultLife = 3;
        public const string UserRole = "USER";
        public const string AdminRole = "ADMIN";
        public const int DefaultPoint = 500;
        public const bool DefaultIsPrivate = false;
        public const int DefaultTotalPointsUsedToBet = 0;


        public ObjectId Id { get; set; }
        public int Point { get; set; }
        public string Role { get; set; }
        public int TotalPointsUsedToBet { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsPrivate { get; set; }
        public List<Item> Items { get; set; }
    }
}