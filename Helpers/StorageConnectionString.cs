using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;

namespace BasicLoginServer.Helpers
{
    public class StorageConnectionString
    {
        static string account = CloudConfigurationManager.GetSetting("StorageAccountName");
        static string key = CloudConfigurationManager.GetSetting("StorageAccountKey");
        public static CloudStorageAccount GetConnectionString()
        {
            string connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net", account, key);
            return CloudStorageAccount.Parse(connectionString);
        }
    }
}