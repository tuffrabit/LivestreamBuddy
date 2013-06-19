using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestreamBuddy
{
    public class UserManager
    {
        private TwitchRequest twitchRequest;

        public UserManager()
        {
            twitchRequest = new TwitchRequest("oauth2/token");
        }
    }
}
