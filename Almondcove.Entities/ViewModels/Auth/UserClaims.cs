using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Laymaann.Entities.ViewModels.Auth
{
    public class UserClaims
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }

        public UserClaims(IEnumerable<Claim> claims)
        {
            FirstName = claims.FirstOrDefault(c => c.Type == "FirstName")?.Value;
            LastName = claims.FirstOrDefault(c => c.Type == "LastName")?.Value;
            UserName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            Role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "guest";
            Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }
    }
}
