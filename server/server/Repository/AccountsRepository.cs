using System.Net;
using Dapper;
using server.Classes;

namespace server.Repository;

public interface IAccountsRepository {
    Task<IEnumerable<AccountDto>> GetAccounts(int userId);
    Task<AccountDto> GetAccount(int userId, int accountId);
    Task LeaveAccount(int userId, int accountId);
    Task DeleteAccount(int accountId);
    Task CreateAccount(string accountName, int adminId);
    Task<AccountDetailsDto> GetAccountDetails(int accountId);
    Task ModifyAccount(AccountDetailsRequest request);
    Task<AccountUserType> GetUserTypeByAccount(int userId, int accountId);
    Task<bool> IsUserInAccount(int accountId, string email);
    Task<bool> IsUserInvitedToAccount(int accountId, string email);
    Task InviteUser(int userId, int accountId, AddAccountUserRequest request);
    Task DeleteInvite(int accountId, string email);
    Task<AccountInvitesDto> GetAccountInvites(int accountId);
}

public class AccountsRepository : BaseRepository, IAccountsRepository {
        
    public AccountsRepository(ISettingsProvider settingsProvider) : base(settingsProvider) {
    }

    public async Task<IEnumerable<AccountDto>> GetAccounts(int userId) {
        return await GetConnection().QueryAsync<AccountDto>(
         @"SELECT  A.accountId as Id, A.accountName as Name, UA.userTypeId as UserType, S.subscriptionName as Subscription
FROM UserAccount as UA
JOIN Account as A ON UA.accountId = A.accountId
JOIN Subscription as S ON S.subscriptionId = A.subscriptionId
WHERE UA.userId = @userId
ORDER BY A.accountName", 
         new { userId });
    }

    public async Task<AccountDto> GetAccount(int userId, int accountId) {
        return await GetConnection().QueryFirstOrDefaultAsync<AccountDto>(
            @"SELECT  A.accountId as Id, accountName as Name, UA.userTypeId as UserType, S.subscriptionName as Subscription
FROM UserAccount as UA
JOIN Account as A ON UA.accountId = A.accountId
JOIN Subscription as S ON S.subscriptionId = A.subscriptionId
WHERE UA.userId = @userId and UA.accountId = @accountId", 
            new { userId, accountId });
    }

    public async Task LeaveAccount(int userId, int accountId) {
        var rowsAffected = await GetConnection().ExecuteAsync(
            @"DELETE FROM UserAccount WHERE accountId = @accountId and userId = @userId", 
            new { userId, accountId });
        if (rowsAffected == 0) {
            throw new DomainException(HttpStatusCode.NotFound, "Account is not found");
        }
    }

    public async Task DeleteAccount(int accountId) {
        var rowsAffected = await GetConnection().ExecuteAsync(
            @"DELETE FROM Account WHERE accountId = @accountId", 
            new { accountId });
        
        if (rowsAffected == 0) {
            throw new DomainException(HttpStatusCode.NotFound, "Account is not found");
        }
    }

    public async Task CreateAccount(string accountName, int adminId) {
        await GetConnection().ExecuteAsync(
            @"DECLARE @accountId int;
INSERT INTO Account (subscriptionId, accountName) values (1, @accountName);
SELECT @accountId = CAST(SCOPE_IDENTITY() as int);
INSERT INTO UserAccount (userId, accountId, userTypeId) values (@adminId, @accountId, @userType);", 
            new { accountName, adminId, userType = AccountUserType.Admin });
    }

    public async Task<AccountDetailsDto> GetAccountDetails(int accountId) {
        var reader = await GetConnection().QueryMultipleAsync(
            @"SElECT accountId as Id, subscriptionId as Subscription, accountName as Name
FROM Account WHERE accountId = @accountId;

SELECT UA.userId as Id, U.name, U.email, UA.userTypeId as UserType
FROM UserAccount as UA
JOIN [User] as U ON U.userId = UA.userId
WHERE UA.accountId = @accountId;", 
            new { accountId });

        var account = await reader.ReadFirstOrDefaultAsync<AccountDetailsDto>();
        if (account == null) {
            throw new DomainException(HttpStatusCode.NotFound, "Account is not found");
        }
        account.AccountUsers = await reader.ReadAsync<UserAccountDto>();

        return account;
    }

    public async Task ModifyAccount(AccountDetailsRequest request) {
        var rowsAffected = await GetConnection().ExecuteAsync(
            @"UPDATE Account
SET accountName = @accountName, subscriptionId = @accountSubscription
WHERE accountId = @accountId;", 
            new { accountId = request.Id, accountName = request.Name, accountSubscription = (int)request.Subscription });
        
        if (rowsAffected == 0) {
            throw new DomainException(HttpStatusCode.NotFound, "Account is not found");
        }
    }

    public async Task<AccountUserType> GetUserTypeByAccount(int userId, int accountId) {
        return await GetConnection().QueryFirstOrDefaultAsync<AccountUserType>(
            @"SELECT userTypeId FROM UserAccount WHERE userId = @userId AND accountId = @accountId", 
            new { userId, accountId });
    }

    public async Task<bool> IsUserInAccount(int accountId, string email) {
        var recordsNumberInDB =  await GetConnection().QueryFirstOrDefaultAsync<Int64>(
            @"SELECT 1 FROM UserAccount as UA
JOIN [User] as U ON UA.userId = U.userId
WHERE UA.accountId = @accountId AND U.email = @email", 
            new { accountId, email });
        return recordsNumberInDB == 1;
    }

    public async Task<bool> IsUserInvitedToAccount(int accountId, string email) {
        var recordsNumberInDB =  await GetConnection().QueryFirstOrDefaultAsync<Int64>(
            @"SELECT 1 FROM Invite
WHERE accountId = @accountId AND guestEmail = @email", 
            new { accountId, email });
        return recordsNumberInDB == 1;
    }

    public async Task InviteUser(int userId, int accountId, AddAccountUserRequest request) {
        await GetConnection().ExecuteAsync(
            @"INSERT INTO Invite (accountId, hostUserId, guestEmail, userTypeId, created) 
VALUES (@accountId, @userId, @email, @userTypeId, GETDATE());", 
            new { accountId, userId, email = request.Email, userTypeId = (int)request.UserType });
    }

    public async Task DeleteInvite(int accountId, string email) {
        await GetConnection().ExecuteAsync(
            @"DELETE FROM Invite WHERE accountId = @accountId and guestEmail = @email", 
            new { accountId, email });
    }

    public async Task<AccountInvitesDto> GetAccountInvites(int accountId) {
        var reader = await GetConnection().QueryMultipleAsync(
            @"SElECT accountId as Id, accountName as Name
FROM Account WHERE accountId = @accountId;

SELECT inviteId as Id, accountId as AccountId, guestEmail as GuestEmail, userTypeId as UserType
FROM Invite
WHERE accountId = @accountId;", 
            new { accountId });

        var accountInfo = await reader.ReadFirstOrDefaultAsync<AccountInvitesDto>();
        if (accountInfo == null) {
            throw new DomainException(HttpStatusCode.NotFound, "Account is not found");
        }
        accountInfo.Invites = await reader.ReadAsync<InviteDto>();

        return accountInfo;
    }
}