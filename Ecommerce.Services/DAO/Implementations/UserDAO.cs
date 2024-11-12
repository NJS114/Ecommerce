using  Ecommerce.Services.DAO.Connexion;
using  Ecommerce.Services.DAO.Interfaces.UserInterface;
using  Ecommerce.Services.DAO.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace  Ecommerce.Services.DAO.Implementations
{
    public class UserDAO : IUserDAO
    {
        private readonly AppDbContext _context;
        public UserDAO(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUserAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "L'utilisateur ne peut pas être nul.");
            }

            var existingUser = await _context.Users.FindAsync(user.Id);

            if (existingUser == null)
            {
                throw new InvalidOperationException($"Aucun utilisateur trouvé avec l'ID {user.Id}.");
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdatePasswordAsync(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Password))
            {
                throw new ArgumentNullException(nameof(user), "L'utilisateur ou le mot de passe ne peut pas être nul ou vide.");
            }

            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser != null)
            {
                existingUser.Password = HashPassword(user.Password);
                await _context.SaveChangesAsync();
            }
        }

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
