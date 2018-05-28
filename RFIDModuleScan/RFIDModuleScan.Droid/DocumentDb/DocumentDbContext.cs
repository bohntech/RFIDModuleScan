using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace RFIDModuleScan.Droid.DocumentDb
{
    /// <summary>
    /// This class is a singleton static class adapted from the Azure Cosmos DB starter samples
    /// It is used to hold a single client session open for the application to share.  This
    /// enables session consistency so a single client will always see it's own updates.
    /// </summary>
    public static class DocumentDBContext
    {
        public static string DatabaseId = "CottonModuleDB";
        public static string CollectionId = "CottonDocs";

        private static DocumentClient client;

        public static bool Initialized
        {
            get
            {
                return client != null;
            }
        }

        /*public static async Task<T> GetItemAsync<T>(string id)
        {
            try
            {
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return default(T);
                }
                else
                {
                    throw;
                }
            }
        }*/

        public static IEnumerable<T> GetAllItems<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            IQueryable<T> query = client.CreateDocumentQuery<T>(
               UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
               .Where(predicate);

            return query.ToList();
        }

        public static IEnumerable<T> GetAllItems<T>() where T : class
        {
            IQueryable<T> query = client.CreateDocumentQuery<T>(
               UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));

            return query.ToList();
        }

        public static void Initialize(string endpoint, string authKey)
        {

            var policy = new ConnectionPolicy
            {
                //ConnectionMode = ConnectionMode.Direct,
                //ConnectionProtocol = Protocol.Https,
                EnableEndpointDiscovery = false
            };

            client = new DocumentClient(new Uri(endpoint), authKey,
                policy, ConsistencyLevel.Eventual);           
        }       
    }
}