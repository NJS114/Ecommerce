/*using using Ecommerce.Services.DAO.Connexion;
using using Ecommerce.Services.DAO.Models;
using System.Data.SqlClient;

namespace using Ecommerce.Services.DAO.Implementations
{
    public class using Ecommerce.ServicesDAO
    {
        private readonly ConnexionManager _connectionManager;
        public using Ecommerce.ServicesDAO(ConnexionManager connexionManager) 
        {
            _connectionManager = connexionManager;
        }
        public IEnumerable<User> GetAllUser()
        {
            var users = new List<User>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM Users", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                // Ajoutez d'autres propriétés selon votre modèle
                            };
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }
        public void CreateUser(User user)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO Users (Name) VALUES (@Name)", connection))
                {
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.ExecuteNonQuery();
                }
            }
        }
        private SqlConnection GetConnection()
        {
            var connectionString = _connectionManager.GetConnectionString();
            return new SqlConnection(connectionString);
        }
    }
}
*/