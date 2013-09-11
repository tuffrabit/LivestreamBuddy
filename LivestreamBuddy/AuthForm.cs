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

            Gecko.Xpcom.Initialize("xulrunner");
        }

        public AuthForm(User user) : this()
        {
            this.user = user;
            string userScope = EnumHelper.GetUserScope(user.Scope);
            url = "https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id=a13o59im5mfpi5y8afoc3jer8vidva0&redirect_uri=http://www.google.com&scope=" + userScope;

            webBrowser.HandleCreated += webBrowser_HandleCreated;
        }

        void webBrowser_HandleCreated(object sender, EventArgs e)
        {
            webBrowser.Navigate(url);
            webBrowser.Navigated += webBrowser_Navigated;
        }

        void webBrowser_Navigated(object sender, Gecko.GeckoNavigatedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Uri.Fragment))
            {
                int keyIndex = e.Uri.Fragment.IndexOf("access_token=");

                if (keyIndex > -1)
                {
                    int endIndex = e.Uri.Fragment.IndexOf('&', keyIndex) - 14;

                    keyIndex += 13;
                    user.AccessToken = e.Uri.Fragment.Substring(keyIndex, endIndex);

                    this.Close();
                }
            }
        }
    }
}
