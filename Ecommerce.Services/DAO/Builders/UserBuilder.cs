using Ecommerce.Services.DAO.Models;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.Services.DAO.Builders
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
        public UserBuilder SetStripeAccountId(string stripeAccountId)
        {
            _user.StripeAccountId = stripeAccountId;
            return this;
        }
        public UserBuilder SetId(string id)
        {
            _user.Id = id;
            return this;
        }

        public UserBuilder SetFirstName(string firstName)
        {
            _user.FirstName = firstName;
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
            _user.Password = password;
            return this;
        }

        // Ajout des nouvelles propriétés
        public UserBuilder SetAddress(string address)
        {
            _user.Address = address;
            return this;
        }

        public UserBuilder SetPhoneNumber(string phoneNumber)
        {
            _user.PhoneNumber = phoneNumber;
            return this;
        }

        public UserBuilder SetCity(string city)
        {
            _user.City = city;
            return this;
        }

        public UserBuilder SetPostalCode(string postalCode)
        {
            _user.PostalCode = postalCode;
            return this;
        }

        public User Build()
        {
            return _user;
        }

      /*  private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }*/
    }
}
