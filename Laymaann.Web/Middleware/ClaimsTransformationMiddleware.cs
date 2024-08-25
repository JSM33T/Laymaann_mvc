using Laymaann.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Laymaann.Web.Middleware
{
    public class ClaimsTransformationMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly ILogger<ClaimsTransformationMiddleware> _logger;

		public ClaimsTransformationMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory, ILogger<ClaimsTransformationMiddleware> logger)
		{
			_next = next;
			_serviceScopeFactory = serviceScopeFactory;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (context.User.Identity.IsAuthenticated)
			{
				var isManualLogin = context.User.FindFirst("ManualLogin")?.Value;

				if (isManualLogin == null) // Only transform claims if it's not a manual login
				{
					try
					{
						using (var scope = _serviceScopeFactory.CreateScope())
						{
							var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
							var googleId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

							if (!string.IsNullOrEmpty(googleId))
							{
								var user = await userRepo.GetUserByGoogleId(googleId);

								if (user == null)
								{
									// If user is not found, sign out and redirect to login
									//await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
									//context.Response.Redirect("/Auth/Login");
									//return;
									await _next(context);
								}
								else
								{
									// Create a new claims identity with the updated user information
									var claimsIdentity = new ClaimsIdentity(context.User.Identity.AuthenticationType);

									// Add custom claims from the database
									claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Username ?? string.Empty));
									claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
									claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty));
									claimsIdentity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty));
									claimsIdentity.AddClaim(new Claim("Avatar", user.ProfilePicture ?? string.Empty));
									claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, user.RoleId.ToString()));
									claimsIdentity.AddClaim(new Claim("Role", user.RoleId.ToString()));

									claimsIdentity.AddClaim(new Claim("Id", user.Id.ToString()));

                                    var principal = new ClaimsPrincipal(claimsIdentity);
									context.User = principal;

									// Sign in with the new claims identity
									await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
								}
							}
						}
					}
					catch (Exception ex)
					{
						_logger.LogError($"Error in middleware: {ex}", ex);
						await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
						context.Response.Redirect("/Auth/Login");
						return;
					}
				}
			}
			await _next(context);
		}
	}
}
