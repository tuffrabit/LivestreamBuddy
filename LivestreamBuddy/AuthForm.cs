using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LivestreamBuddy
{
    public partial class AuthForm : Form
    {
        private User user;
        private string url;

        public AuthForm()
        {
            InitializeComponent();
        }

        public AuthForm(User user) : this()
        {
            this.user = user;
            string userScope = EnumHelper.GetUserScope(user.Scope);
            url = "https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id=a13o59im5mfpi5y8afoc3jer8vidva0&redirect_uri=http://localhost&scope=" + userScope;

            webBrowser.Navigate(url);
            //webBrowser.Navigating += webBrowser_Navigating;
            webBrowser.Navigated += webBrowser_Navigated;
        }

        void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Url.OriginalString.Contains("api.twitch.tv/kraken/oauth2/allow"))
            {
                //webBrowser.Navigate(url);
                HttpWebRequest request = WebRequest.Create("https://api.twitch.tv/kraken/oauth2/token") as HttpWebRequest;
                string postData = "client_id=a13o59im5mfpi5y8afoc3jer8vidva0&client_secret=tre25a95srpev7j03kvoslxt93ysdt4&";

                //request.Headers.Add("Client-ID: a13o59im5mfpi5y8afoc3jer8vidva0");

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    /*StreamReader responseReader = new StreamReader(response.GetResponseStream());

                    returnValue = responseReader.ReadToEnd();*/
                }
            }
        }

        void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            //throw new NotImplementedException();
            /*if (e.Url.OriginalString.Contains("api.twitch.tv/kraken/oauth2/allow"))
            {
                webBrowser.Navigate(url);
            }*/
        }
    }
}
