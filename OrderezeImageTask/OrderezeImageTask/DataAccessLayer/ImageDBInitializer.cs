using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderezeImageTask.Models;

namespace OrderezeImageTask.DataAccessLayer
{
    public class ImageDBInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ImageContext>   
    {
        //Initializes DB with sample data, only for developement status
        protected override void Seed(ImageContext context)
        {
            var img = new List<Image> 
            {
                new Image { Id = 1, Name = "Img1_Sea", Description = "Image of the sea", ImagePath="../TestImages/img1_sea.jpg"},
                new Image { Id = 2, Name = "Img2_autumn", Description = "Image depicting a forest during autumn", ImagePath="../TestImages/img2_autumn.jpg"},
                new Image { Id = 3, Name = "Img3_winter", Description = "Image depicting a lake during winter", ImagePath="../TestImages/img3_winter.jpg"}
            };

            img.ForEach(i => context.Images.Add(i));
            context.SaveChanges();
        }

    }

}