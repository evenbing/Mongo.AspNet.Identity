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
    using System.Threading.Tasks;

    public partial class MongoUserStore<TUser> : IUserPhoneNumberStore<TUser>
    {
        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public async Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            IMongoCollection<ExtendedUser> userCollection = GetCollection<ExtendedUser>(UserCollectionName);

            ExtendedUser extendedUser = await userCollection.Find(Builders<ExtendedUser>.Filter.Eq(extUser => extUser.Id, ((IUser)user).Id))
                                                    .SingleAsync();

            return extendedUser.PhoneNumberConfirmed;
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            IMongoCollection<TUser> userCollection = GetCollection<TUser>(UserCollectionName);

            return userCollection.UpdateOneAsync
            (
                Builders<TUser>.Filter.Eq(this.userIdPropertySelector, ((IUser)user).Id),
                Builders<TUser>.Update.Set(someUser => someUser.PhoneNumber, phoneNumber)
            );
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            IMongoCollection<ExtendedUser> userCollection = GetCollection<ExtendedUser>(UserCollectionName);

            return userCollection.UpdateOneAsync
            (
                Builders<ExtendedUser>.Filter.Eq(extUser => extUser.Id, ((IUser)user).Id),
                Builders<ExtendedUser>.Update.Set(extUser => extUser.PhoneNumberConfirmed, confirmed)
            );
        }
    }
}