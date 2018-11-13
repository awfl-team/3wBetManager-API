using System;
using MongoDB.Bson;

namespace Models
{
    public class User
    {
        public ObjectId UId { get; set; }
        public int Id { get; set; }
        public int Point { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Pseudo { get; set; }
    }
}