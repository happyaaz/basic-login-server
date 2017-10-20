using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BasicLoginServer.Helpers
{
    public class ImageService
    {
        public string UploadImage(HttpPostedFileBase imageToUpload)
        {
            string imageFullPath = null;
            string imageName = null;
            if (imageToUpload == null || imageToUpload.ContentLength == 0)
            {
                return null;
            }
            try
            {
                CloudStorageAccount cloudStorageAccount = StorageConnectionString.GetConnectionString();
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                //  as usual, depending on where the user came from, container is gonna be different
                string userEmployer = GetCurrentClaimValues.GetCurrentUserEmployer();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(CommonPaths.ReturnContainerPath () + userEmployer);

                if (cloudBlobContainer.CreateIfNotExists())
                {
                    cloudBlobContainer.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        }
                        );
                }
                imageName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(imageToUpload.FileName);
                
                Console.WriteLine(imageName);

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);
                cloudBlockBlob.Properties.ContentType = imageToUpload.ContentType;
                cloudBlockBlob.UploadFromStream(imageToUpload.InputStream);

                imageFullPath = cloudBlockBlob.Uri.ToString();

                //  here's where I should put the shit into the database with the respective user
                Console.WriteLine(imageFullPath);
            }
            catch (Exception ex)
            {
                return null;
            }
            return imageName;
            //return imageFullPath;
        }



        public void DeleteImageFromBlob(string imageFullPath)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = StorageConnectionString.GetConnectionString();
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                //  as usual, depending on where the user came from, container is gonna be different
                string userEmployer = GetCurrentClaimValues.GetCurrentUserEmployer();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(CommonPaths.ReturnContainerPath() + userEmployer);

                var splitPath = imageFullPath.Split('/');

                CloudBlockBlob _blockBlob = cloudBlobContainer.GetBlockBlobReference(splitPath[splitPath.Length - 1]);
                //delete blob from container    
                _blockBlob.Delete();
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
                return;
            }
        }
    }
}