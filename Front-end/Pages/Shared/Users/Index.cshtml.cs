using Ecommerce.Services;
using Ecommerce.Services.DAO.Interfaces.Services;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class UsersModel : PageModel
{
    private readonly UserService.UserServiceClient _userServiceClient;

    // Injection du client gRPC
    public UsersModel(UserService.UserServiceClient userServiceClient)
    {
        _userServiceClient = userServiceClient;
    }

    public List<UserResponse> Users { get; set; } = new List<UserResponse>();

    public async Task OnGetAsync()
    {
        // Appel au service gRPC pour récupérer tous les utilisateurs
        var userList = await _userServiceClient.GetAllUsersAsync(new Empty());
        Users = userList.Users.ToList();
    }
}
