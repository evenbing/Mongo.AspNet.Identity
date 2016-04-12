/*
    Copyright 2015 Matías Fidemraizer (https://linkedin.com/in/mfidemraizer)
    
    "Mongo.AspNet.Identity" project (https://github.com/mfidemraizer/Mongo.AspNet.Identity)
    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
 
    You may obtain a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0
    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

namespace Mongo.AspNet.Identity
{
    using Microsoft.AspNet.Identity;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract partial class MongoUserStore<TUserId, TUser> : IUserRoleStore<TUser>
    {
        public async Task AddToRoleAsync(TUser user, string roleName)
        {
            IMongoCollection<ExtenderUser<TUserId>> userRoleCollection = GetCollection<ExtenderUser<TUserId>>(UserCollectionName);
            ExtenderUser<TUserId> userRoleHolder = await FindExtendedUserByIdAsync(((IIdentityUser<TUserId>)user).Id, f => f.Project("Roles"));

            if (userRoleHolder.Roles.Add(roleName))
            {
                await userRoleCollection.UpdateOneAsync
                (
                    Builders<ExtenderUser<TUserId>>.Filter.Eq("Id", ((IIdentityUser<TUserId>)user).Id),
                    Builders<ExtenderUser<TUserId>>.Update.Set("Roles", userRoleHolder.Roles),
                    new UpdateOptions { IsUpsert = true }
                );
            }
        }

        public async Task<IList<string>> GetRolesAsync(TUser user)
        {
            IMongoCollection<ExtenderUser<TUserId>> userRoleCollection = GetCollection<ExtenderUser<TUserId>>(UserCollectionName);
            ExtenderUser<TUserId> userRoleHolder = await FindExtendedUserByIdAsync(((IIdentityUser<TUserId>)user).Id, f => f.Project("Roles"));

            return userRoleHolder.Roles != null ? userRoleHolder.Roles.ToList() : new List<string>();
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            IMongoCollection<ExtenderUser<TUserId>> userRoleCollection = GetCollection<ExtenderUser<TUserId>>(UserCollectionName);

            return await userRoleCollection
                    .Find
                    (
                        Builders<ExtenderUser<TUserId>>.Filter.And
                        (
                            Builders<ExtenderUser<TUserId>>.Filter.Eq("Id", ((IIdentityUser<TUserId>)user).Id),
                            Builders<ExtenderUser<TUserId>>.Filter.ElemMatch("Roles", Builders<string>.Filter.Eq("role", roleName))
                        )
                    ).CountAsync() > 0;
        }

        public async Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            IMongoCollection<ExtenderUser<TUserId>> userRoleCollection = GetCollection<ExtenderUser<TUserId>>(UserCollectionName);
            ExtenderUser<TUserId> userRoleHolder = await FindExtendedUserByIdAsync(((IIdentityUser<TUserId>)user).Id, f => f.Project("Roles"));

            if (userRoleHolder.Roles.Remove(roleName))
            {
                await userRoleCollection.UpdateOneAsync
                (
                    Builders<ExtenderUser<TUserId>>.Filter.Eq("Id", ((IIdentityUser<TUserId>)user).Id),
                    Builders<ExtenderUser<TUserId>>.Update.Set("Roles", userRoleHolder.Roles),
                    new UpdateOptions { IsUpsert = true }
                );
            }
        }
    }
}