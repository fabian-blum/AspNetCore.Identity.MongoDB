# AspNetCore.Identity.MongoDB
MongoDB data store for AspNetCore.Identity 2.0

# Nuget Package
https://www.nuget.org/packages/FB.AspNetCore.Identity.MongoDB

# How to use
You have to add the following lines to your Startup.cs in the ASP.NET Core Project.
```C#
public void ConfigureServices(IServiceCollection services)
{
services.AddIdentity<ApplicationUser, MongoIdentityRole>()
                .AddDefaultTokenProviders();

            services
                .Configure<MongoDBOption>(Configuration.GetSection("MongoDBOption"))
                .AddMongoDatabase()
                .AddMongoDbContext<ApplicationUser, MongoIdentityRole>()
                .AddMongoStore<ApplicationUser, MongoIdentityRole>();

}
```

Also make sure you use the right inheritance in the Models class ApplicationUser.

```C#
public class ApplicationUser : MongoIdentityUser
    {
    }
```

In appsettings.json you can configure the correct Path to your MongoDB instance.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=xxxx"
  },
  "MongoDBOption": {
    "ConnectionString": "mongodb://192.168.103.115:27017",
    "Database": "AspCoreIdentity",
    "User": {
      "CollectionName": "Users",
      "ManageIndicies": true
    },
    "Role": {
      "CollectionName": "Roles",
      "ManageIndicies": true
    }
  },
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  }
}

```
