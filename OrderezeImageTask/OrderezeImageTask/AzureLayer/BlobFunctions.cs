using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using OrderezeImageTask.Models;


namespace OrderezeImageTask.AzureLayer
{
    public class BlobFunctions
    {

        public CloudBlobClient BlobClientConnect(string connstring)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings[connstring].ToString());
            // Create and return the blob client.
            return storageAccount.CreateCloudBlobClient();
        }

        public CloudBlobContainer BlobGetContainerRef(CloudBlobClient blobclient, string containerName)
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
            return container;
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

        public string UploadFileToBlob(Image imageforupload)
        {
            var blobcontainer = BlobGetContainerRef(BlobClientConnect("StorageConnectionString"), "imagecontainer");
            var blob = BlobGetBlobRef(blobcontainer, Guid.NewGuid().ToString() + Path.GetExtension(imageforupload.ImagePath));
            // Create or overwrite the blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(imageforupload.ImagePath))
            {
                blob.UploadFromStream(fileStream);
            }

            blob.Properties.ContentType = System.Web.MimeMapping.GetMimeMapping(imageforupload.ImagePath);
            blob.SetProperties();
            // Return a URI for viewing the photo
            return blob.Uri.ToString();
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
            var blobcontainer = BlobGetContainerRef(BlobClientConnect("StorageConnectionString"), "imagecontainer");
            var blob = BlobGetBlobRef(blobcontainer, Path.GetFileName(bloburi));
            // Delete the blob if exists.
            blob.DeleteIfExists();
        }
    }
}