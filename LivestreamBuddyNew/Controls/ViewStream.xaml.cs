using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Awesomium.Core;
using Awesomium.Windows.Controls;

namespace LivestreamBuddyNew.Controls
{
    /// <summary>
    /// Interaction logic for ViewStream.xaml
    /// </summary>
    public partial class ViewStream : UserControl
    {
        public ViewStream()
        {
            InitializeComponent();

            webViewStream.SizeChanged += webViewStream_SizeChanged;
            webViewStream.NativeViewInitialized += webViewStream_NativeViewInitialized;

            this.IsShowing = false;
        }

        public ViewStream(string channelName, bool isWindow, double minimumHeight, bool show = true)
            : this()
        {
            this.channelName = channelName;
            this.minimumHeight = minimumHeight;
            this.isWindow = isWindow;
            this.showOnLoad = show;

            if (isWindow)
            {
                webViewStream.ViewType = WebViewType.Window;
            }

            setWebViewStreamHeight(this.minimumHeight);
        }

        # region Private Members

        private string channelName;
        private double minimumHeight;
        private double previousHeight;
        private bool isWindow;
        private bool showOnLoad;

        # endregion

        # region Public Members

        public bool IsShowing { get; set; }

        # endregion

        # region Private Methods

        private void loadHTML()
        {
            webViewStream.DocumentReady += webViewStream_DocumentReady;

            string allowFullscreen = "false";

            if (this.isWindow)
            {
                allowFullscreen = "true";
            }

            if (this.showOnLoad)
            {
                this.IsShowing = true;
                webViewStream.LoadHTML("<html><head><script>function resizePlayer(width, height){var player=document.getElementById('live_embed_player_flash');if (width==-1){width=window.innerWidth - 16;}if (height==-1){height=window.innerHeight - 16;}player.style.width=width + 'px';player.style.maxWidth=width + 'px';player.style.height=height + 'px';player.style.maxHeight=height + 'px';}</script></head><body><object type='application/x-shockwave-flash' height='271' width='456' id='live_embed_player_flash' data='http://www.twitch.tv/widgets/live_embed_player.swf?channel=" + this.channelName + "' bgcolor='#000000'><param name='allowFullScreen' value='" + allowFullscreen + "'/><param name='allowScriptAccess' value='always'/><param name='allowNetworking' value='all'/><param name='movie' value='http://www.twitch.tv/widgets/live_embed_player.swf'/><param name='flashvars' value='hostname=www.twitch.tv&channel=" + this.channelName + "&auto_play=true&start_volume=25'/></object></body></html>");
            }
            else
            {
                Hide();
                this.showOnLoad = true;
            }
        }

        private void setWebViewStreamHeight(double height)
        {
            webViewStream.Height = height;
        }

        # endregion

        # region Public Methods

        public void Shutdown()
        {
            if (!webViewStream.IsDisposed)
            {
                webViewStream.Dispose();
            }
        }

        public void Hide()
        {
            this.IsShowing = false;
            webViewStream.LoadHTML("<html><head></head><body></body></html>");
        }

        public void Show()
        {
            loadHTML();
        }

        # endregion

        # region Event Handlers

        void webViewStream_NativeViewInitialized(object sender, WebViewEventArgs e)
        {
            INavigationInterceptor navigationInterceptor = webViewStream.GetService(typeof(INavigationInterceptor)) as INavigationInterceptor;

            if (navigationInterceptor != null)
            {
                navigationInterceptor.AddRule("*", NavigationRule.Deny);
            }

            loadHTML();
        }

        void webViewStream_DocumentReady(object sender, UrlEventArgs e)
        {
            webViewStream.DocumentReady -= webViewStream_DocumentReady;

            try
            {
                webViewStream.ExecuteJavascript("resizePlayer(-1, -1)");
            }
            catch { }
        }

        void webViewStream_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (webViewStream.IsDocumentReady)
            {
                webViewStream.ExecuteJavascript("resizePlayer(" + (webViewStream.ActualWidth - 16).ToString() + ", " + (webViewStream.ActualHeight - 16).ToString() + ")");
            }
        }

        private void UserControl_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Height > 0)
            {
                double newHeight = e.NewSize.Height;

                if (newHeight >= this.minimumHeight)
                {
                    setWebViewStreamHeight(newHeight);
                }
                else
                {
                    setWebViewStreamHeight(this.minimumHeight);
                }
            }
        }

        # endregion
    }
}
