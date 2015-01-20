﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ApiClient.Utilities;
using VirtoCommerce.Web.Core.Configuration.Catalog;

namespace VirtoCommerce.ApiClient.Extensions
{
    using System.Configuration;

    public static class BrowseClientExtension
    {
        public static BrowseClient CreateBrowseClient(this CommerceClients source)
        {
            return source.CreateBrowseClient(ClientContext.Session.CatalogId, ClientContext.Session.Language);
        }

        public static BrowseClient CreateBrowseClient(this CommerceClients source, string catalogId, string language)
        {
            // http://localhost/admin/api/mp/{0}/{1}/
            var connectionString = ClientContext.Configuration.ConnectionString + String.Format("{0}/{1}/", catalogId, language);
            return CreateBrowseClientWithUri(source, connectionString);
        }

        public static BrowseClient CreateBrowseClientWithUri(this CommerceClients source, string serviceUrl)
        {
            var connectionString = serviceUrl;
            var subscriptionKey = ConfigurationManager.AppSettings["vc-public-apikey"];
            var client = new BrowseClient(new Uri(connectionString), new AzureSubscriptionMessageProcessingHandler(subscriptionKey, "secret"));
            return client;
        }
 
    }
}
