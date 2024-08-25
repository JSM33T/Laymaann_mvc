using Laymaann.Entities.Shared;
using Laymaann.Repositories;
using Laymaann.Web.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using System.Security.Claims;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

#region Serilog
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.WriteTo.Async(a => a.File($"Logs/log.txt", rollingInterval: RollingInterval.Day))
	.WriteTo.Console()
	.CreateLogger();

builder.Host.UseSerilog();
#endregion

builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
	builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
}
else
{
	builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
}

builder.Services.AddHttpContextAccessor();
var rateLimitingOptions = new RateLimitingOptions();
builder.Configuration.GetSection("RateLimiting").Bind(rateLimitingOptions);


#region rateLimiter
builder.Services.AddRateLimiter(options =>
{
	// Apply global rate limiting
	options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
	{
		return RateLimitPartition.GetFixedWindowLimiter(
			partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
			factory: partition => new FixedWindowRateLimiterOptions
			{
				PermitLimit = rateLimitingOptions.Global.PermitLimit,
				Window = rateLimitingOptions.Global.Window,
				QueueLimit = rateLimitingOptions.Global.QueueLimit,
			});
	});

	// Apply rate limiting for specific routes
	foreach (var route in rateLimitingOptions.Routes)
	{
		options.AddFixedWindowLimiter(route.Key, opt =>
		{
			opt.PermitLimit = route.Value.PermitLimit;
			opt.Window = route.Value.Window;
			opt.QueueLimit = route.Value.QueueLimit;
		});
	}

	options.RejectionStatusCode = 429; // Too Many Requests
});

#endregion

var almondcoveConfigSection = builder.Configuration.GetSection("LaymaannConfig");
var almondcoveConfig = builder.Configuration.GetSection("LaymaannConfig").Get<LaymaannConfig>();

builder.Services.Configure<LaymaannConfig>(almondcoveConfigSection);


builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
	  .AddCookie(options =>
	  {
		  options.LoginPath = "/auth/login";
		  options.AccessDeniedPath = "/accessdenied";
		  options.Events = new CookieAuthenticationEvents
		  {
			  OnRedirectToLogin = context =>
			  {
				  if (context.Request.Path.StartsWithSegments("/api"))
				  {
					  context.Response.StatusCode = 404;
				  }
				  else
				  {
					  var returnUrl = context.Request.Path + context.Request.QueryString;
					  context.Response.Redirect($"{context.RedirectUri}");
				  }
				  return Task.CompletedTask;
			  },
			  OnRedirectToAccessDenied = context =>
			  {
				  if (context.Request.Path.StartsWithSegments("/api"))
				  {
					  context.Response.StatusCode = 401;
				  }
				  else
				  {
					  context.Response.Redirect(context.RedirectUri);
				  }
				  return Task.CompletedTask;
			  }
		  };
	  })
	.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
	{
		options.ClientId = almondcoveConfig.GoogleKeys.ClientId;
		options.ClientSecret = almondcoveConfig.GoogleKeys.ClientSecret;
		options.SaveTokens = true;
		options.Scope.Add("profile");
		options.Scope.Add("email");

		options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
		options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
		options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
		options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
		options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
		options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
	});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddControllersWithViews();

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
	builder.AllowAnyOrigin()
		   .AllowAnyMethod()
		   .AllowAnyHeader();
}));


var app = builder.Build();
app.UseDeveloperExceptionPage();
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseCors("MyPolicy");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRateLimiter();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<ClaimsTransformationMiddleware>();
app.UseMiddleware<AcValidationMiddleware>();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
