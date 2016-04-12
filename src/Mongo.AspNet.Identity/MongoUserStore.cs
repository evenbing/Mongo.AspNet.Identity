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
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using System;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Threading;

    public abstract partial class MongoUserStore<TUserId, TUser>
        where TUserId : IEquatable<TUserId>
        where TUser : class, IUser, IIdentityUser<TUserId>
    {
        private readonly AutoResetEvent _configEvent = new AutoResetEvent(true);
        private readonly MongoClient _client;

        private const string RequiredSettingFormat = "'{0}' setting is missing or empty in current application configuration";

        public MongoUserStore(MongoClient client)
        {
            Contract.Requires(client != null, "A client is mandatory");
            Contract.Assert(!string.IsNullOrEmpty(DatabaseName), string.Format(RequiredSettingFormat, "aspNet:identity:mongo:databaseName"));
            _client = client;
        }

        private MongoClient Client { get { return _client; } }
        public string DatabaseName { get { return ConfigurationManager.AppSettings["aspNet:identity:mongo:databaseName"]; } }
        private static bool AlreadyConfigured { get; set; }

        private IMongoCollection<TDocument> GetCollection<TDocument>(string name)
        {
            return Client.GetDatabase(DatabaseName).GetCollection<TDocument>(name);
        }

        protected abstract TUserId ConvertUserIdFromString(string userId);
    }
}