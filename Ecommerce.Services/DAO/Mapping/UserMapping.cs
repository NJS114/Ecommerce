using  Ecommerce.Services.DAO.DTOs;
using  Ecommerce.Services.DAO.Models;
using System.Collections.Generic;
using System.Linq;

namespace  Ecommerce.Services.DAO.Mapping
{
    public static class UserMapping
    {
            public static RegisterDTO ToDTO(User user)
            {
                if (user == null) return null;

            return new RegisterDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password

            };
            }

        public static User ToModel(RegisterDTO userDto)
        {
            if (userDto == null) return null;


            return new User
            {
                Id =  userDto.Id.ToString() ,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email
            };
        }
        public static UserDTO ToUserDTO(User user)
        {
            if (user == null) return null;

           
               return new UserDTO
               {
                   Id = user.Id,
                   FirstName = user.FirstName,
                   LastName = user.LastName,
                   Email = user.Email,
                   Password = user.Password,
                   Status = user.Status,
                   Role = user.Role,
                   StripeAccountId = user.StripeAccountId,
                   BankAccountDetails = user.BankAccountDetails,
                   Address = user.Address, // Include Address
                   PhoneNumber = user.PhoneNumber, // Include PhoneNumber
                   BooksForSaleOrRent = user.BooksForSaleOrRent.Select(b => b.Id).ToList(),
                   Transactions = user.Transactions.Select(t => t.Id.ToString()).ToList(),
               };
        }

        // Mapping de UserDTO vers User
        public static User ToUserModel(UserDTO userDto)
        {
            if (userDto == null) return null;

           return new User
            {
               Id = userDto.Id,
               FirstName = userDto.FirstName,
               LastName = userDto.LastName,
               Email = userDto.Email,
               Password = userDto.Password,
               Status = userDto.Status,
               Role = userDto.Role,
               StripeAccountId = userDto.StripeAccountId,
               BankAccountDetails = userDto.BankAccountDetails,
               Address = userDto.Address, 
               PhoneNumber = userDto.PhoneNumber, 
               BooksForSaleOrRent = userDto.BooksForSaleOrRent.Select(book => new Book { Id = book }).ToList(), 
               Transactions = userDto.Transactions.Select(t => new PaymentMethod { Id = t }).ToList(), 
           };
        }



        public static IEnumerable<RegisterDTO> ToDTOList(IEnumerable<User> users)
        {
            return users?.Select(user => ToDTO(user)).ToList() ?? new List<RegisterDTO>();
        }

        public static IEnumerable<User> ToModelList(IEnumerable<RegisterDTO> userDtos)
        {
            return userDtos?.Select(dto => ToModel(dto)).ToList() ?? new List<User>();
        }
        public static IEnumerable<UserDTO> ToUserDTOList(IEnumerable<User> users)
        {
            return users?.Select(user => ToUserDTO(user)).ToList() ?? new List<UserDTO>();
        }

        
        public static IEnumerable<User> ToUserModelList(IEnumerable<UserDTO> userDtos)
        {
            return userDtos?.Select(dto => ToUserModel(dto)).ToList() ?? new List<User>();
        }
    }
}
