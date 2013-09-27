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

        public void UpdateChannel(User user, string channelName, string title, string game)
        {
            if (string.IsNullOrEmpty(user.GetAccessToken(UserScope.ChannelEditor)))
            {
                throw new ArgumentNullException("AccessToken must be present for PUT requests.");
            }

            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException("Channel name must be present for PUT requests.");
            }

            Channel channel = new Channel()
            {
                Title = title,
                CurrentStream = new Stream()
                {
                    Game = game
                }
            };

            twitchRequest.AccessToken = user.GetAccessToken(UserScope.ChannelEditor);
            JObject response = JObject.Parse(twitchRequest.MakeRequest(RequestType.Put, "/" + channelName, ToJson(channel)));

            if (response["error"] != null)
            {
                throw new Exception(string.Format("UpdateChannel failed.  Error {0} {1}: {2}", response["status"], response["error"], response["message"]));
            }
        }

        public void RunCommercial(User user, string channelName, CommercialLength commerciallength)
        {
            if (string.IsNullOrEmpty(user.GetAccessToken(UserScope.ChannelEditor)))
            {
                throw new ArgumentNullException("AccessToken must be present for POST requests.");
            }

            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException("Channel name must be present for POST requests.");
            }

            int length = 60;

            switch (commerciallength)
            {
                case CommercialLength.ThirtySeconds:
                    length = 30;
                    break;
                case CommercialLength.SixtySeconds:
                    length = 60;
                    break;
                case CommercialLength.NinetySeconds:
                    length = 90;
                    break;
            }

            twitchRequest.AccessToken = user.GetAccessToken(UserScope.ChannelEditor);
            JObject response = JObject.Parse(twitchRequest.MakeRequest(RequestType.Post, "/" + channelName + "/commercial", "length=" + length.ToString()));

            if (response["error"] != null)
            {
                throw new Exception(string.Format("RunCommercial failed.  Error {0} {1}: {2}", response["status"], response["error"], response["message"]));
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
