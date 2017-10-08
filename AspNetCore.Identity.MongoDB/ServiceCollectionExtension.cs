using AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection ConfigureMongoDbOption(this IServiceCollection services, Action<MongoDBOption> configure)
        {
            services.Configure(configure);

            return services;
        }

        public static IServiceCollection AddMongoDatabase(this IServiceCollection services)
        {
            services.AddTransient(provider =>
             {
                 var options = provider.GetService<IOptions<MongoDBOption>>();
                 var client = new MongoClient(options.Value.ConnectionString);
                 var database = client.GetDatabase(options.Value.Database);

                 return database;
             });

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TUser, TRole>(this IServiceCollection services)
            where TUser : MongoIdentityUser
            where TRole : MongoIdentityRole
        {
            services.AddTransient<IMongoDBDbContext<TUser, TRole>, MongoDbContext<TUser, TRole>>();

            return services;
        }

        public static IServiceCollection AddMongoStore<TUser, TRole>(this IServiceCollection services)
            where TUser : MongoIdentityUser
            where TRole : MongoIdentityRole
        {
            services.AddTransient<IUserStore<TUser>, UserStore<TUser, TRole>>();
            services.AddTransient<IRoleStore<TRole>, RoleStore<TUser, TRole>>();


            return services;
        }
    }
}
