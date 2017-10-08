using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCore.Identity.MongoDB.Test
{
    public class UserStoreTest
    {
        [Fact]
        public async Task AddUserToUnknownRoleFails()
        {
            var manager = CreateManager();
            var u = CreateTestUser();

            var result = await manager.CreateAsync(u);

            Assert.True(result.Succeeded);

            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await manager.AddToRoleAsync(u, "bogus"));

            AssertSucceeded(await manager.DeleteAsync(u));
        }

        [Fact]
        //public async Task CanAddAuthenticationTokenUsingUserManager()
        public async Task CanAddUserLoginInfo()
        {
            var guid = Guid.NewGuid().ToString();

            var serviceProvider = BuildServiceProvider();

            //var userStore = serviceProvider.GetService<IUserStore<IdentityUser>>();

            var userManager = serviceProvider.GetService<UserManager<MongoIdentityUser>>();

            var cancelToken = new CancellationTokenSource();

            var user = CreateTestUser();

            AssertSucceeded(await userManager.CreateAsync(user));

            var loginInfo = new Microsoft.AspNetCore.Identity.UserLoginInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Facebook");

            AssertSucceeded(await userManager.AddLoginAsync(user, loginInfo));

            var retrieveUser = await userManager.FindByIdAsync(user.Id);

            Assert.NotEmpty(retrieveUser.Logins);
            Assert.Equal(loginInfo.LoginProvider, retrieveUser.Logins[0].LoginProvider);
            Assert.Equal(loginInfo.ProviderKey, retrieveUser.Logins[0].ProviderKey);
            Assert.Equal("Facebook", retrieveUser.Logins[0].ProviderDisplayName);

            //IUserLockoutStore<IdentityUser>

            ////Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken);
            ////Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken);
            ////Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken);
            ////Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken);

            //var result = await userManager.CreateAsync(user);

            //Assert.True(result.Succeeded);

            AssertSucceeded(await userManager.DeleteAsync(user));
        }


        [Fact]
        public async Task CanRetrieveUserLoginInfo()
        {
            var guid = Guid.NewGuid().ToString();

            var serviceProvider = BuildServiceProvider();

            var userManager = serviceProvider.GetService<UserManager<MongoIdentityUser>>();

            var cancelToken = new CancellationTokenSource();

            var user = CreateTestUser();

            AssertSucceeded(await userManager.CreateAsync(user));

            var loginInfo = new Microsoft.AspNetCore.Identity.UserLoginInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Facebook");

            AssertSucceeded(await userManager.AddLoginAsync(user, loginInfo));

            var retrieveUser = await userManager.FindByIdAsync(user.Id);

            Assert.NotEmpty(retrieveUser.Logins);
            Assert.Equal(loginInfo.LoginProvider, retrieveUser.Logins[0].LoginProvider);
            Assert.Equal(loginInfo.ProviderKey, retrieveUser.Logins[0].ProviderKey);
            Assert.Equal("Facebook", retrieveUser.Logins[0].ProviderDisplayName);


            var foundUser = await userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);

            Assert.NotNull(foundUser);
            Assert.Equal(user.Id, foundUser.Id);

            AssertSucceeded(await userManager.DeleteAsync(user));
        }

        [Fact]
        public async Task CanRemoveUserLoginInfo()
        {
            var guid = Guid.NewGuid().ToString();

            var serviceProvider = BuildServiceProvider();

            var userManager = serviceProvider.GetService<UserManager<MongoIdentityUser>>();

            var cancelToken = new CancellationTokenSource();

            var user = CreateTestUser();

            AssertSucceeded(await userManager.CreateAsync(user));

            var loginInfo = new Microsoft.AspNetCore.Identity.UserLoginInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Facebook");

            AssertSucceeded(await userManager.AddLoginAsync(user, loginInfo));

            var retrieveUser = await userManager.FindByIdAsync(user.Id);

            Assert.NotEmpty(retrieveUser.Logins);
            Assert.Equal(loginInfo.LoginProvider, retrieveUser.Logins[0].LoginProvider);
            Assert.Equal(loginInfo.ProviderKey, retrieveUser.Logins[0].ProviderKey);
            Assert.Equal("Facebook", retrieveUser.Logins[0].ProviderDisplayName);


            AssertSucceeded(await userManager.RemoveLoginAsync(retrieveUser, loginInfo.LoginProvider, loginInfo.ProviderKey));

            var noUser = await userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);

            Assert.Null(noUser);

            retrieveUser = await userManager.FindByIdAsync(user.Id);
            Assert.Empty(retrieveUser.Logins);


            AssertSucceeded(await userManager.DeleteAsync(user));
        }

        [Fact]
        public async Task CanCreateUserUsingMongoDb()
        {
            var serviceProvider = BuildServiceProvider();
            var db = serviceProvider.GetService<IMongoDBDbContext<IdentityUser, IdentityRole>>();

            var guid = Guid.NewGuid().ToString();
            var cancelToken = new CancellationTokenSource();

            await db.User.InsertOneAsync(new MongoIdentityUser
            {
                Email = $"someone.email${new Random().NextDouble()}@gmail.com",
                UserName = guid,
                PasswordHash = "randomepassword",
                Name = "name",
                NormalizedName = "USER",
                EmailConfirmed = true,
                NormalizedEmail = "SOMEONE.EMAIL@GMAIL.COM"
            }, null, cancelToken.Token);

            var found = await db.User.Find(u => u.UserName == guid, null).SingleAsync();
            Assert.NotNull(found);
        }

        [Fact]
        public async Task CanCreateUsingManager()
        {
            var guid = Guid.NewGuid().ToString();

            var serviceProvider = BuildServiceProvider();

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = CreateTestUser();

            AssertSucceeded(await userManager.CreateAsync(user));

            AssertSucceeded(await userManager.DeleteAsync(user));
        }

        [Fact]
        public async Task CanCreateUsingUserStore()
        {
            var serviceProvider = BuildServiceProvider();

            var userStore = serviceProvider.GetService<IUserStore<MongoIdentityUser>>();

            var user = new MongoIdentityUser
            {
                Email = "someone.email@gmail.com",
                UserName = "someone.email",
                PasswordHash = "randomepassword",
                Name = "name",
                NormalizedName = "USER",
                EmailConfirmed = true,
                NormalizedEmail = "SOMEONE.EMAIL@GMAIL.COM"
            };

            var cancelToken = new CancellationTokenSource();
            AssertSucceeded(await userStore.CreateAsync(user, cancelToken.Token));

            var found = await userStore.FindByIdAsync(user.Id, cancelToken.Token);

            Assert.NotNull(found);
            Assert.Equal(user.Email, found.Email);
            Assert.Equal(user.UserName, found.UserName);
            Assert.Equal(user.PasswordHash, found.PasswordHash);
            Assert.Equal(user.Name, found.Name);
            Assert.Equal(user.NormalizedName, found.NormalizedName);
            Assert.Equal(user.EmailConfirmed, found.EmailConfirmed);
            Assert.Equal(user.NormalizedEmail, found.NormalizedEmail);

            Assert.Equal("someone.email@gmail.com", found.Email);
            Assert.Equal("someone.email", found.UserName);
            Assert.Equal("randomepassword", found.PasswordHash);
            Assert.Equal("name", found.Name);
            Assert.Equal("USER", found.NormalizedName);
            Assert.True(found.EmailConfirmed);
            Assert.Equal("SOMEONE.EMAIL@GMAIL.COM", found.NormalizedEmail);

            AssertSucceeded(await userStore.DeleteAsync(found, cancelToken.Token));
        }

        [Fact]
        public async Task TwoUsersSamePasswordDifferentHash()
        {
            var serviceProvider = BuildServiceProvider();

            var manager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var userA = CreateTestUser();
            var userB = CreateTestUser();

            var result = await manager.CreateAsync(userA, "password");

            Assert.True(result.Succeeded);

            result = await manager.CreateAsync(userB, "password");

            Assert.True(result.Succeeded);

            Assert.NotEqual(userA.PasswordHash, userB.PasswordHash);

            AssertSucceeded(await manager.DeleteAsync(userA));
            AssertSucceeded(await manager.DeleteAsync(userB));
        }

        private void AssertSucceeded(IdentityResult result)
        {
            Assert.True(result.Succeeded);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            SetupIdentityServices(services);

            services
                .Configure((MongoDBOption option) =>
                {
                    option.ConnectionString = "mongodb://192.168.103.115:27017";
                    option.Database = "AspCoreIdentity";
                })
                .AddMongoDatabase()
                .AddMongoDbContext<MongoIdentityUser, MongoIdentityRole>()
                .AddMongoStore<MongoIdentityUser, MongoIdentityRole>()
                .AddTransient<IUserStore<MongoIdentityUser>, UserStore<MongoIdentityUser, MongoIdentityRole>>()
                .AddTransient<IUserClaimStore<MongoIdentityUser>, UserStore<MongoIdentityUser, MongoIdentityRole>>()
                .AddTransient<IUserPasswordStore<MongoIdentityUser>, UserStore<MongoIdentityUser, MongoIdentityRole>>()
                .AddTransient<IUserSecurityStampStore<MongoIdentityUser>, UserStore<MongoIdentityUser, MongoIdentityRole>>()
                .AddTransient<IUserEmailStore<MongoIdentityUser>, UserStore<MongoIdentityUser, MongoIdentityRole>>()
                .AddTransient<IUserLockoutStore<MongoIdentityUser>, UserStore<MongoIdentityUser, MongoIdentityRole>>()
                .AddTransient<IUserPhoneNumberStore<MongoIdentityUser>, UserStore<MongoIdentityUser, MongoIdentityRole>>()
                .AddTransient<IQueryableUserStore<MongoIdentityUser>, UserStore<MongoIdentityUser, MongoIdentityRole>>()
                .AddTransient<IUserAuthenticationTokenStore<MongoIdentityUser>, UserStore<MongoIdentityUser, MongoIdentityRole>>()
                .AddTransient<IUserTwoFactorStore<MongoIdentityUser>, UserStore<MongoIdentityUser, MongoIdentityRole>>()
                .AddTransient<IUserRoleStore<MongoIdentityUser>, UserStore<MongoIdentityUser, MongoIdentityRole>>();

            return services.BuildServiceProvider();
        }

        private UserManager<MongoIdentityUser> CreateManager()
        {
            var serviceProvider = BuildServiceProvider();

            return serviceProvider.GetService<UserManager<MongoIdentityUser>>();
        }

        private MongoIdentityUser CreateTestUser()
        {
            return new MongoIdentityUser
            {
                Email = $"someone.email${new Random().NextDouble()}@gmail.com",
                UserName = Guid.NewGuid().ToString(),
                Name = "name",
                EmailConfirmed = true
            };
        }

        private void SetupIdentityServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDataProtection();

            var builder = services.AddIdentity<MongoIdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.AllowedUserNameCharacters = null;
            }).AddDefaultTokenProviders();

            services.AddLogging();
        }
    }
}