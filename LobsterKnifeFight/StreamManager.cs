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

namespace LobsterKnifeFight
{
    public class StreamManager : TwitchManager<Stream>
    {
        public StreamManager()
        {
            twitchRequest = new TwitchRequest("streams");
        }

        public LobsterKnifeFight.Stream GetStream(string channelName)
        {
            LobsterKnifeFight.Stream stream = null;
            TwitchRequestResult result = twitchRequest.MakeRequest(RequestType.Get, "/" + channelName, null);

            switch (result.Result)
            {
                case TwitchRequestResults.Success:
                    JObject rawStream = JObject.Parse(result.Data);

                    if (rawStream["error"] != null)
                    {
                        throw new Exception(string.Format("GetStream failed.  Error {0} {1}: {2}", rawStream["status"], rawStream["error"], rawStream["message"]));
                    }
                    else if (rawStream["stream"] != null && rawStream["stream"].HasValues)
                    {
                        stream = new LobsterKnifeFight.Stream
                        {
                            IsOnline = true,
                            Id = (Int64)rawStream["stream"]["_id"],
                            Game = (string)rawStream["stream"]["game"],
                            ViewerCount = (Int64)rawStream["stream"]["viewers"],
                            Channel = new LobsterKnifeFight.Channel
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
                        stream = new LobsterKnifeFight.Stream
                        {
                            IsOnline = false
                        };
                    }

                    break;
                case TwitchRequestResults.Unauthorized:
                    throw new TwitchRequestUnauthorizedException();
                    break;
            }

            return stream;
        }

        /// <summary>
        /// Returns a list of twitch streams
        /// </summary>
        /// <param name="channels">Comma separated list of channels to query</param>
        /// <returns>A list of twitch streams</returns>
        public List<LobsterKnifeFight.Stream> GetStreams(string channels)
        {
            List<LobsterKnifeFight.Stream> streams = new List<LobsterKnifeFight.Stream>();
            TwitchRequestResult result = twitchRequest.MakeRequest(RequestType.Get, "?channel=" + channels, null);

            switch (result.Result)
            {
                case TwitchRequestResults.Success:
                    JObject rawStreams = JObject.Parse(result.Data);

                    if (rawStreams["error"] != null)
                    {
                        throw new Exception(string.Format("GetStreams failed.  Error {0} {1}: {2}", rawStreams["status"], rawStreams["error"], rawStreams["message"]));
                    }
                    else if (rawStreams["streams"] != null && rawStreams["streams"].HasValues)
                    {
                        foreach (JToken child in rawStreams["streams"].ToArray())
                        {
                            LobsterKnifeFight.Stream stream = null;

                            stream = new LobsterKnifeFight.Stream
                            {
                                IsOnline = true,
                                Id = (Int64)child["_id"],
                                Game = (string)child["game"],
                                ViewerCount = (Int64)child["viewers"],
                                Channel = new LobsterKnifeFight.Channel
                                {
                                    Id = (Int64)child["channel"]["_id"],
                                    Name = (string)child["channel"]["name"],
                                    Title = (string)child["channel"]["status"]
                                }
                            };

                            streams.Add(stream);
                        }
                    }

                    break;
                case TwitchRequestResults.Unauthorized:
                    throw new TwitchRequestUnauthorizedException();
                    break;
            }

            return streams;
        }

        public List<LobsterKnifeFight.Stream> GetFollowedStreams(User user)
        {
            if (string.IsNullOrEmpty(user.AccessToken))
            {
                throw new ArgumentNullException("AccessToken must be present for POST requests.");
            }

            List<LobsterKnifeFight.Stream> streams = new List<LobsterKnifeFight.Stream>();
            TwitchRequestResult result = twitchRequest.MakeRequest(RequestType.Get, "/followed", null, user.AccessToken);

            switch (result.Result)
            {
                case TwitchRequestResults.Success:
                    JObject rawFollowedStreams = JObject.Parse(result.Data);

                    if (rawFollowedStreams["error"] != null)
                    {
                        throw new Exception(string.Format("GetFollowedStreams failed.  Error {0} {1}: {2}", rawFollowedStreams["status"], rawFollowedStreams["error"], rawFollowedStreams["message"]));
                    }
                    else if (rawFollowedStreams["streams"] != null && rawFollowedStreams["streams"].HasValues)
                    {
                        foreach (JToken child in rawFollowedStreams["streams"].ToArray())
                        {
                            LobsterKnifeFight.Stream stream = new LobsterKnifeFight.Stream
                            {
                                IsOnline = true,
                                Id = (Int64)child["_id"],
                                Game = (string)child["game"],
                                ViewerCount = (Int64)child["viewers"],
                                Channel = new LobsterKnifeFight.Channel
                                {
                                    Id = (Int64)child["channel"]["_id"],
                                    Name = (string)child["channel"]["name"],
                                    Title = (string)child["channel"]["status"]
                                }
                            };

                            streams.Add(stream);
                        }
                    }

                    break;
                case TwitchRequestResults.Unauthorized:
                    throw new TwitchRequestUnauthorizedException();
                    break;
            }

            return streams;
        }

        public List<LobsterKnifeFight.Stream> GetFeaturedStreams()
        {
            List<LobsterKnifeFight.Stream> streams = new List<LobsterKnifeFight.Stream>();
            TwitchRequestResult result = twitchRequest.MakeRequest(RequestType.Get, "/featured", null);

            switch (result.Result)
            {
                case TwitchRequestResults.Success:
                    JObject rawFeaturedStreams = JObject.Parse(result.Data);

                    if (rawFeaturedStreams["error"] != null)
                    {
                        throw new Exception(string.Format("GetFeaturedStreams failed.  Error {0} {1}: {2}", rawFeaturedStreams["status"], rawFeaturedStreams["error"], rawFeaturedStreams["message"]));
                    }
                    else if (rawFeaturedStreams["featured"] != null && rawFeaturedStreams["featured"].HasValues)
                    {
                        foreach (JToken child in rawFeaturedStreams["featured"].ToArray())
                        {
                            LobsterKnifeFight.Stream stream = new LobsterKnifeFight.Stream
                            {
                                IsOnline = true,
                                Id = (Int64)child["stream"]["_id"],
                                Game = (string)child["stream"]["game"],
                                ViewerCount = (Int64)child["stream"]["viewers"],
                                Channel = new LobsterKnifeFight.Channel
                                {
                                    Id = (Int64)child["stream"]["channel"]["_id"],
                                    Name = (string)child["stream"]["channel"]["name"],
                                    Title = (string)child["stream"]["channel"]["status"]
                                }
                            };

                            streams.Add(stream);
                        }
                    }

                    break;
                case TwitchRequestResults.Unauthorized:
                    throw new TwitchRequestUnauthorizedException();
                    break;
            }

            return streams;
        }

        public override string ToJson(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
