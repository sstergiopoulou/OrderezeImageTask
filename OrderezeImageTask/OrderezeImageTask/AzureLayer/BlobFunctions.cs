using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using OrderezeImageTask.Models;
using System.Web;
using OrderezeImageTask.Logging;

namespace OrderezeImageTask.AzureLayer
{
    public class BlobFunctions
    {
        ILogger log = null;

        public CloudBlobClient BlobClientConnect(string connstring)
        {
            try
            {
                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings[connstring].ToString());
                log.Information("Successfully retrieved storage account from connection string (BlobFunctions:BlobClientConnect)");
                // Create and return the blob client.
                return storageAccount.CreateCloudBlobClient();
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failure to retrieve storage account from connection string (BlobFunctions:BlobClientConnect)");
                throw;
            }
        }

        public CloudBlobContainer BlobGetContainerRef(CloudBlobClient blobclient, string containerName)
        {
            try
            {
                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobclient.GetContainerReference(containerName);
                // Create the container if it doesn't already exist.
                container.CreateIfNotExists();
                // By default, the new container is private, so we set set the container to be public
                container.SetPermissions(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Container
                    });
                log.Information("Successfully retrieved reference to a container and set permissions (BlobFunctions:BlobGetContainerRef)");
                return container;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failure to retrieve reference to a container and set permissions (BlobFunctions:BlobGetContainerRef)");
                throw;
            }          
        }

        public CloudBlockBlob BlobGetBlobRef(CloudBlobContainer container, string blobname)
        {
            // Retrieve reference to a blob named
            return container.GetBlockBlobReference(blobname);
        }

        public void SetPublicPermissions(CloudBlobContainer container, BlobContainerPublicAccessType Level)
        {
            container.SetPermissions(
                new BlobContainerPermissions
                {
                    PublicAccess = Level
                });
        }

        public void AddBlobMetadata(CloudBlockBlob blob, string metadataKey, string metadataValue)
        {
            blob.Metadata.Add(metadataKey, metadataValue);
        }

        public string UploadFileToBlob(Image imageforupload, HttpPostedFileBase file)
        {
            try
            {
                var blobcontainer = BlobGetContainerRef(BlobClientConnect("StorageConnectionString"), "imagecontainer");
                if (file != null)
                {
                    CloudBlockBlob blockBlob = blobcontainer.GetBlockBlobReference(file.FileName);
                    blockBlob.UploadFromStream(file.InputStream);
                    log.Information("Successfully uploaded image (BlobFunctions:UploadFileToBlob)");
                    // Return a URI for viewing the photo
                    return blockBlob.Uri.ToString();
                }
                else
                {
                    log.Error("EX-1", "Error in uploading photo");
                    return null;
                }  
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failure to upload image (BlobFunctions:UploadFileToBlob)");
                throw;
            }

        }

        public string UploadFromBytes(byte[] fileBytes, HttpPostedFileBase file)
        {
            var blobcontainer = BlobGetContainerRef(BlobClientConnect("StorageConnectionString"), "imagecontainer");
            // Create the container and blob.
            CloudBlockBlob blockBlob = blobcontainer.GetBlockBlobReference(file.FileName);

            // Set the content type to image
            blockBlob.Properties.ContentType = "image/" + Path.GetExtension(file.FileName).Replace(".", "");
            blockBlob.UploadFromByteArray(fileBytes, 0, fileBytes.Length - 1);

            // Return a URI fro viewing the photo
            return blockBlob.Uri.AbsoluteUri;
        }

        public List<Image> GetBlobFiles()
        {
            List<Image> imageList = new List<Image>();
            var blobcontainer = BlobGetContainerRef(BlobClientConnect("StorageConnectionString"), "imagecontainer");
            // Loop over items within the container and get image list
            foreach (var blobItem in blobcontainer.ListBlobs())
            {
                var aBlob = BlobGetBlobRef(blobcontainer, blobItem.Uri.AbsoluteUri);
                aBlob.FetchAttributes();

                imageList.Add(new Image()
                {
                    Id = int.Parse(aBlob.Metadata["id"]),
                    Name = aBlob.Metadata["name"],
                    Description = aBlob.Metadata["description"],
                    ImagePath = aBlob.Uri.AbsoluteUri
                });
            }
            return imageList;
        }

        public void DeleteBlobFile(string bloburi)
        {
            try
            {
                var blobcontainer = BlobGetContainerRef(BlobClientConnect("StorageConnectionString"), "imagecontainer");
                var blob = BlobGetBlobRef(blobcontainer, Path.GetFileName(bloburi));
                // Delete the blob if exists.
                blob.DeleteIfExists();
                log.Information("Successfully deleted blob (BlobFunctions:DeleteBlobFile)");
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failure to delete blob (BlobFunctions:DeleteBlobFile)");
                throw;
            }
        }
    }
}