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

    public abstract partial class MongoUserStore<TUserId, TUser> : IUserLoginStore<TUser, string>
    {
        public async Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            IMongoCollection<ExtenderUser<TUserId>> userCollection = GetCollection<ExtenderUser<TUserId>>(UserCollectionName);

            ExtenderUser<TUserId> userRoleHolder = await userCollection
                                                    .Find(Builders<ExtenderUser<TUserId>>.Filter.Eq(holder => holder.Id, ((IIdentityUser<TUserId>)user).Id))
                                                    .SingleAsync();
            if (userRoleHolder.Logins.Add(login))
            {
                await userCollection.UpdateOneAsync
                (
                    Builders<ExtenderUser<TUserId>>.Filter.Eq(holder => holder.Id,((IIdentityUser<TUserId>)user).Id),
                    Builders<ExtenderUser<TUserId>>.Update.Set(holder => holder.Logins, userRoleHolder.Logins),
                    new UpdateOptions { IsUpsert = true }
                );
            }
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            IMongoCollection<TUser> userCollection = GetCollection<TUser>(UserCollectionName);

            return userCollection.Find
            (
                Builders<TUser>.Filter.And
                (
                    Builders<TUser>.Filter.AnyEq("logins.provider", login.LoginProvider),
                    Builders<TUser>.Filter.AnyEq("logins.key", login.ProviderKey)
                )

            ).SingleAsync();
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            ExtenderUser<TUserId> extendedUser = await FindExtendedUserByIdAsync
            (
                ((IIdentityUser<TUserId>)user).Id,
                find =>
                {
                    find.Project(extUser => extUser.Logins);
                }
            );

            return extendedUser.Logins.ToList();
        }

        public async Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            TUserId userId = ((IIdentityUser<TUserId>)user).Id;

            ExtenderUser<TUserId> extendedUser = await FindExtendedUserByIdAsync
            (
                userId,
                find =>
                {
                    find.Project(extUser => extUser.Logins);
                }
            );

            if(extendedUser.Logins.Remove(login))
            {
                IMongoCollection<ExtenderUser<TUserId>> userCollection = GetCollection<ExtenderUser<TUserId>>(UserCollectionName);

                await userCollection.UpdateOneAsync
                (
                    Builders<ExtenderUser<TUserId>>.Filter.Eq(extUser => extUser.Id, userId),
                    Builders<ExtenderUser<TUserId>>.Update.Set(extUser => extUser.Logins, extendedUser.Logins)
                );
            }
        }
    }
}