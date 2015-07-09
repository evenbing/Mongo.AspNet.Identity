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
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public partial class MongoUserStore<TUser> : IUserStore<TUser>
    {
        private readonly Expression<Func<TUser, string>> userIdPropertySelector = someUser => ((IIdentityUser)someUser).Id;

        public MongoUserStore()
        {
            Contract.Requires(!string.IsNullOrEmpty(UserCollectionName), string.Format(RequiredSettingFormat, "mongo:aspnetidentity:users:collectionName"));
        }

        public string UserCollectionName { get { return ConfigurationManager.AppSettings["mongo:aspnetidentity:users:collectionName"]; } }

        public Task CreateAsync(TUser user)
        {
            IMongoCollection<TUser> userCollection = GetCollection<TUser>(UserCollectionName);

            return userCollection.InsertOneAsync(user);
        }

        public Task DeleteAsync(TUser user)
        {
            IMongoCollection<TUser> userCollection = GetCollection<TUser>(UserCollectionName);

            return userCollection.DeleteOneAsync(Builders<TUser>.Filter.Eq(userIdPropertySelector, ((IIdentityUser)user).Id));
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            IMongoCollection<TUser> userCollection = GetCollection<TUser>(UserCollectionName);

            return userCollection.Find(Builders<TUser>.Filter.Eq(userIdPropertySelector, userId))
                        .SingleAsync();
        }

        public Task<ExtendedUser> FindExtendedUserByIdAsync(string userId, Action<IFindFluent<ExtendedUser, ExtendedUser>> projection = null)
        {
            IMongoCollection<ExtendedUser> userCollection = GetCollection<ExtendedUser>(UserCollectionName);

            IFindFluent<ExtendedUser, ExtendedUser> findFluent = userCollection.Find(Builders<ExtendedUser>.Filter.Eq(extUser => extUser.Id, userId));

            if (projection != null)
                projection(findFluent);

            return findFluent.SingleAsync();
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            IMongoCollection<TUser> userCollection = GetCollection<TUser>(UserCollectionName);

            return userCollection.Find(Builders<TUser>.Filter.Eq(someUser => someUser.UserName, userName))
                        .SingleAsync();
        }

        public Task UpdateAsync(TUser user)
        {
            IMongoCollection<TUser> userCollection = GetCollection<TUser>(UserCollectionName);

            return userCollection.ReplaceOneAsync
            (
                Builders<TUser>.Filter.Eq(userIdPropertySelector, ((IIdentityUser)user).Id),
                user
            );
        }

        public void Dispose()
        {
        }

    }
}