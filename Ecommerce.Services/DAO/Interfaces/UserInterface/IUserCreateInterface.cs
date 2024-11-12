using  Ecommerce.Services.Controllers;
using  Ecommerce.Services.DAO.DTOs;
using  Ecommerce.Services.DAO.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace  Ecommerce.Services.DAO.Interfaces.UserInterface
{
    public interface IUserCreateInterface
    {
        Task CreateUserAsync(User user);
        Task UpdatePasswordAsync(User user);
        

    }

}
