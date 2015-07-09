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

    public partial class MongoUserStore<TUser> : IUserRoleStore<TUser>
    {
        public async Task AddToRoleAsync(TUser user, string roleName)
        {
            IMongoCollection<ExtendedUser> userRoleCollection = GetCollection<ExtendedUser>(UserCollectionName);

            ExtendedUser userRoleHolder = await userRoleCollection
                                                    .Find(Builders<ExtendedUser>.Filter.Eq(holder => holder.Id, ((IUser)user).Id))
                                                    .SingleAsync();
            if (userRoleHolder.Roles.Add(roleName))
            {
                await userRoleCollection.UpdateOneAsync
                (
                    Builders<ExtendedUser>.Filter.Eq(holder => holder.Id, ((IUser)user).Id),
                    Builders<ExtendedUser>.Update.Set(holder => holder.Roles, userRoleHolder.Roles),
                    new UpdateOptions { IsUpsert = true }
                );
            }
        }

        public async Task<IList<string>> GetRolesAsync(TUser user)
        {
            IMongoCollection<ExtendedUser> userRoleCollection = GetCollection<ExtendedUser>(UserCollectionName);
            ExtendedUser userRoleHolder = await userRoleCollection
                                                    .Find(Builders<ExtendedUser>.Filter.Eq(holder => holder.Id, ((IUser)user).Id))
                                                    .SingleOrDefaultAsync();

            return userRoleHolder.Roles.ToList();
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            IMongoCollection<ExtendedUser> userRoleCollection = GetCollection<ExtendedUser>(UserCollectionName);

            return await userRoleCollection
                    .Find
                    (
                        Builders<ExtendedUser>.Filter.And
                        (
                            Builders<ExtendedUser>.Filter.Eq(holder => holder.Id, ((IUser)user).Id),
                            Builders<ExtendedUser>.Filter.ElemMatch(holder => holder.Roles, role => role == roleName)
                        )
                    ).SingleOrDefaultAsync() != null;
        }

        public async Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            IMongoCollection<ExtendedUser> userRoleCollection = GetCollection<ExtendedUser>(UserCollectionName);

            ExtendedUser userRoleHolder = await userRoleCollection
                                                    .Find(Builders<ExtendedUser>.Filter.Eq(holder => holder.Id, ((IUser)user).Id))
                                                    .SingleOrDefaultAsync();

            if (userRoleHolder.Roles.Remove(roleName))
            {
                await userRoleCollection.UpdateOneAsync
                (
                    Builders<ExtendedUser>.Filter.Eq(holder => holder.Id, ((IUser)user).Id),
                    Builders<ExtendedUser>.Update.Set(holder => holder.Roles, userRoleHolder.Roles),
                    new UpdateOptions { IsUpsert = true }
                );
            }
        }
    }
}