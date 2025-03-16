using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Mapping
{
    public class MarchentMapping
    {
        public static User ToModel(MerchantRegisterDTO merchantDto)
        {
            if (merchantDto == null) return null;

            return new User
            {
                Id = merchantDto.Id.ToString(),
                FirstName = merchantDto.FirstName,
                LastName = merchantDto.LastName,
                Email = merchantDto.Email,
                Password = merchantDto.Password,
            };
        }

        public static MerchantRegisterDTO ToDTO(User user)
        {
            if (user == null) return null;

            return new MerchantRegisterDTO
            {
                Id = Guid.TryParse(user.Id, out var guidId) ? guidId : Guid.NewGuid(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                Document = null // À mapper si nécessaire à partir des documents associés
            };
        }

        public static IEnumerable<MerchantRegisterDTO> ToDTOList(IEnumerable<User> users)
        {
            return users?.Select(user => ToDTO(user)).ToList() ?? new List<MerchantRegisterDTO>();
        }

        public static IEnumerable<User> ToModelList(IEnumerable<MerchantRegisterDTO> merchantDtos)
        {
            return merchantDtos?.Select(dto => ToModel(dto)).ToList() ?? new List<User>();
        }
    }
}
