namespace Ecommerce.Services.DAO.Mapping
{
    public static class UserSummaryMapper
    {
        public static DTOs.UserSummary ToDto(Models.UserSummary model)
        {
            return new DTOs.UserSummary
            {
                UserId = model.UserId,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Email = model.Email
            };
        }
        public static Models.UserSummary ToModel(DTOs.UserSummary dto)
        {
            return new Models.UserSummary
            {
                UserId = dto.UserId,
                Firstname = dto.Firstname,
                Lastname = dto.Lastname,
                Email = dto.Email
            };
        }
    }

}
