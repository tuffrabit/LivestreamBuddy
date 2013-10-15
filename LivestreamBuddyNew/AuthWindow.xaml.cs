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
using LobsterKnifeFight;

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

            this.saveAccessToken = true;

            webBrowser.NativeViewInitialized += webBrowser_NativeViewInitialized;
            webBrowser.DocumentReady += webBrowser_DocumentReady;
            webBrowser.LoadingFrame += webBrowser_LoadingFrame;
        }

        public AuthWindow(User user, UserScope[] scopes)
            : this()
        {
            this.user = user;
            this.Scopes = scopes;
        }

        # region Public Properties

        public string AccessToken { get; private set; }

        public UserScope[] Scopes { get; private set; }

        # endregion

        # region Private Properties

        private User user;
        private bool saveAccessToken;

        # endregion

        # region Events

        void webBrowser_NativeViewInitialized(object sender, WebViewEventArgs e)
        {
            webBrowser.LoadHTML("<html><body><h3>Loading...</h3></body></html>");

            string userscopes = string.Empty;

            for (int i = 0; i < Scopes.Length; i++)
            {
                UserScope scope = Scopes[i];

                if (i + 1 < Scopes.Length)
                {
                    userscopes += EnumHelper.GetUserScope(scope) + " ";
                }
                else
                {
                    userscopes += EnumHelper.GetUserScope(scope);
                }
            }

            webBrowser.Source = new Uri("https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id=a13o59im5mfpi5y8afoc3jer8vidva0&redirect_uri=http://www.google.com&scope=" + userscopes);
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

                        if (!Object.ReferenceEquals(null, userLoginField))
                        {
                            try
                            {
                                string value = (string)userLoginField.value;

                                if (!string.IsNullOrEmpty(value))
                                {
                                    if (string.IsNullOrEmpty(user.Name))
                                    {
                                        user.Name = value;
                                    }
                                    else if (string.Compare(user.Name, value, StringComparison.OrdinalIgnoreCase) != 0)
                                    {
                                        this.saveAccessToken = false;
                                    }
                                }
                            }
                            catch { this.saveAccessToken = true; }
                        }
                    }
                }
            }
        }

        void webBrowser_DocumentReady(object sender, Awesomium.Core.UrlEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Url.Fragment))
            {
                if (this.saveAccessToken)
                {
                    int keyIndex = e.Url.Fragment.IndexOf("access_token=");

                    if (keyIndex > -1)
                    {
                        int endIndex = e.Url.Fragment.IndexOf('&', keyIndex) - 14;

                        keyIndex += 13;
                        this.AccessToken = e.Url.Fragment.Substring(keyIndex, endIndex);
                    }

                    this.DialogResult = true;
                }
                else
                {
                    this.DialogResult = false;
                }

                this.Close();
            }
            else if (e.Url.Host.ToLower().Contains("google"))
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        # endregion
    }
}
