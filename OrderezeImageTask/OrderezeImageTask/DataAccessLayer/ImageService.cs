using System.Collections.Generic;
using OrderezeImageTask.Models;
using OrderezeImageTask.AzureLayer;

namespace OrderezeImageTask.DataAccessLayer
{
    public class ImageService : IImagesService
    {
        BlobFunctions    blf = new BlobFunctions();
        ImageDBFunctions dbf = new ImageDBFunctions();

        /// <summary>
        /// Returns all images 
        /// </summary>
        public List<Image> GetImages()
        {
            return dbf.getImagesFromDB();
        }

        /// <summary>
        /// Adds the supplied <paramref name="image"/> to the system and returns the Id.
        /// Store the Image in the blob storage.
        /// </summary>
        public int AddNewImage(Image image)
        {
            image.ImagePath = blf.uploadFileToBlob(image);
            image.Id = dbf.insertImageToDb(image);

            return image.Id;
        }

        /// <summary>
        /// Deletes the Image with the supplied <paramref name="id"/> from the system 
        /// and deletes the file from the blob storage as well.
        /// </summary>
        public void DeleteImage(int id)
        {
            blf.deleteBlobFile(dbf.deleteImagefromDB(id));
        }
    }
}