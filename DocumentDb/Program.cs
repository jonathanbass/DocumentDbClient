using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace DocumentDb
{
    internal class Program
    {
        private static string _documentDbEndpoint;
        private static string _documentDbAuthKey;
        private static string _documentDbCollection;
        private static string _testDatabaseName;

        private static void Main()
        {
            _documentDbEndpoint = ConfigurationManager.AppSettings["DocumentDbEndpoint"];
            _documentDbAuthKey = ConfigurationManager.AppSettings["DocumentDbAuthKey"];
            _documentDbCollection = ConfigurationManager.AppSettings["DocumentDbAuditCollection"];
            _testDatabaseName = Guid.NewGuid().ToString();

            CreateTestDatabase().Wait();
            TeardownTestDatabase().Wait();
        }

        private static async Task CreateTestDatabase()
        {
            using (var client = new DocumentClient(new Uri(_documentDbEndpoint), _documentDbAuthKey))
            {
                await client.CreateDatabaseAsync(new Database { Id = _testDatabaseName });

                var myCollection = new DocumentCollection { Id = _documentDbCollection };
                await client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(_testDatabaseName), myCollection);
            }
        }

        private static async Task TeardownTestDatabase()
        {
            using (var client = new DocumentClient(new Uri(_documentDbEndpoint), _documentDbAuthKey))
            {
                var database = await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_testDatabaseName));

                await client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(database.Resource.Id));
            }
        }
    }
}
