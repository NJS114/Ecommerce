using  Ecommerce.Services.DAO.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  Ecommerce.Services.DAO.Interfaces.UserInterface
{
    public interface IUserReadInterface
    {
        Task<User> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUserAsync();
        
    }
}
