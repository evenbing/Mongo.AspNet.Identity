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

    public partial class MongoUserStore<TUser> : IUserLockoutStore<TUser, string>
    {
        public async Task<int> GetAccessFailedCountAsync(TUser user)
        {
            ExtendedUser extendedUser = await FindExtendedUserByIdAsync(((IUser)user).Id);

            return extendedUser.AccessFailedCount;
        }

        public async Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            ExtendedUser extendedUser = await FindExtendedUserByIdAsync(((IUser)user).Id);

            return extendedUser.LockoutEnabled;
        }

        public async Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            ExtendedUser extendedUser = await FindExtendedUserByIdAsync(((IUser)user).Id);

            return extendedUser.LockoutEndDate;
        }

        public async Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            ExtendedUser extendedUser = await FindExtendedUserByIdAsync(((IUser)user).Id);

            extendedUser.AccessFailedCount++;

            IMongoCollection<ExtendedUser> userCollection = GetCollection<ExtendedUser>(UserCollectionName);

            await userCollection.UpdateOneAsync
            (
                Builders<ExtendedUser>.Filter.Eq(extUser => extUser.Id, ((IUser)user).Id),
                Builders<ExtendedUser>.Update.Set(extUser => extUser.AccessFailedCount, extendedUser.AccessFailedCount)
            );

            return extendedUser.AccessFailedCount;
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            IMongoCollection<ExtendedUser> userCollection = GetCollection<ExtendedUser>(UserCollectionName);

            return userCollection.UpdateOneAsync
            (
                Builders<ExtendedUser>.Filter.Eq(extUser => extUser.Id, ((IUser)user).Id),
                Builders<ExtendedUser>.Update.Set(extUser => extUser.AccessFailedCount, 0)
            );
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            IMongoCollection<ExtendedUser> userCollection = GetCollection<ExtendedUser>(UserCollectionName);

            return userCollection.UpdateOneAsync
            (
                Builders<ExtendedUser>.Filter.Eq(extUser => extUser.Id, ((IUser)user).Id),
                Builders<ExtendedUser>.Update.Set(extUser => extUser.LockoutEnabled, enabled)
            );
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            IMongoCollection<ExtendedUser> userCollection = GetCollection<ExtendedUser>(UserCollectionName);

            return userCollection.UpdateOneAsync
            (
                Builders<ExtendedUser>.Filter.Eq(extUser => extUser.Id, ((IUser)user).Id),
                Builders<ExtendedUser>.Update.Set(extUser => extUser.LockoutEndDate, lockoutEnd)
            );
        }
    }
}