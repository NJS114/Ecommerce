using Ecommerce.Services.DAO.Models;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Ecommerce.Services.DAO.Connexion;
using System.Drawing.Printing;
using MongoFramework;
using IMongoDbConnection = Ecommerce.Services.DAO.Interfaces.IRepository.IMongoDbConnection;
using Ecommerce.Services.DAO.Interfaces.IDAO;
using MongoDB.Bson;
namespace Ecommerce.Services.DAO.Implementations
{
    public class UserDAO : IUserDAO
    {
        private readonly IMongoCollection<User> _users;

        public UserDAO(IMongoDbConnection mongoDbConnection)
        {
            var database = mongoDbConnection.GetDatabase();
            _users = database.GetCollection<User>("Users");
        }

        // Récupérer tous les utilisateurs
        public async Task<IEnumerable<User>> GetAllUserAsync(int page, int pageSize)
        {
            var users = await _users.Find(user => true)
                           .Skip((page - 1) * pageSize)
                           .Limit(pageSize)
                           .ToListAsync();

            foreach (var user in users)
            {
                user.Id = user.Id.ToString();
            }

            return users;
        }

        // Récupérer un utilisateur par ID
        public async Task<User> GetUserByIdAsync(string id) // Utilisez string pour l'ID si c'est un ObjectId MongoDB
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new KeyNotFoundException("Utilisateur non trouvé.");
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                throw new InvalidOperationException("Le mot de passe de l'utilisateur est nul ou vide.");
            }

            return user;
        }

        // Créer un utilisateur
        public async Task CreateUserAsync(User user)
        {
            if (string.IsNullOrEmpty(user.Id))
            {
                user.Id = ObjectId.GenerateNewId().ToString();
            }

            await _users.InsertOneAsync(user); 
        }

        // Mettre à jour un utilisateur
        public async Task UpdateUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "L'utilisateur ne peut pas être nul.");
            }

            var existingUser = await _users.Find(u => u.Id == user.Id).FirstOrDefaultAsync();

            if (existingUser == null)
            {
                throw new InvalidOperationException($"Aucun utilisateur trouvé avec l'ID {user.Id}.");
            }

            // Mettez à jour les champs de l'utilisateur
            var update = Builders<User>.Update
                .Set(u => u.FirstName, user.FirstName)
                .Set(u => u.LastName, user.LastName)
                .Set(u => u.Email, user.Email);

            await _users.UpdateOneAsync(u => u.Id == user.Id, update); // Utilisation de MongoDB pour la mise à jour
        }

        // Supprimer un utilisateur
        public async Task DeleteUserAsync(string id) // Utilisez string pour l'ID si c'est un ObjectId MongoDB
        {
            var result = await _users.DeleteOneAsync(user => user.Id == id); // Suppression via MongoDB
            if (result.DeletedCount == 0)
            {
                throw new InvalidOperationException($"Aucun utilisateur trouvé avec l'ID {id}.");
            }
        }

        // Mettre à jour le mot de passe d'un utilisateur
        public async Task UpdatePasswordAsync(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Password))
            {
                throw new ArgumentNullException(nameof(user), "L'utilisateur ou le mot de passe ne peut pas être nul ou vide.");
            }

            var existingUser = await _users.Find(u => u.Id == user.Id).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                var hashedPassword = HashPassword(user.Password);
                var update = Builders<User>.Update.Set(u => u.Password, hashedPassword);

                await _users.UpdateOneAsync(u => u.Id == user.Id, update);
            }
        }

        // Fonction pour hacher le mot de passe
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
