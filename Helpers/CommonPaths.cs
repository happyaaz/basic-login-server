using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasicLoginServer.Helpers
{
    public class CommonPaths
    {
        private const string IMAGE_SERVICE_CONTAINER_PATH = "projectName-container-";
        private const string PATH_TO_BLOB = "https://projectName.blob.core.windows.net/projectName-container-";
        private const string CONNECTION_STRING = "projectName-";

        public static string ReturnContainerPath ()
        {
            return IMAGE_SERVICE_CONTAINER_PATH;
        }

        public static string RerurnPathToBlob()
        {
            return PATH_TO_BLOB;
        }

        public static string RerurnConnectionString()
        {
            return CONNECTION_STRING;
        }
    }
}