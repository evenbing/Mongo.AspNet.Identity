namespace Mongo.AspNet.Identity.OAuth
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using MongoDB.Driver;
    using System;
    using System.Collections.Immutable;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    public class OAuthClientStore<TClient> : IDisposable
        where TClient : IOAuthClient, new()
    {
        private readonly MongoClient _client;

        public OAuthClientStore(MongoClient client)
        {
            Contract.Requires(client != null, "A client is mandatory");

            Contract.Assert(!string.IsNullOrEmpty(DatabaseName), "Either 'aspNet:identity:oauth:mongo:databaseName' application setting is missing or empty'");
            Contract.Assert(!string.IsNullOrEmpty(ClientCollectionName), "Either 'aspNet:identity:oauth:mongo:clientCollectionName' application setting is missing or empty'");

            _client = client;
        }

        private MongoClient ConnectionMultiplexer { get { return _client; } }
        private string DatabaseName { get { return ConfigurationManager.AppSettings["aspNet:identity:oauth:mongo:databaseName"]; } }
        private string ClientCollectionName { get { return ConfigurationManager.AppSettings["aspNet:identity:oauth:mongo:clientCollectionName"]; } }

        protected virtual IMongoCollection<TClient> ClientCollection
        {
            get { return Database.GetCollection<TClient>(ClientCollectionName); }
        }

        protected virtual IMongoDatabase Database
        {
            get { return _client.GetDatabase(DatabaseName); }
        }

        public virtual async Task<TClient> RegisterClientAsync(string ownerUserName, string name, OAuthGrantType grantType)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));

            TClient client = new TClient();

            client.GrantType = grantType;

            using (RijndaelManaged cryptoManager = new RijndaelManaged())
            {
                cryptoManager.GenerateKey();
                client.Id = BitConverter.ToString(cryptoManager.Key).Replace("-", string.Empty).ToLowerInvariant();

                cryptoManager.GenerateKey();
                client.Secret = BitConverter.ToString(cryptoManager.Key).Replace("-", string.Empty).ToLowerInvariant();
            }

            client.OwnerUserName = ownerUserName;
            client.Name = name;
            client.SecretHash = new PasswordHasher().HashPassword(client.Secret);
            client.DateAdded = DateTimeOffset.Now;

            await ClientCollection.InsertOneAsync(client);

            return client;
        }

        public virtual Task<TClient> GetClientByIdAsync(string id)
        {
            return ClientCollection.Find(Builders<TClient>.Filter.Eq("Id", id)).SingleAsync();
        }

        public virtual async Task<ImmutableHashSet<TClient>> GetAllClientsByOwnerUserNameAsync(string ownerUserName)
        {
            return (await ClientCollection.Find(client => client.OwnerUserName == ownerUserName)
                        .ToListAsync())
                        .ToImmutableHashSet(new OAuthClient.OAuthClientEqualityComparer<TClient>());
        }

        public void Dispose()
        {
        }
    }
}