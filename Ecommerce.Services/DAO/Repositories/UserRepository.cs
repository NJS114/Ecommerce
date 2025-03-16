using Ecommerce.Services.DAO.Builders;
using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Interfaces.IDAO;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Ecommerce.Services.DAO.Interfaces.IServices;
using Ecommerce.Services.DAO.Mapping;
using Ecommerce.Services.DAO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Services.DAO.Repositories
{
    public class UserRepository : ControllerBase, IUserRepository
    {
        private readonly IUserDAO _userDao;
        private readonly IInvoiceService _customerService;

        public UserRepository(IUserDAO userDao, IInvoiceService customerService)
        {
            _userDao = userDao;
            _customerService = customerService;
        }

        public async Task<UserDTO> GetUserWithDetailsAsync(string email)
        {
            var user = await _userDao.GetUserByEmailAsync(email);
            return UserMapping.ToUserDTO(user);
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsDTOAsync(int page, int pageSize)
        {
            var users = await _userDao.GetAllUserAsync(page, pageSize);
            return users.Select(UserMapping.ToUserDTO); 
        }

        public async Task RegisterFromDTOAsync(RegisterDTO userDto)
        {
            if (await _userDao.GetUserByEmailAsync(userDto.Email) != null)
            {
                throw new InvalidOperationException("Un utilisateur avec cet email existe déjà.");
            }

            userDto.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            var stripeCustomerId = await CreateStripeCustomerAsync(
                userDto.Email,
                userDto.FirstName,
                userDto.LastName,
                null, 
                null  
            );

            var user = new UserBuilder(new User())
                .SetFirstName(userDto.FirstName)
                .SetLastName(userDto.LastName)
                .SetEmail(userDto.Email)
                .SetPassword(userDto.Password)
                .SetStripeAccountId(stripeCustomerId)
                .Build();

            await _userDao.CreateUserAsync(user);
        }


        private async Task<string> CreateStripeCustomerAsync(string email, string firstName, string lastName, string phoneNumber, string address)
        {
            // Créer un client Stripe
            var options = new CustomerCreateOptions
            {
                Email = email,
                Name = $"{firstName} {lastName}",
                Phone = phoneNumber,
                Address = new AddressOptions
                {
                    Line1 = address,
                    PostalCode = "", // Vous pouvez ajouter d'autres informations comme le code postal
                },
            };

            var customer = await _customerService.CreateAsync(options);

            return customer.Id;
        }
        public async Task CreateUserDTOFromDTOAsync(UserDTO userDto)
        {
            var user = new UserBuilder(new User())
                .SetFirstName(userDto.FirstName)
                .SetLastName(userDto.LastName)
                .SetEmail(userDto.Email)
                .SetPhoneNumber(userDto.PhoneNumber)
                .SetAddress(userDto.Address)
                .SetCity(userDto.City)
                .SetPostalCode(userDto.PostalCode)
                .SetPassword(userDto.Password)
                .Build();

            await _userDao.CreateUserAsync(user);
        }

        public async Task DeleteUserFromDTOAsync(string id)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            if (user != null)
            {
                await _userDao.DeleteUserAsync(id);
            }
        }

        public async Task<UserDTO> GetUserByIdAsync(string id)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            return user != null ? UserMapping.ToUserDTO(user) : null;
        }

        public async Task UpdateNewPasswordAsync(RegisterDTO userDto)
        {
            if (userDto == null || string.IsNullOrWhiteSpace(userDto.Password))
            {
                throw new ArgumentNullException(nameof(userDto), "L'utilisateur ou le mot de passe ne peut pas être nul ou vide.");
            }

            var user = new User
            {
                Id = userDto.Id.ToString(),
                Password = userDto.Password
            };

            await _userDao.UpdatePasswordAsync(user);
        }

        private bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }

        public async Task<IActionResult> ValidateUserAsync(string id, UserDTO userDto)
        {
            if (userDto == null) return new BadRequestObjectResult("Les informations de l'utilisateur ne peuvent pas être nulles.");

            if (id != userDto.Id)
                return new BadRequestObjectResult("L'ID de l'utilisateur ne correspond pas à l'ID dans l'URL.");

            if (string.IsNullOrEmpty(userDto.FirstName) || string.IsNullOrEmpty(userDto.LastName) || string.IsNullOrEmpty(userDto.Email))
                return new BadRequestObjectResult("Tous les champs sont requis.");

            if (!IsValidEmail(userDto.Email)) return new BadRequestObjectResult("L'adresse email est invalide.");

            var user = await _userDao.GetUserByIdAsync(id);
            if (user == null)
                return new NotFoundObjectResult($"L'utilisateur avec l'ID {id} n'existe pas.");

            return null;
        }

        public async Task<IActionResult> UpdateUserDTOAsync(string id, UserDTO userDto)
        {
            var validationResult = await ValidateUserAsync(id, userDto);
            if (validationResult != null)
            {
                return validationResult;
            }

            var user = await _userDao.GetUserByIdAsync(id);
            if (user == null)
            {
                return BadRequest($"L'utilisateur avec l'ID {id} n'existe pas.");
            }

            // Vérifier et créer le client Stripe si nécessaire
            if (string.IsNullOrEmpty(user.StripeAccountId))
            {
                var stripeCustomerId = await CreateStripeCustomerAsync(
                    userDto.Email,
                    userDto.FirstName,
                    userDto.LastName,
                    userDto.PhoneNumber,
                    userDto.Address
                );

                // Mettre à jour le StripeAccountId
                user.StripeAccountId = stripeCustomerId;
            }

            // Mettre à jour les informations de l'utilisateur
            user = new UserBuilder(user)
                 .SetFirstName(userDto.FirstName)
                 .SetLastName(userDto.LastName)
                 .SetEmail(userDto.Email)
                 .SetPhoneNumber(userDto.PhoneNumber)
                 .SetAddress(userDto.Address)
                 .SetCity(userDto.City)
                 .SetPostalCode(userDto.PostalCode)
                 .SetPassword(userDto.Password) // Assurez-vous de gérer le mot de passe correctement (hash)
                 .Build();

            await _userDao.UpdateUserAsync(user);

            return NoContent();
        }


    }
}
