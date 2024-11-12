using  Ecommerce.Services.DAO.Models;
using System.Security.Cryptography;
using System.Text;

namespace  Ecommerce.Services.DAO.Builders
{
    public class UserBuilder
    {
        private readonly User _user;

        public UserBuilder()
        {
            _user = new User();
        }

        public UserBuilder(User user)
        {
            _user = user;
        }

        public UserBuilder SetName(string fullName)
        {
            var names = fullName.Split(' ');
            _user.FirstName = names[0];
            _user.LastName = names.Length > 1 ? names[1] : string.Empty; 
            return this;
        }

        public UserBuilder SetLastName(string lastName)
        {
            _user.LastName = lastName;
            return this;
        }

        public UserBuilder SetEmail(string email)
        {
            _user.Email = email;
            return this;
        }

        public UserBuilder SetPassword(string password)
        {
            _user.Password = HashPassword(password);
            return this;
        }

        public User Build()
        {
            return _user;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
