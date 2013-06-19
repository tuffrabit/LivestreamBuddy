using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestreamBuddy
{
    public class User
    {
        public string UserId { get; set; }

        public string Password { get; set; }

        public string AccessToken { get; set; }

        public UserScope Scope { get; set; }
    }
}
