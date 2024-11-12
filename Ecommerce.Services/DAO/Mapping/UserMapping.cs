using  Ecommerce.Services.DAO.DTOs;
using  Ecommerce.Services.DAO.Models;
using System.Collections.Generic;
using System.Linq;

namespace  Ecommerce.Services.DAO.Mapping
{
    public static class UserMapping
    {
        public static UserDTO ToDTO(User user)
        {
            if (user == null) return null;

            return new UserDTO
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email
            };
        }

        public static User ToModel(UserDTO userDto)
        {
            if (userDto == null) return null;

            var nameParts = userDto.FullName?.Split(' ').Where(part => !string.IsNullOrWhiteSpace(part)).ToArray();

            return new User
            {
                Id = userDto.Id,
                FirstName = nameParts != null && nameParts.Length > 0 ? nameParts[0] : string.Empty,
                LastName = nameParts != null && nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty,
                Email = userDto.Email
            };
        }

        public static IEnumerable<UserDTO> ToDTOList(IEnumerable<User> users)
        {
            return users?.Select(user => ToDTO(user)).ToList() ?? new List<UserDTO>();
        }

        public static IEnumerable<User> ToModelList(IEnumerable<UserDTO> userDtos)
        {
            return userDtos?.Select(dto => ToModel(dto)).ToList() ?? new List<User>();
        }
    }
}
