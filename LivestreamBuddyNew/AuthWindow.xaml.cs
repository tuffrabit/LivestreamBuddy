using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Awesomium.Core;
using LivestreamBuddy;

namespace LivestreamBuddyNew
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();

            webBrowser.NativeViewInitialized += webBrowser_NativeViewInitialized;
            webBrowser.DocumentReady += webBrowser_DocumentReady;
            webBrowser.LoadingFrame += webBrowser_LoadingFrame;
        }

        public AuthWindow(User user, UserScope scope)
            : this()
        {
            this.user = user;
            this.Scope = scope;
        }

        # region Public Properties

        public string AccessToken { get; private set; }

        public UserScope Scope { get; private set; }

        # endregion

        # region Private Properties

        private User user;

        # endregion

        # region Events

        void webBrowser_NativeViewInitialized(object sender, WebViewEventArgs e)
        {
            webBrowser.LoadHTML("<html><body><h3>Loading...</h3></body></html>");

            string userScope = EnumHelper.GetUserScope(this.Scope);

            webBrowser.Source = new Uri("https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id=a13o59im5mfpi5y8afoc3jer8vidva0&redirect_uri=http://www.google.com&scope=" + userScope);
        }

        void webBrowser_LoadingFrame(object sender, Awesomium.Core.LoadingFrameEventArgs e)
        {
            if (webBrowser.IsDocumentReady)
            {
                dynamic document = (JSObject)webBrowser.ExecuteJavascriptWithResult("document");

                if (document != null)
                {
                    using (document)
                    {
                        dynamic userLoginField = document.getElementById("user_login");

                        if (userLoginField != null && string.IsNullOrEmpty(user.Name))
                        {
                            user.Name = userLoginField.value;
                        }
                    }
                }
            }
        }

        void webBrowser_DocumentReady(object sender, Awesomium.Core.UrlEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Url.Fragment))
            {
                int keyIndex = e.Url.Fragment.IndexOf("access_token=");

                if (keyIndex > -1)
                {
                    int endIndex = e.Url.Fragment.IndexOf('&', keyIndex) - 14;

                    keyIndex += 13;
                    this.AccessToken = e.Url.Fragment.Substring(keyIndex, endIndex);

                    this.Close();
                }
            }
        }

        # endregion
    }
}
