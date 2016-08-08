using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Dnx.Identity.MongoDB.Models;

namespace Dnx.Identity.MongoDB
{
    public class MongoRoleStore<TRole> : IRoleStore<TRole> where TRole : MongoRole
    {
        private readonly IMongoCollection<TRole> _roles;
        private bool _disposed;
        public MongoRoleStore(IMongoDatabase database, ILoggerFactory loggerFactory)
        {
            _roles = database.GetCollection<TRole>("roles");
        }

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));

            try
            {
                await _roles.InsertOneAsync(role,null, cancellationToken);

            }
            catch (Exception e)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = e.Message
                    }
                );
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null) throw new ArgumentNullException(nameof(role));

            try
            {
                DeleteResult result = await _roles.DeleteOneAsync(r => r.Id == role.Id);

                if (result.DeletedCount == 1)
                {
                    return IdentityResult.Success;

                }
                else
                {
                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = $"The role {role.Name} (ID: {role.Id}) could not be deleted."
                        }
                    );
                }

            }
            catch (Exception e)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = e.Message
                    }
                );
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
           return _roles.Find(x => x.Id == roleId).FirstOrDefaultAsync();
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return _roles.Find(x => x.Name == normalizedRoleName).FirstOrDefaultAsync();
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name); ;
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (String.IsNullOrWhiteSpace(normalizedName)) throw new ArgumentNullException(nameof(normalizedName));

            role.Name = normalizedName;

            return Task.FromResult(0);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (String.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

            role.Name = roleName;

            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));

            try
            {
                ReplaceOneResult result = await _roles.ReplaceOneAsync(r => r.Id == role.Id, role, cancellationToken: cancellationToken);

                if (result.ModifiedCount == 1)
                {
                    return IdentityResult.Success;

                }
                else
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = $"The role {role.Name} (ID: {role.Id}) could not be updated."
                    });
                }

            }
            catch (Exception e)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = e.Message
                    }
                );
            }
        }
    }

}
