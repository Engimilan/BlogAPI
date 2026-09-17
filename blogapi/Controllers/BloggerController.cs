using blogapi.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace blogapi.Controllers
{
    [Route("bloggers")]
    [ApiController]
    public class BloggerController : ControllerBase
    {
        public string ConnectionString = "server=localhost;database=blog2;uid=root;password=";
        [HttpGet("all")]
        public object GetAllBlogger() 
        {

            List<Blogger> bloggers = new List<Blogger>();

            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT * FROM `blogger2`";

            var command = new MySqlCommand(sql, connector);

            var datareader = command.ExecuteReader();

            while (datareader.Read())
            {
                var blogger = new Blogger
                {
                    Id = datareader.GetInt32(0),
                    Name = datareader.GetString(1),
                    Email = datareader.GetString(2),
                    Age = datareader.GetInt32(3),
                    Password = datareader.GetString(4),
                    RegistrationTime = datareader.GetDateTime(5)
                };
                bloggers.Add(blogger);
            }


            connector.Close();



            return new { message = "Sikeres lekérdezés. ", bloggers };
        }

        [HttpGet("byId")]
        public object GetBloggerById(int Id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT * FROM `blogger2` WHERE `Id`= @Id;";

            var command = new MySqlCommand(sql, connector);
            command.Parameters.AddWithValue("@Id", Id);

            var datareader = command.ExecuteReader();

            datareader.Read();

            var blogger = new Blogger
            {
                Id = datareader.GetInt32(0),
                Name = datareader.GetString(1),
                Email = datareader.GetString(2),
                Age = datareader.GetInt32(3),
                Password = datareader.GetString(4),
                RegistrationTime = datareader.GetDateTime(5)
            };



            connector.Close();
            return new { message = "jo", result = blogger };
        }
    }
}
