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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LivestreamBuddy
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

        public string MakeRequest(RequestType requestType, string data, string body)
        {
            string returnValue = null;

            if (!string.IsNullOrEmpty(RequestDomain))
            {
                string requestUrl = "https://api.twitch.tv/kraken/" + RequestDomain;

                switch (requestType)
                {
                    case RequestType.Get:
                        requestUrl += "/" + data;

                        break;
                    case RequestType.Put:
                        break;
                    case RequestType.Delete:
                        break;
                    case RequestType.Auth:


                        break;
                }

                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;

                request.Headers.Add("Client-ID: a13o59im5mfpi5y8afoc3jer8vidva0");

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader responseReader = new StreamReader(response.GetResponseStream());

                    returnValue = responseReader.ReadToEnd();
                }
            }
            else
            {

            }

            return returnValue;
        }
    }
}
