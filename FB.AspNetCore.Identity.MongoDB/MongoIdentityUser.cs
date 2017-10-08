using global::MongoDB.Bson;
using global::MongoDB.Bson.Serialization.Attributes;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AspNetCore.Identity.MongoDB
{
    public class MongoIdentityUser : IdentityUser
    {
        public MongoIdentityUser(string userName, string email)
            : this()
        {
            Email = email;
            UserName = userName;
        }

        public MongoIdentityUser()
        {
            Id = ObjectId.GenerateNewId().ToString();
            Roles = new List<string>();
            Logins = new List<UserLoginInfo>();
            Claims = new List<UserClaim>();
            AuthTokens = new List<AuthToken>();
        }


        [BsonRepresentation(BsonType.ObjectId)]
        public override string Id { get; set; }


        public override string UserName { get; set; }

        public string Name { get; set; }
        public string NormalizedName { get; set; }

        public override bool EmailConfirmed { get; set; }
        public override string Email { get; set; }
        public override string NormalizedEmail { get; set; }
        public override string PhoneNumber { get; set; }
        public override bool PhoneNumberConfirmed { get; set; }


        public override string PasswordHash { get; set; }

        public override string SecurityStamp { get; set; }
        public DateTimeOffset? LockoutEndDate { get; set; }
        public override int AccessFailedCount { get; set; }
        public override bool LockoutEnabled { get; set; }
        public override bool TwoFactorEnabled { get; set; }

        [BsonIgnoreIfNull]
        public List<UserClaim> Claims { get; set; }

        [BsonIgnoreIfNull]
        public List<UserLoginInfo> Logins { get; set; }

        [BsonIgnoreIfNull]
        public List<string> Roles { get; set; }

        [BsonIgnoreIfNull]
        public List<AuthToken> AuthTokens { get; set; }

        #region Roles Operation
        public virtual void AddRole(string role)
        {
            Roles.Add(role);
        }

        public virtual void RemoveRole(string role)
        {
            Roles.Remove(role);
        }
        public virtual void RemoveRole(IEnumerable<string> roles)
        {
            Roles = Roles.Except(roles).ToList();
        }
        #endregion

        #region User Clamins Operation
        public void AddClaims(IEnumerable<UserClaim> claims)
        {
            foreach (var claim in claims)
            {
                AddClaim(claim);
            }
        }

        public void AddClaim(UserClaim claim)
        {
            var existingClaim = Claims.SingleOrDefault(t => t.Type == claim.Type && t.Value == claim.Value);
            if (existingClaim == null)
            {
                Claims.Add(claim);
            }
        }

        public void ReplaceClaim(UserClaim oldClaim, UserClaim newClaim)
        {
            if (Claims.Remove(oldClaim))
            {
                Claims.Add(newClaim);
            }
        }

        public void RemoveClaims(IEnumerable<UserClaim> claims)
        {
            foreach (var claim in claims)
            {
                var existingClaim = Claims.SingleOrDefault(t => t.Type == claim.Type && t.Value == claim.Value);
                if (existingClaim == null)
                {
                    Claims.Remove(claim);
                }
            }
        }
        #endregion

        #region User Login Info Operation
        public void AddLogins(IEnumerable<UserLoginInfo> ulis)
        {
            foreach (var item in ulis)
            {
                AddLogin(item);
            }
        }

        public void AddLogin(UserLoginInfo loginInfo)
        {
            var existingLoginInfo = Logins.SingleOrDefault(t => t.LoginProvider == loginInfo.LoginProvider);
            if (existingLoginInfo == null)
            {
                Logins.Add(loginInfo);
            }
            else
            {
                existingLoginInfo.ProviderDisplayName = loginInfo.ProviderDisplayName;
            }
        }

        public void RemoveLogin(string loginProvider, string providerKey)
        {
            Logins = Logins
                .Except(Logins.Where(li => li.ProviderKey == providerKey && li.LoginProvider == loginProvider))
                .ToList();
        }

        public void RemoveLogin(string loginProvider)
        {
            Logins = Logins.Except(Logins.Where(li => li.LoginProvider == loginProvider)).ToList();
        }
        #endregion

        #region Auth Token Operation
        public void AddToken(AuthToken token)
        {
            var existingToken = AuthTokens.SingleOrDefault(t => t.LoginProvider == token.LoginProvider && t.Name == token.Name);
            if (existingToken == null)
            {
                AuthTokens.Add(token);
            }
            else
            {
                existingToken.Token = token.Token;
            }
        }

        public void RemoveToken(string loginProvider, string name)
        {
            AuthTokens = AuthTokens
                .Except(AuthTokens.Where(t => t.LoginProvider == loginProvider && t.Name == name))
                .ToList();
        }

        #endregion
    }
}
