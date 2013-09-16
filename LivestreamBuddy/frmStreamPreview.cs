using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gecko;
using Gecko.DOM;

namespace LivestreamBuddy
{
    public partial class frmStreamPreview : Form
    {
        private string channel = string.Empty;

        public frmStreamPreview()
        {
            InitializeComponent();

            Gecko.Xpcom.Initialize("xulrunner");
            GeckoPreferences.Default["extensions.blocklist.enabled"] = false;
        }

        public frmStreamPreview(string channel)
            : this()
        {
            this.channel = channel;

            webBrowser.HandleCreated += webBrowser_HandleCreated;
            webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;

            this.Text = "Stream Preview: " + this.channel;
        }

        void webBrowser_DocumentCompleted(object sender, EventArgs e)
        {
            GeckoButtonElement doStuff = webBrowser.Document.GetElementById("doStuff") as GeckoButtonElement;

            doStuff.SetAttribute("onclick", "javascript:pnlChat_Resize(641, 361);");
            doStuff.Click();

            GeckoElement pnlChat = webBrowser.Document.GetElementById("pnlChat");
            GeckoElement objectNode = webBrowser.Document.CreateElement("object");
            GeckoElement allowFullScreenNode = webBrowser.Document.CreateElement("param");
            GeckoElement allowScriptAccessNode = webBrowser.Document.CreateElement("param");
            GeckoElement allowNetworkingNode = webBrowser.Document.CreateElement("param");
            GeckoElement movieNode = webBrowser.Document.CreateElement("param");
            GeckoElement flashvarsNode = webBrowser.Document.CreateElement("param");

            objectNode.SetAttribute("type", "application/x-shockwave-flash");
            objectNode.SetAttribute("height", "360");
            objectNode.SetAttribute("width", "640");
            objectNode.SetAttribute("id", "live_embed_player_flash");
            objectNode.SetAttribute("data", "http://www.twitch.tv/widgets/live_embed_player.swf?channel=" + this.channel + "");
            objectNode.SetAttribute("bgcolor", "#000000");

            allowFullScreenNode.SetAttribute("name", "allowFullScreen");
            allowFullScreenNode.SetAttribute("value", "false");

            allowScriptAccessNode.SetAttribute("name", "allowScriptAccess");
            allowScriptAccessNode.SetAttribute("value", "false");

            allowNetworkingNode.SetAttribute("name", "allowNetworking");
            allowNetworkingNode.SetAttribute("value", "all");

            movieNode.SetAttribute("name", "movie");
            movieNode.SetAttribute("value", "http://www.twitch.tv/widgets/live_embed_player.swf");

            flashvarsNode.SetAttribute("name", "flashvars");
            flashvarsNode.SetAttribute("value", "hostname=www.twitch.tv&channel=" + this.channel + "&auto_play=true&start_volume=25");

            objectNode.AppendChild(allowFullScreenNode);
            objectNode.AppendChild(allowScriptAccessNode);
            objectNode.AppendChild(allowNetworkingNode);
            objectNode.AppendChild(movieNode);
            objectNode.AppendChild(flashvarsNode);
            pnlChat.AppendChild(objectNode);
        }

        void webBrowser_HandleCreated(object sender, EventArgs e)
        {
            string currentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            webBrowser.Navigate("file://" + currentPath + "\\chat.html");
        }
    }
}
