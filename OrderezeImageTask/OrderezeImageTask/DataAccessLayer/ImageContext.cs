using System.Data.Entity;
using OrderezeImageTask.Models;

namespace OrderezeImageTask.DataAccessLayer
{
    public class ImageContext : DbContext
    {

        public ImageContext() : base("ImageContext")
        {
        } 

        public DbSet<Image> Images { get; set; }
    } 
}