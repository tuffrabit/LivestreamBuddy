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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LivestreamBuddy;

namespace LivestreamBuddyNew
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Awesomium.Core.WebCore.Initialize(new Awesomium.Core.WebConfig());

            visibleStreams = new List<string>();
            user = new User();

            channelsControl.OnStreamOpen += channelsControl_OnStreamOpen;
        }

        # region Private Properties

        private List<string> visibleStreams;
        private User user;

        # endregion

        # region Private Methods

        private bool isStreamVisible(string channelName)
        {
            bool visible = false;

            foreach (string stream in visibleStreams)
            {
                if (string.Compare(stream, channelName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    visible = true;
                    break;
                }
            }

            return visible;
        }

        # endregion

        # region Events

        void channelsControl_OnStreamOpen(object sender, Controls.StreamOpenEventArgs e)
        {
            if (e != null && !string.IsNullOrEmpty(e.ChannelName) && !isStreamVisible(e.ChannelName))
            {
                if (user.GetAccessToken(UserScope.ChatLogin) == null)
                {
                    AuthWindow authWindow = new AuthWindow(user, UserScope.ChatLogin);
                    authWindow.ShowDialog();

                    user.AccessTokens.Add(UserScope.ChatLogin, authWindow.AccessToken);
                }

                if (user.GetAccessToken(UserScope.ChatLogin) != null && !string.IsNullOrEmpty(user.Name))
                {
                    if (e.OpenInNewTab)
                    {
                        mainTabs.Items.Add(new TabItem { Header = e.ChannelName, Content = new Controls.Stream() });
                    }
                    else
                    {
                        IconBitmapDecoder ibd = new IconBitmapDecoder(new Uri("pack://application:,,,/LivestreamBuddy;component/livestream-ICON.ico"), BitmapCreateOptions.None, BitmapCacheOption.Default);

                        Window newWindow = new Window
                        {
                            Width = 525,
                            Height = 425,
                            Title = e.ChannelName,
                            Icon = ibd.Frames[0],
                            Content = new Border { Padding = new Thickness(13, 13, 13, 13), Child = new Controls.Stream() }
                        };

                        newWindow.ShowDialog();
                    }

                    visibleStreams.Add(e.ChannelName);
                }
            }
        }

        # endregion
    }
}
