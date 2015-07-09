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
    using System.Reflection;

    public partial class MongoUserStore<TUser>
        where TUser : class, IUser, IIdentityUser
    {
        private readonly MongoClient _client;
        private readonly Action<BsonClassMap<TUser>> _genericUserMapper;

        private const string RequiredSettingFormat = "'{0}' setting is missing or empty in current application configuration";

        public MongoUserStore(MongoClient client, Action<BsonClassMap<TUser>> genericUserMapper)
        {
            Contract.Assert(!string.IsNullOrEmpty(DatabaseName), string.Format(RequiredSettingFormat, "mongo:aspnetidentity:databaseName"));

            _genericUserMapper = genericUserMapper;
            _client = client;

            CreateClassMaps();
        }

        private MongoClient Client { get { return _client; } }
        public string DatabaseName { get { return ConfigurationManager.AppSettings["mongo:aspnetidentity:databaseName"]; } }

        private IMongoCollection<TDocument> GetCollection<TDocument>(string name)
        {
            return Client.GetDatabase(DatabaseName).GetCollection<TDocument>(name);
        }

        private void CreateClassMaps()
        {
            BsonClassMap.RegisterClassMap<IUser>
            (
                map =>
                {
                    map.SetIsRootClass(true);
                    map.AddKnownType(typeof(IIdentityUser));
                    map.AddKnownType(typeof(TUser));
                    map.AddKnownType(typeof(ExtendedUser));

                    map.MapMember(user => user.Id).SetElementName("id");
                    map.MapMember(user => user.UserName).SetElementName("name");
                }
            );

            BsonClassMap.RegisterClassMap<IIdentityUser>
            (
                map =>
                {
                    map.AddKnownType(typeof(IUser));
                    map.AddKnownType(typeof(TUser));
                    map.AddKnownType(typeof(ExtendedUser));

                    map.MapMember(user => user.Id).SetElementName("id");
                    map.MapMember(user => user.Email).SetElementName("email");
                    map.MapMember(user => user.PasswordHash).SetElementName("passwordHash");
                    map.MapMember(user => user.PhoneNumber).SetElementName("phoneNumber");
                    map.MapMember(user => user.SecurityStamp).SetElementName("securityStamp");
                }
            );

            BsonClassMap<TUser> genericUserMap = BsonClassMap.RegisterClassMap<TUser>
            (
                map =>
                {
                    map.AddKnownType(typeof(IUser));
                    map.AddKnownType(typeof(IIdentityUser));
                    map.AddKnownType(typeof(ExtendedUser));

                    map.MapMember(typeof(TUser).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)).SetElementName("id");
                    map.MapMember(user => user.Email).SetElementName("email");
                    map.MapMember(user => user.PasswordHash).SetElementName("passwordHash");
                    map.MapMember(user => user.PhoneNumber).SetElementName("phoneNumber");
                    map.MapMember(user => user.SecurityStamp).SetElementName("securityStamp");

                    if (_genericUserMapper != null)
                        _genericUserMapper(map);
                }
            );

            BsonClassMap.RegisterClassMap<ExtendedUser>
            (
                map =>
                {
                    map.AddKnownType(typeof(IUser));
                    map.AddKnownType(typeof(IIdentityUser));
                    map.AddKnownType(typeof(TUser));

                    map.MapMember(user => user.Id).SetElementName("id");
                    map.MapMember(user => user.PhoneNumberConfirmed).SetElementName("phoneNumberConfirmed");
                    map.MapMember(user => user.TwoFactorAuthenticationEnabled).SetElementName("twoFactorAuthenticationEnabled");
                    map.MapMember(user => user.Roles).SetElementName("roles");
                    map.MapMember(user => user.Logins).SetElementName("logins");
                }
            );

            BsonClassMap.RegisterClassMap<UserLoginInfo>
            (
                map =>
                {
                    map.MapMember(user => user.LoginProvider).SetElementName("provider");
                    map.MapMember(user => user.ProviderKey).SetElementName("key");
                }
            );
        }
    }
}