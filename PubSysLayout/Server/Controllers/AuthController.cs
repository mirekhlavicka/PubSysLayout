using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using PubSysLayout.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace PubSysLayout.Server.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public class loginRequest
        {
            public string userName { get; set; }
            public string password { get; set; }
            public bool rememberMe { get; set; }
        }

        [HttpPost]
        //[Route("login")]
        public async Task<IActionResult> Login(/*[FromBody]*/ loginRequest lr)
        {
            //var user = _context.Users.SingleOrDefault(u => u.LoginName == lr.userName);

            if (String.IsNullOrEmpty(lr.userName) || String.IsNullOrEmpty(lr.password) || lr.password != lr.userName + "123" /*|| user == null || user.Password != lr.password*/)
            {
                return BadRequest();
            }

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, lr.userName),
                //new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                //new Claim("FullName", user.FullName),
                new Claim(ClaimTypes.Role, "Administrator")
            }, "Cookies");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await Request.HttpContext.SignInAsync("Cookies", claimsPrincipal, new AuthenticationProperties { IsPersistent = lr.rememberMe });

            return NoContent();
        }

        [HttpPost]
        //[Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await Request.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return NoContent();
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        public IActionResult GetCurrentUser() =>
                    Ok(User.Identity.IsAuthenticated ? CreateUserInfo(User) : UserInfo.Anonymous);

        private UserInfo CreateUserInfo(ClaimsPrincipal claimsPrincipal)
        {
            if (!claimsPrincipal.Identity.IsAuthenticated)
            {
                return UserInfo.Anonymous;
            }

            var userInfo = new UserInfo
            {
                IsAuthenticated = true
            };

            if (claimsPrincipal.Identity is ClaimsIdentity claimsIdentity)
            {
                userInfo.NameClaimType = claimsIdentity.NameClaimType;
                userInfo.RoleClaimType = claimsIdentity.RoleClaimType;
            }
            //else
            //{
            //    userInfo.NameClaimType = JwtClaimTypes.Name;
            //    userInfo.RoleClaimType = JwtClaimTypes.Role;
            //}

            if (claimsPrincipal.Claims.Any())
            {
                var claims = new List<ClaimValue>();
                var nameClaims = claimsPrincipal.FindAll(userInfo.NameClaimType);
                foreach (var claim in nameClaims)
                {
                    claims.Add(new ClaimValue(userInfo.NameClaimType, claim.Value));
                }

                // Uncomment this code if you want to send additional claims to the client.
                //foreach (var claim in claimsPrincipal.Claims.Except(nameClaims))
                //{
                //    claims.Add(new ClaimValue(claim.Type, claim.Value));
                //}

                userInfo.Claims = claims;
            }

            return userInfo;
        }
    }
}