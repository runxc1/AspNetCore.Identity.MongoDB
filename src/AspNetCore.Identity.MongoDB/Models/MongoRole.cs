using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dnx.Identity.MongoDB.Models
{
    public class MongoRole
    {
        public MongoRole()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        public MongoRole(string roleName) : this()
		{
            Name = roleName;
        }

        public string Id { get; private set; }

        public string Name { get; set; }
    }
}
