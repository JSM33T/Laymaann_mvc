using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laymaann.Entities.Shared
{
    public class RateLimitingOptions
    {
        public RateLimitPolicyOptions Global { get; set; }
        public Dictionary<string, RateLimitPolicyOptions> Routes { get; set; }
    }

}
