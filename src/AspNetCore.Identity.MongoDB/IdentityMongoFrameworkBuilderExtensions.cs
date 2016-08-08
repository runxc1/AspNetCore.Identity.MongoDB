using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dnx.Identity.MongoDB
{

    public static class IdentityMongoFrameworkBuilderExtensions
    {
        /// <summary>
        /// Adds an Entity Framework implementation of identity information stores.
        /// </summary>
        /// <typeparam name="TContext">The Entity Framework database context to use.</typeparam>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddMongoStores(this IdentityBuilder builder)
        {
            builder.Services.TryAdd(GetDefaultServices(builder.UserType, builder.RoleType));
            return builder;
        }

        public static IdentityBuilder AddMongoStores(this IdentityBuilder builder, IMongoDatabase mongoDatabase)
        {
            //IMongoDatabase
            
            builder.Services.TryAdd(GetDefaultServices(builder.UserType, builder.RoleType, mongoDatabase));
            return builder;
        }


        private static IServiceCollection GetDefaultServices(Type userType, Type roleType, IMongoDatabase userDB = null)
        {
            Type userStoreType;
            Type roleStoreType;
           // keyType = keyType ?? typeof(string);
            userStoreType = typeof(MongoUserStore<>).MakeGenericType(userType);
            roleStoreType = typeof(MongoRoleStore<>).MakeGenericType(roleType);

            var services = new ServiceCollection();
            if (userDB != null)
                services.AddSingleton<IMongoDatabase>(userDB);
            services.AddScoped(
                typeof(IUserStore<>).MakeGenericType(userType),
                userStoreType);
            services.AddScoped(
                typeof(IRoleStore<>).MakeGenericType(roleType),
                roleStoreType);
            return services;
        }
    }
}
