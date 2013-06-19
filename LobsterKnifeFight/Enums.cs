using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestreamBuddy
{
    public enum RequestType
    {
        Get, 
        Put, 
        Delete, 
        Auth
    }

    public enum UserScope
    {
        UserRead, 
        UserBlocksEdit, 
        UserBlocksRead, 
        UserFollowsEdit, 
        ChannelRead, 
        ChannelEditor, 
        ChannelCommercial, 
        ChannelStream, 
        ChannelSubscriptions, 
        ChannelCheckSubscription, 
        ChatLogin
    }
}
