using blogapi.models;
using blogapi.models.DTOs;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace blogapi.Controllers
{
    [Route("blogpost")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        public string ConnectionString = "server=localhost;database=blog2;uid=root;password=";
        [HttpGet("read")]
        public object GetAllBlogger()
        {

            List<blogPost> blogposts = new List<blogPost>();

            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT * FROM `blogpost`";

            var command = new MySqlCommand(sql, connector);

            var datareader = command.ExecuteReader();

            while (datareader.Read())
            {
                var blog = new blogPost
                {
                    Id = datareader.GetInt32(0),
                    Title = datareader.GetString(1),
                    Content = datareader.GetString(2),
                    postTime = datareader.GetDateTime(3),
                    updateTime = datareader.GetDateTime(4),
                    blogId = datareader.GetInt32(5)
                };
                blogposts.Add(blog);
            }


            connector.Close();



            return new { message = "Sikeres lekérdezés. ", blogposts };
        }

      

        [HttpPost("create")]
        public object AddNewBlogger(AddnewBlogPost addnewblogpost)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"INSERT INTO `blogpost`(`Title`, `Content`, `postTime`, `updateTime`, `blogId`) VALUES(@title, @content, @posttime, @updatetime, @blogid)";

            var command = new MySqlCommand(sql, connector);

            command.Parameters.AddWithValue("@title", addnewblogpost.Title);
            command.Parameters.AddWithValue("@content", addnewblogpost.Content);
            command.Parameters.AddWithValue("@posttime", DateTime.Now);
            command.Parameters.AddWithValue("@updatetime", DateTime.Now);
            command.Parameters.AddWithValue("@blogid", addnewblogpost.blogId);

            command.ExecuteNonQuery();

            connector.Close();

            return new { message = "jo", results = addnewblogpost };
        }



        [HttpDelete]

        public object DeleteBlogger([FromBody] int Id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"DELETE FROM `blogpost` WHERE `Id`= @Id;";

            var command = new MySqlCommand(sql, connector);
            command.Parameters.AddWithValue("@Id", Id);

            object result = command.ExecuteNonQuery() > 0 ? new { message = "Sikeres törlés." } : new { message = "Nincs ilyen felhasználó." };

            connector.Close();

            return result;
        }

        [HttpPut("update")]
        public object UpateBlogger([FromQuery] int Id, [FromBody] UpdateBlogpost updateblogpost)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"UPDATE `blogpost` SET `Title`=@title,`Content`=@content,`postTime`=@posttime,`updateTime`=@updatetime,`blogId`=@blogid WHERE `id`= @id;";

            var command = new MySqlCommand(sql, connector);

            command.Parameters.AddWithValue("@title", updateblogpost.Title);
            command.Parameters.AddWithValue("@content", updateblogpost.Content);
            command.Parameters.AddWithValue("@posttime", DateTime.Now);
            command.Parameters.AddWithValue("@updatetime", DateTime.Now);
            command.Parameters.AddWithValue("@blogid", updateblogpost.blogId);
            command.Parameters.AddWithValue("@id", Id);

            object result = command.ExecuteNonQuery() > 0 ? new { message = "Sikeres frissítés." } : new { message = "Nincs ilyen felhasználó." };

            connector.Close();

            return result;
        }
        [HttpGet("name_email")]
        public object GetBloggerNameAndEmail(int id)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = @"SELECT `Name`, `Email` FROM `blogger2` WHERE `Id` = @id";
            var command = new MySqlCommand(sql, connector);
            command.Parameters.AddWithValue("@id", id);

            var datareader = command.ExecuteReader();
            object result = null;

            if (datareader.Read())
            {
                result = new
                {
                    Name = datareader.GetString(0),
                    Email = datareader.GetString(1)
                };
            }
            else
            {
                result = new { message = "Blogger nem található a blog2 táblában." };
            }

            connector.Close();
            return result;
        }

        [HttpGet("posts")]
        public object GetBloggerPostsWithInnerJoin(int id)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = @"SELECT blogger2.Name, blogpost.Title, blogpost.Content FROM `blogger2` blogger2 INNER JOIN `blogpost` blogpost ON blogger2.Id = blogpost.blogId WHERE blogger2.Id = @id";

            var command = new MySqlCommand(sql, connector);
            command.Parameters.AddWithValue("@id", id);
            var reader = command.ExecuteReader();

            string bloggerName = null;
            var posts = new List<object>();

            while (reader.Read())
            {
                if (bloggerName == null)
                {
                    bloggerName = reader.GetString(0);
                }

                posts.Add(new
                {
                    Title = reader.GetString(1),
                    Content = reader.GetString(2)
                });
            }

            connector.Close();

            if (bloggerName == null)
            {
                return new { message = "A blogger nem található a blog2 táblában, vagy nincsenek bejegyzései." };
            }

            return new
            {
                BloggerName = bloggerName,
                Posts = posts
            };
        }

        [HttpGet("posts_count")]
        public object GetTotalPostCount()
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = @"SELECT COUNT(*) FROM `blogpost`";
            var command = new MySqlCommand(sql, connector);

            long count = Convert.ToInt32(command.ExecuteScalar());

            connector.Close();

            return new { totalPosts = count };
        }


        [HttpGet("blogger_count")]
        public object GetBloggerPostCount(int id)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = @"SELECT COUNT(*) FROM `blogpost` WHERE `blogId` = @id";
            var command = new MySqlCommand(sql, connector);
            command.Parameters.AddWithValue("@id", id);

            long count = Convert.ToInt32(command.ExecuteScalar());

            connector.Close();

            return new { bloggerId = id, postCount = count };
        }

    }
}
