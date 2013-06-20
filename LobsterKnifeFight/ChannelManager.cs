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
using Newtonsoft.Json.Linq;

namespace LivestreamBuddy
{
    public class ChannelManager : TwitchManager<Channel>
    {
        public ChannelManager()
        {
            twitchRequest = new TwitchRequest("channels");
        }

        public void UpdateChannel(User user, Channel channel)
        {
            if (string.IsNullOrEmpty(user.AccessToken))
            {
                throw new ArgumentNullException("AccessToken must be present for PUT requests.");
            }

            if (string.IsNullOrEmpty(user.UserId))
            {
                throw new ArgumentNullException("UserId must be present for PUT requests.");
            }

            twitchRequest.AccessToken = user.AccessToken;
            JObject response = JObject.Parse(twitchRequest.MakeRequest(RequestType.Put, user.UserId, ToJson(channel)));

            if (response["error"] != null)
            {
                throw new Exception(string.Format("GetStream failed.  Error {0} {1}: {2}", response["status"], response["error"], response["message"]));
            }
        }

        public override string ToJson(Channel channel)
        {
            JObject o = JObject.FromObject(new
            {
                channel = new
                {
                    status = channel.Title, 
                    game = channel.CurrentStream.Game
                }
            });

            return o.ToString();
        }
    }
}
