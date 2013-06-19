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
