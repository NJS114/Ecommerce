using  Ecommerce.Services.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace  Ecommerce.Services.DAO.Interfaces.UserInterface
{
    public interface IUserDeleteInterface
    {
        Task DeleteUserAsync(int id);
        
    }
}
