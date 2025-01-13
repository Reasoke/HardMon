using System.Net;
using server.Classes;
using server.Repository;

namespace server.Services
{
    public interface IUsersService {
        Task<UserDto> GetUser(int userId);
        Task<UserDto> Login(string email, string password);
        Task<UserDto> Register(string email, string password);
        Task ModifyUser(int userId, ModifyRequest request);
    }

    public class UsersService : IUsersService {

        private readonly IUsersRepository usersRepository;

        public UsersService(IUsersRepository usersRepository) {
            this.usersRepository = usersRepository;
        }

        public async Task<UserDto> GetUser(int userId) {
            return await usersRepository.GetUser(userId);
        }

        public async Task<UserDto> Login(string email, string password) {
            var hashedPassword = password.GetHash();
            var user = await usersRepository.GetUser(email, hashedPassword);
            if (user == null)
                throw new DomainException(HttpStatusCode.Forbidden, "User is not found");
            return user;
        }
        
        public async Task<UserDto> Register(string email, string password) {
            //todo: check if it's mail
            var registered = await usersRepository.IsEmailRegistered(email);
            if (registered) {
                throw new DomainException(HttpStatusCode.BadRequest, "User is already registered");
            }
            
            var hashedPassword = password.GetHash();
            var name = email;//todo:
            var user = await usersRepository.Register(name, email, hashedPassword);
            if (user == null) {
                throw new DomainException(HttpStatusCode.BadRequest, "Error registering User, try again later");
            }
            //todo (or not): create default acc with admin rights
            return user;
        }

        public async Task ModifyUser(int userId, ModifyRequest request) {
            await usersRepository.ModifyUser(userId, request);
        }
    }
}
