/*Copyright (C) 2013 Robert A. Boucher Jr. (TuFFrabit)

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobsterKnifeFight
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
