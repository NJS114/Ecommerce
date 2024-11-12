using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Ecommerce.Services.DAO.Interfaces.Services
{
    public class UserServiceImpl : Ecommerce.Services.UserService.UserServiceBase
    {
        private readonly IUserRepository _userRepository;

        public UserServiceImpl(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public override async Task<UserList> GetAllUsers(Empty request, ServerCallContext context)
        {
            var users = await _userRepository.GetAllUsersAsDTOAsync();
            var userList = new UserList();
            userList.Users.AddRange(users.Select(u => new UserResponse
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email
            }));
            return userList;
        }

        public override async Task<UserResponse> GetUser(UserRequest request, ServerCallContext context)
        {
            var user = await _userRepository.GetUserWithDetailsAsync(request.Id);
            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"User with ID {request.Id} not found"));

            return new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email
            };
        }

        public override async Task<UserResponse> CreateUser(UserRequest request, ServerCallContext context)
        {
            var newUser = new UserDTO
            {
                FullName = request.FullName,
                Email = request.Email,
                Password = request.Password
            };
            var createdUser = await _userRepository.CreateUserFromDTOAsync(newUser);
            return new UserResponse
            {
                Id = createdUser.Id,
                FullName = createdUser.FullName,
                Email = createdUser.Email
            };
        }

        public override async Task<Empty> DeleteUser(UserRequest request, ServerCallContext context)
        {
            await _userRepository.DeleteUserFromDTOAsync(request.Id);
            return new Empty();
        }
    }

}
