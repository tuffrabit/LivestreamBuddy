using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestreamBuddy
{
    public static class EnumHelper
    {
        public static string GetUserScope(UserScope userScope)
        {
            string returnValue = string.Empty;

            switch (userScope)
            {
                case UserScope.UserRead:
                    returnValue = "user_read";
                    break;
                case UserScope.UserBlocksEdit:
                    returnValue = "user_blocks_edit";
                    break;
                case UserScope.UserBlocksRead:
                    returnValue = "user_blocks_read";
                    break;
                case UserScope.UserFollowsEdit:
                    returnValue = "user_follows_edit";
                    break;
                case UserScope.ChannelRead:
                    returnValue = "channel_read";
                    break;
                case UserScope.ChannelEditor:
                    returnValue = "channel_editor";
                    break;
                case UserScope.ChannelCommercial:
                    returnValue = "channel_commercial";
                    break;
                case UserScope.ChannelStream:
                    returnValue = "channel_stream";
                    break;
                case UserScope.ChannelSubscriptions:
                    returnValue = "channel_subscriptions";
                    break;
                case UserScope.ChannelCheckSubscription:
                    returnValue = "channel_check_subscriptions";
                    break;
                case UserScope.ChatLogin:
                    returnValue = "chat_login";
                    break;
            }

            return returnValue;
        }
    }
}
