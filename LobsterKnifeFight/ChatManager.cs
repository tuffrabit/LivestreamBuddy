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

namespace LobsterKnifeFight
{
    public class ChatManager : TwitchManager<Chat>
    {
        public ChatManager()
        {
            twitchRequest = new TwitchRequest("chat");
        }

        public Emoticon[] GetEmoticons()
        {
            List<Emoticon> emoticons = null;
            TwitchRequestResult result = twitchRequest.MakeRequest(RequestType.Get, "/emoticons", null);

            if (result.Result == TwitchRequestResults.Success)
            {
                JObject rawChat = JObject.Parse(result.Data);

                if (rawChat["error"] != null)
                {
                    throw new Exception(string.Format("GetEmoticons failed.  Error {0} {1}: {2}", rawChat["status"], rawChat["error"], rawChat["message"]));
                }
                else if (rawChat["emoticons"] != null && rawChat["emoticons"].HasValues)
                {
                    emoticons = new List<Emoticon>();

                    foreach (JToken child in rawChat["emoticons"].ToArray())
                    {
                        emoticons.Add(new Emoticon
                        {
                            Pattern = (string)child["regex"],
                            //Height = (Int32)child["images"][0]["height"],
                            //Width = (Int32)child["images"][0]["width"],
                            Url = (string)child["images"][0]["url"]
                            //Set = (Int32)child["images"][0]["emoticon_set"]
                        });
                    }
                }

                if (emoticons == null)
                {
                    return null;
                }
                else
                {
                    return emoticons.ToArray();
                }
            }

            return null;
        }

        public override string ToJson(Chat twitchObject)
        {
            throw new NotImplementedException();
        }
    }
}
