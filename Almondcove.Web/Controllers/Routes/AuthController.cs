using Laymaann.Entities.Shared;
using Laymaann.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Laymaann.Web.Controllers.Routes
{
    public class AuthController(IUserRepository userRepository) : Controller
    {
        private readonly IUserRepository _userRepo = userRepository;

        [AllowAnonymous]
        [Route("auth/claims")]
        public IActionResult Claims()
        {
            var claims = User.Claims.Select(c => new
            {
                Type = c.Type,
                Value = c.Value
            });

            return View("Views/Auth/Claims.cshtml", claims);
        }

        [AllowAnonymous]
        [Route("/auth/login")]
        public IActionResult LoginPage(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("Views/Auth/Login.cshtml");
        }

        [AllowAnonymous]
        [Route("/auth/google-login")]
        public async Task Login(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", new { returnUrl })
            };
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
        }

        [AllowAnonymous]
        [Route("/auth/google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result.Succeeded && result.Principal != null)
            {
                var claims = result.Principal.Identities.FirstOrDefault()?.Claims;

                var googleId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var firstName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
                var lastName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? string.Empty;
                var profilePicture = claims?.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value ?? string.Empty;

                var user = new AcUser
                {
                    GoogleId = googleId,
                    Username = email.EndsWith("@gmail.com") ? email.Substring(0, email.Length - 10) : email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    ProfilePicture = profilePicture
                };

                user = await _userRepo.AddOrUpdateUser(user);

                if (string.IsNullOrEmpty(user.GoogleId))
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return RedirectToAction("Login");
                }

                var userClaims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.Username ?? string.Empty),
                    new(ClaimTypes.NameIdentifier, user.GoogleId),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
                    new(ClaimTypes.Surname, user.LastName ?? string.Empty),
                    new("Id", user.Id.ToString()),
                    new("Avatar", user.ProfilePicture ?? string.Empty),
                    new(ClaimTypes.Role, user.RoleId.ToString()),
                    new("ManualLogin", "true")
                };

                var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return LocalRedirect(returnUrl);
            }

            return Redirect("/");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
    }
}
