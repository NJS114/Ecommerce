using Ecommerce.Services.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ecommerce.Services.DAO.Interfaces.IDAO
{
    public interface IUserDeleteInterface
    {
        Task DeleteUserAsync(string id);

    }
}
