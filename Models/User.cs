﻿using System.Collections.Generic;
using MongoDB.Bson;

namespace Models
{
    public class User
    {
        public ObjectId Id { get; set; } 
        public int Point { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsPrivate { get; set; }
    }
}