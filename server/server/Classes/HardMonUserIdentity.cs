using System.Security.Claims;

namespace server.Classes;

public class HardMonUserIdentity : ClaimsIdentity
{
    public int UserId { get; }
    public string UserEmail { get; }
    public bool IsSysAdmin { get; }

    public HardMonUserIdentity(int userId, string userEmail, bool isSysAdmin) : base([new Claim("sub", userId.ToString())], "Basic") {

        UserId = userId;
        UserEmail = userEmail;
        IsSysAdmin = isSysAdmin;
    }
}