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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LobsterKnifeFight
{
    public class TwitchRequest
    {
        public string RequestDomain { get; set; }

        public TwitchRequest()
        {
            RequestDomain = string.Empty;
        }

        public TwitchRequest(string requestDomain)
        {
            RequestDomain = requestDomain;
        }

        private void writeDataToRequest(ref HttpWebRequest request, string data)
        {
            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(data);
            request.ContentLength = dataBuffer.Length;

            using (System.IO.Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(dataBuffer, 0, dataBuffer.Length);
            }
        }

        public TwitchRequestResult MakeRequest(RequestType requestType, string data, string body, string accessToken = null)
        {
            TwitchRequestResult returnValue = new TwitchRequestResult();

            if (!string.IsNullOrEmpty(RequestDomain))
            {
                string requestUrl = string.Format("https://api.twitch.tv/kraken/{0}{1}", RequestDomain, data);
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;

                request.Accept = "application/vnd.twitchtv.v2+json";
                request.ContentType = "application/json";
                request.Headers.Add("Client-ID: a13o59im5mfpi5y8afoc3jer8vidva0");

                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Add("Authorization: OAuth " + accessToken);
                }

                switch (requestType)
                {
                    case RequestType.Get:
                        request.Method = "GET";

                        break;
                    case RequestType.Put:
                        request.Method = "PUT";

                        byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(body);
                        request.ContentLength = dataBuffer.Length;

                        using (System.IO.Stream dataStream = request.GetRequestStream())
                        {
                            dataStream.Write(dataBuffer, 0, dataBuffer.Length);
                        }

                        break;
                    case RequestType.Post:
                        request.Method = "POST";

                        writeDataToRequest(ref request, body);

                        break;
                    case RequestType.Delete:
                        break;
                    case RequestType.Auth:
                        break;
                }

                try
                {
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        StreamReader responseReader = new StreamReader(response.GetResponseStream());

                        returnValue.Data = responseReader.ReadToEnd();
                        returnValue.Result = TwitchRequestResults.Success;
                    }
                }
                catch (WebException ex)
                {
                    string message = ex.Message.ToLower();

                    if (message.Contains("401") && message.Contains("unauthorized"))
                    {
                        returnValue.Data = string.Empty;
                        returnValue.Result = TwitchRequestResults.Unauthorized;
                    }
                }
            }
            else
            {

            }

            return returnValue;
        }
    }

    public class TwitchRequestResult
    {
        public string Data { get; set; }

        public TwitchRequestResults Result { get; set; }
    }

    public enum TwitchRequestResults
    {
        Success, 
        Unauthorized
    }
}
