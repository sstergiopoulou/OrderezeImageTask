using System.Collections.Generic;
using OrderezeImageTask.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace OrderezeImageTask.DataAccessLayer
{
    public class ImageDBFunctions
    {
        string connstring = ConfigurationManager.AppSettings["dbconnstring"].ToString();

        public int insertImageToDb(Image image)
        {   // Insert the new image to database
            using (SqlConnection conn = new SqlConnection(connstring))
            {
                SqlCommand CmdSql = new SqlCommand("INSERT INTO [imagesTable] (Name, Description,imagepath) VALUES (@Name, @Description, @Imagepath);SELECT SCOPE_IDENTITY();", conn);
                conn.Open();
                CmdSql.Parameters.AddWithValue("@Name", image.Name);
                CmdSql.Parameters.AddWithValue("@Description", image.Description);
                CmdSql.Parameters.AddWithValue("@Imagepath", image.ImagePath);

                object newimageid = CmdSql.ExecuteScalar();
                conn.Close();

                return int.Parse(newimageid.ToString());
            }
        }

        public string deleteImagefromDB(int id)
        {
            // Insert an image from the database
            using (SqlConnection conn = new SqlConnection(connstring))
            {
                SqlCommand CmdSql = new SqlCommand("SELECT imagepath from [imagesTable] where id=@ID", conn);
                CmdSql.Connection = conn;
                conn.Open();
                CmdSql.Parameters.AddWithValue("@ID", id);
                CmdSql.ExecuteNonQuery();

                SqlDataReader myReader = CmdSql.ExecuteReader();

                myReader.Read();
                string imagepath = myReader["imagepath"].ToString();

                CmdSql.CommandText = "DELETE FROM [imagesTable] WHERE ID=@ID;";
                myReader.Close();
                CmdSql.ExecuteNonQuery();

                conn.Close();

                return imagepath;
            }
        }


        public List<Image> getImagesFromDB()
        {
            // Read images from the database
            using (SqlConnection conn = new SqlConnection(connstring))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [imagesTable]", conn))
                {
                    conn.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        List<Image> imagesdata = new List<Image>();
                        while (sdr.Read())
                        {
                            imagesdata.Add(new Image());
                            imagesdata[imagesdata.Count - 1].Id = int.Parse(sdr["id"].ToString());
                            imagesdata[imagesdata.Count - 1].Name = sdr["name"].ToString();
                            imagesdata[imagesdata.Count - 1].Description = sdr["description"].ToString();
                            imagesdata[imagesdata.Count - 1].ImagePath = sdr["imagepath"].ToString();
                        }
                        conn.Close();
                        return imagesdata;
                    }
                }
            }
        }
    }
}