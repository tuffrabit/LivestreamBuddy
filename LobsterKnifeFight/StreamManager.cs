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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LivestreamBuddy
{
    public class StreamManager : TwitchManager<Stream>
    {
        public StreamManager()
        {
            twitchRequest = new TwitchRequest("streams");
        }

        public LivestreamBuddy.Stream GetStream(string channelName)
        {
            LivestreamBuddy.Stream stream = new LivestreamBuddy.Stream();
            JObject rawStream = JObject.Parse(twitchRequest.MakeRequest(RequestType.Get, channelName, null));

            if (rawStream["error"] != null)
            {
                throw new Exception(string.Format("GetStream failed.  Error {0} {1}: {2}", rawStream["status"], rawStream["error"], rawStream["message"]));
            }
            else if (rawStream["stream"] != null && rawStream["stream"].HasValues)
            {
                stream = new LivestreamBuddy.Stream
                {
                    IsOnline = true,
                    Id = (Int64)rawStream["stream"]["_id"],
                    Game = (string)rawStream["stream"]["game"],
                    ViewerCount = (Int64)rawStream["stream"]["viewers"],
                    Channel = new LivestreamBuddy.Channel
                    {
                        Id = (Int64)rawStream["stream"]["channel"]["_id"],
                        Name = (string)rawStream["stream"]["channel"]["name"],
                        Title = (string)rawStream["stream"]["channel"]["status"]
                    }
                };

                stream.Channel.CurrentStream = stream;
            }
            else
            {
                stream = new LivestreamBuddy.Stream
                {
                    IsOnline = false
                };
            }
            
            return stream;
        }

        public override string ToJson(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
