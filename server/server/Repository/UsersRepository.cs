using System.Net;
using Dapper;
using server.Classes;

namespace server.Repository
{
    public interface IUsersRepository {
        Task<UserDto> GetUser(int userId);
        Task<UserDto> GetUser(string email, string password);
        Task<bool> IsEmailRegistered(string email);
        Task<UserDto> Register(string name, string email, string password);
        Task ModifyUser(int userId, ModifyRequest request);
    }

    public class UsersRepository : BaseRepository, IUsersRepository {
        
        public UsersRepository(ISettingsProvider settingsProvider) : base(settingsProvider) {
        }

        public async Task<UserDto> GetUser(int userId) {

            return await GetConnection().QueryFirstOrDefaultAsync<UserDto>(
                @"SELECT userId as Id, Name, Email, isSysAdmin FROM [User] u WHERE userId = @userId", 
                new { userId });
        }

        public async Task<UserDto> GetUser(string email, string password) {

            return await GetConnection().QueryFirstOrDefaultAsync<UserDto>(
                @"SELECT userId as Id, Name, Email, isSysAdmin FROM [User] u WHERE upper(Email) = @email AND Password = @password", 
                new { email = email.ToUpper(), password });
        }

        public async Task<bool> IsEmailRegistered(string email) {
            var count = await GetConnection().ExecuteScalarAsync<int>(
                @"SELECT userId as Id, Name, Email FROM [User] u WHERE upper(Email) = @email", 
                new { email = email.ToUpper()});
            return count > 0;
        }

        public async Task<UserDto> Register(string name, string email, string password) {
            var userId = await GetConnection().ExecuteScalarAsync<int>(
                @"INSERT INTO [User] (name, email, password) VALUES (@name, @email, @password);
SELECT CAST(SCOPE_IDENTITY() as int);", 
                new {name, email, password});
            return userId > 0 ? new UserDto{Id = userId, Name = name, Email = email} : null;
        }

        public async Task ModifyUser(int userId, ModifyRequest request) {
            var rowsAffected = await GetConnection().ExecuteAsync(
                @"UPDATE [User]
SET name = @name
WHERE userId = @userId", 
                new { userId, name = request.Name});
        
            if (rowsAffected == 0) {
                throw new DomainException(HttpStatusCode.NotFound, "User is not found");
            }
        }
    }
}
