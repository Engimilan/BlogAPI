using blogapi.models;
using blogapi.models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Globalization;
using System.Security.Cryptography;

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

        [HttpPost("register")]
        public object AddNewBlogger(AddNewBloggerDTO addNewBloggerDTO)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"INSERT INTO blogger2 (Name, Email, Age, Password, RegistrationTime) VALUES (@name,@email,@age,@password,@registrationTime)";

            var command = new MySqlCommand(sql, connector);

            command.Parameters.AddWithValue("@name", addNewBloggerDTO.Name);
            command.Parameters.AddWithValue("@email", addNewBloggerDTO.Email);
            command.Parameters.AddWithValue("@age", addNewBloggerDTO.Age);
            command.Parameters.AddWithValue("@password", addNewBloggerDTO.Password);
            command.Parameters.AddWithValue("@registrationTime", DateTime.Now);

            command.ExecuteNonQuery();

            connector.Close();

            return new { message = "jo", results = addNewBloggerDTO };
        }

        [HttpPost("login")]
        public object LoginBlogger(LoginDTO loginDTO)
        {

            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"SELECT `Id` FROM `blogger2` WHERE `Email`= @Email AND `Password`=@password;";

            var command = new MySqlCommand(sql, connector);
            command.Parameters.AddWithValue("@Email", loginDTO.Email);
            command.Parameters.AddWithValue("@Password", loginDTO.Password);

            var datareader = command.ExecuteReader();

            if (datareader.Read() == true)
            {
                return new { message = "SIKERES BELÉPÉS", result = datareader.GetInt32("Id") };
            }
            else
            {
                return new { message = "SIKERTELEN BELÉPÉS", result = loginDTO };
            }
        }

        [HttpDelete]

        public object DeleteBlogger([FromBody] int Id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"DELETE FROM `blogger2` WHERE `Id`= @Id;";

            var command = new MySqlCommand(sql, connector);
            command.Parameters.AddWithValue("@Id", Id);

            object result = command.ExecuteNonQuery() > 0 ? new { message = "Sikeres törlés." } : new { message = "Nincs ilyen felhasználó." };

            connector.Close();

            return result;
        }

        [HttpPut("update")]
        public object UpateBlogger([FromQuery] int Id, [FromBody] UpdateDTO updatedto)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"UPDATE `blogger2` SET `name`=@name,`email`=@email,`age`=@age,`password`=@password WHERE `id`= @id;";

            var command = new MySqlCommand(sql, connector);

            command.Parameters.AddWithValue("@name", updatedto.Name);
            command.Parameters.AddWithValue("@email", updatedto.Email);
            command.Parameters.AddWithValue("@age", updatedto.Age);
            command.Parameters.AddWithValue("@password", updatedto.Password);
            command.Parameters.AddWithValue("@id", Id);

            object result = command.ExecuteNonQuery() > 0 ? new { message = "Sikeres frissítés." } : new { message = "Nincs ilyen felhasználó." };

            connector.Close();

            return result;
        }
    }
}
