using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Classes;

namespace server.Controllers
{

    [ApiController]
    public class BaseApiController : Controller {

        protected bool Authorized => User.Identity != null && User.Identity.IsAuthenticated && User.Identity is HardMonUserIdentity;
        protected HardMonUserIdentity CurrentIdentity => User.Identity as HardMonUserIdentity;
    }
}
