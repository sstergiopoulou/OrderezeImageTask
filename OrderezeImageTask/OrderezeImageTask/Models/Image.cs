using System.ComponentModel.DataAnnotations;

namespace OrderezeImageTask.Models
{
    public class Image
    {
        /// <summary>
        /// The Id of the entity
        /// </summary>
        /// 
        public int Id { get; set; }

        /// <summary>
        /// The name of the image. It can be different than actual file name.
        /// </summary>
        [Required, Display(Name = "Name"), 
         StringLength(100, MinimumLength = 3, ErrorMessage = "Image name must be between 3 and 100 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// The description of the image
        /// </summary>
        [Required, Display(Name = "Image Description"), DataType(DataType.MultilineText),
         StringLength(2000, ErrorMessage = "Image description cannot be longer than 2000 characters.")]
        public string Description { get; set; }

        /// <summary>
        /// The path the actual image is stored (normally the blob storage reference)
        /// </summary>
        [Required, Display(Name = "ImagePath"),
         StringLength(1000, ErrorMessage = "Image path cannot be longer than 1000 characters.")]
        public string ImagePath { get; set; }
    }
}