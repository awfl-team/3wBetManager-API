using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Models
{
    public class Team
    {
        public ObjectId UId { get; set; }
        public int Id { get; set; }
        public int ApiId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Tla { get; set; }
        public string CrestUrl { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime Founded { get; set; }
        public string Colors { get; set; }
        public string Venue { get; set; }
    }
}