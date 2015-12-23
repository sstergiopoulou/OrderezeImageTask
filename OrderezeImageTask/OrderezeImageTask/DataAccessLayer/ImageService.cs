using System.Collections.Generic;
using OrderezeImageTask.Models;
using OrderezeImageTask.AzureLayer;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System;
using System.Data;
using OrderezeImageTask.Logging;

namespace OrderezeImageTask.DataAccessLayer
{
    public class ImageService : IImagesService
    {
        private BlobFunctions    _blobFunctions = new BlobFunctions();
        private ImageContext     _imageContext = new ImageContext();
        ILogger log = new Logger();

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
        public int AddNewImage(Image image, HttpPostedFileBase file)
        {
            try
            {
                image.ImagePath = _blobFunctions.UploadFileToBlob(image, file);
                _imageContext.Images.Add(image);
                _imageContext.SaveChanges();
                log.Information("Successfully add new image (ImageService:AddNewImage)");
            }
            catch (Exception ex)
            {
                log.Error(ex, "failure to add new image (imageservice:addnewimage)");
                throw;
            }
            return image.Id;
        }

        /// <summary>
        /// Deletes the Image with the supplied <paramref name="id"/> from the system 
        /// and deletes the file from the blob storage as well.
        /// </summary>
        public void DeleteImage(int id)
        {
            try
            {
                Image image = _imageContext.Images.Find(id);
                _imageContext.Images.Remove(image);
                _imageContext.SaveChanges();
                _blobFunctions.DeleteBlobFile(image.ImagePath);
                log.Information("Successfully deleted image (ImageService:DeleteImage)");
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failure to delete image (ImageService:DeleteImage)");
                throw;
            }
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
            try
            {
                Image imageToUpload = _imageContext.Images.Find(image.Id);
                imageToUpload.Name = image.Name;
                imageToUpload.Description = image.Description;
                image = imageToUpload;
                _imageContext.SaveChanges();
                log.Information("Successfully edited image (ImageService:EditImage)");
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failure edited image (ImageService:EditImage)");
                throw;
            }
        }

        //Search
        public IQueryable <Image> SearchImage(string searchString)
        {
            var images = from s in _imageContext.Images
                         select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                images = images.Where(s => s.Name.Contains(searchString));
            }
            images = images.OrderBy(s => s.Name);
            return images;
        }


    }
}