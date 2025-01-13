using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Classes;
using server.Services;

namespace server.Controllers
{

    public class ProfileController : BaseApiController {
            
        private readonly IUsersService usersService;

        public ProfileController(IUsersService usersService) {
            this.usersService = usersService;
        }

        [AllowAnonymous]
        [HttpPost, Route("api/profile/login")]
        public async Task<UserDto> Login(LoginRequest request) {
            
            var user = await usersService.Login(request.Email, request.Password);
            AuthorizationHelper.Login(user, Response.Cookies);
            return user;
        }

        [AllowAnonymous]
        [HttpPost, Route("api/profile/register")]
        public async Task<UserDto> Register(RegisterRequest request) {
            var user = await usersService.Register(request.Email, request.Password);
            AuthorizationHelper.Login(user, Response.Cookies);
            return user;
        }
        
        [AllowAnonymous]
        [HttpGet, Route("api/profile/logout")]
        public IActionResult Logout() {
            
            AuthorizationHelper.Logout(Request.Cookies, Response.Cookies);

            // return RedirectPermanent("http://frontsite.ua/#/login");
            return Ok(new {
                response = "logged out",
            });
        }
        
        [HttpGet, Route("api/profile")]
        public async Task<UserDto> GetUser() {
            return await usersService.GetUser(CurrentIdentity.UserId);
        }
        
        [HttpPost, Route("api/profile")]
        public async Task ModifyUser(ModifyRequest request) {
            await usersService.ModifyUser(CurrentIdentity.UserId, request);
        }
    }
}
