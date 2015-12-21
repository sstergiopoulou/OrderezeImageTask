using System.Collections.Generic;
using OrderezeImageTask.Models;
using OrderezeImageTask.AzureLayer;
using System.Linq;
using System.Data.Entity;

namespace OrderezeImageTask.DataAccessLayer
{
    public class ImageService : IImagesService
    {
        private BlobFunctions    _blobFunctions = new BlobFunctions();
        private ImageContext     _imageContext = new ImageContext();

        /// <summary>
        /// Returns all images 
        /// </summary>
        public List<Image> GetImages()
        {
            return _imageContext.Images.ToList();
        }

        /// <summary>
        /// Adds the supplied <paramref name="image"/> to the system and returns the Id.
        /// Store the Image in the blob storage.
        /// </summary>
        public int AddNewImage(Image image)
        {
            image.ImagePath = _blobFunctions.UploadFileToBlob(image);
            _imageContext.Images.Add(image);
            _imageContext.SaveChanges();
            return image.Id;
        }

        /// <summary>
        /// Deletes the Image with the supplied <paramref name="id"/> from the system 
        /// and deletes the file from the blob storage as well.
        /// </summary>
        public void DeleteImage(int id)
        {
            Image image = _imageContext.Images.Find(id);
            _imageContext.Images.Remove(image);
            _imageContext.SaveChanges();
            _blobFunctions.DeleteBlobFile(image.ImagePath);
        }

        public Image FindImage(int? id)
        {
            Image image = _imageContext.Images.Find(id);
            return image;
        }

        /// <summary>
        /// Edits Image data
        /// </summary>
        public void EditImage(Image image)
        {
            _imageContext.Entry(image).State = EntityState.Modified;
            _imageContext.SaveChanges();
        }
    }
}