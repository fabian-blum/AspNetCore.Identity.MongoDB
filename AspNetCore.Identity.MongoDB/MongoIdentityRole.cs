using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AspNetCore.Identity.MongoDB
{
    /// <summary>
    /// Represents a role in the identity system
    /// </summary>
    public class MongoIdentityRole : IdentityRole<string>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MongoIdentityRole"/>.
        /// </summary>
        public MongoIdentityRole() { }

        /// <summary>
        /// Initializes a new instance of <see cref="MongoIdentityRole"/>.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        public MongoIdentityRole(string roleName) : this()
        {
            Name = roleName;
            Id = ObjectId.GenerateNewId().ToString();
        }

        /// <summary>
        /// A random value that should change whenever a role is persisted to the store
        /// </summary>
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the primary key for this role.
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string Id { get; set; }

        /// <summary>
        /// Gets or sets the name for this role.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the normalized name for this role.
        /// </summary>
        public virtual string NormalizedName { get; set; }

    }
}