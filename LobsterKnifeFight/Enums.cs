﻿/*Copyright (C) 2013 Robert A. Boucher Jr. (TuFFrabit)

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
    public enum RequestType
    {
        Get, 
        Put, 
        Post, 
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

    public enum CommercialLength
    {
        ThirtySeconds,
        SixtySeconds,
        NinetySeconds
    }
}
