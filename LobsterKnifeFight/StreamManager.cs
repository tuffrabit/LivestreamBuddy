using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LivestreamBuddy
{
    public class StreamManager
    {
        private TwitchRequest twitchRequest;

        public StreamManager()
        {
            twitchRequest = new TwitchRequest("streams");
        }

        public LivestreamBuddy.Stream GetStream(string channelName)
        {
            LivestreamBuddy.Stream stream = new LivestreamBuddy.Stream();
            JObject rawStream = JObject.Parse(twitchRequest.MakeRequest(RequestType.Get, channelName, null));

            if (rawStream["stream"] != null && rawStream["stream"].HasValues)
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
                        Title = (string)rawStream["stream"]["channel"]["status"],
                    }
                };
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
    }
}
