using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestreamBuddyNew
{
    public enum IRCErrors
    {
        LoginUnsuccessful, 
        ListenThreadError, 
        JoinError, 
        PartError, 
        ChannelMessageError, 
        NamesError, 
        ModeError
    }
}
