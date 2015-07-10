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

    public partial class MongoUserStore<TUser> : IUserEmailStore<TUser>
    {
        public Task<TUser> FindByEmailAsync(string email)
        {
            IMongoCollection<TUser> userCollection = GetCollection<TUser>(UserCollectionName);

            return userCollection.Find(Builders<TUser>.Filter.Eq(user => user.Email, email)).SingleOrDefaultAsync();
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(user.Email);
        }

        public async Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            ExtendedUser extendedUser = await FindExtendedUserByIdAsync(((IUser)user).Id, p => p.Project(extUser => extUser.EmailConfirmed));

            return extendedUser.EmailConfirmed;
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            user.Email = email;

            return Task.FromResult(true);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            IMongoCollection<ExtendedUser> userCollection = GetCollection<ExtendedUser>(UserCollectionName);

            return userCollection.UpdateOneAsync
            (   
                Builders<ExtendedUser>.Filter.Eq(extUser => extUser.Id, ((IUser)user).Id),
                Builders<ExtendedUser>.Update.Set(extUser => extUser.EmailConfirmed, confirmed)
            );
        }
    }
}