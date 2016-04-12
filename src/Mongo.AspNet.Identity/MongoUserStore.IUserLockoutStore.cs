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
    using System;
    using System.Threading.Tasks;

    public abstract partial class MongoUserStore<TUserId, TUser> : IUserLockoutStore<TUser, string>
    {
        public async Task<int> GetAccessFailedCountAsync(TUser user)
        {
            ExtenderUser<TUserId> extendedUser = await FindExtendedUserByIdAsync(((IIdentityUser<TUserId>)user).Id, f => f.Project(u => u.AccessFailedCount));

            return extendedUser.AccessFailedCount;
        }

        public async Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            ExtenderUser<TUserId> extendedUser = await FindExtendedUserByIdAsync(((IIdentityUser<TUserId>)user).Id, f => f.Project(u => u.LockoutEnabled));

            return extendedUser.LockoutEnabled;
        }

        public async Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            ExtenderUser<TUserId> extendedUser = await FindExtendedUserByIdAsync(((IIdentityUser<TUserId>)user).Id, f => f.Project(u => u.LockoutEndDate));

            return extendedUser.LockoutEndDate;
        }

        public async Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            ExtenderUser<TUserId> extendedUser = await FindExtendedUserByIdAsync(((IIdentityUser<TUserId>)user).Id, f => f.Project(u => u.AccessFailedCount));

            extendedUser.AccessFailedCount++;

            IMongoCollection<ExtenderUser<TUserId>> userCollection = GetCollection<ExtenderUser<TUserId>>(UserCollectionName);

            await userCollection.UpdateOneAsync
            (
                Builders<ExtenderUser<TUserId>>.Filter.Eq("Id", ((IIdentityUser<TUserId>)user).Id),
                Builders<ExtenderUser<TUserId>>.Update.Set("AccessFailedCount", extendedUser.AccessFailedCount)
            );

            return extendedUser.AccessFailedCount;
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            IMongoCollection<ExtenderUser<TUserId>> userCollection = GetCollection<ExtenderUser<TUserId>>(UserCollectionName);

            return userCollection.UpdateOneAsync
            (
                Builders<ExtenderUser<TUserId>>.Filter.Eq("Id", ((IIdentityUser<TUserId>)user).Id),
                Builders<ExtenderUser<TUserId>>.Update.Set("AccessFailedCount", 0)
            );
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            IMongoCollection<ExtenderUser<TUserId>> userCollection = GetCollection<ExtenderUser<TUserId>>(UserCollectionName);

            return userCollection.UpdateOneAsync
            (
                Builders<ExtenderUser<TUserId>>.Filter.Eq(extUser => extUser.Id, ((IIdentityUser<TUserId>)user).Id),
                Builders<ExtenderUser<TUserId>>.Update.Set("LockoutEnabled", enabled)
            );
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            IMongoCollection<ExtenderUser<TUserId>> userCollection = GetCollection<ExtenderUser<TUserId>>(UserCollectionName);

            return userCollection.UpdateOneAsync
            (
                Builders<ExtenderUser<TUserId>>.Filter.Eq("Id", ((IIdentityUser<TUserId>)user).Id),
                Builders<ExtenderUser<TUserId>>.Update.Set("LockoutEndDate", lockoutEnd)
            );
        }
    }
}