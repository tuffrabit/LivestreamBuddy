using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Awesomium.Core;
using LobsterKnifeFight;

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
            WebCore.Initialize(
                new Awesomium.Core.WebConfig()
                    {
                        PluginsPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                    }, 
                true);

            visibleStreams = new List<string>();
            user = DataFileManager.GetUser();

            channelsControl.User = this.user;
            channelsControl.OnStreamOpen += channelsControl_OnStreamOpen;

            potentialNicknameColors = new string[] { "#0000FF", "#FF0000", "#00FF00", "#9900CC", "#FF99CC", 
                                                        "#990000", "#3399FF", "#99CCFF", "#FF0033", "#33FF00", 
                                                        "#9933FF", "#FF3399", "#663300", "#669999", "#00FFFF", 
                                                        "#CC0000", "#FF9933", "#33F33", "#9999FF", "#CC0066", 
                                                        "#CC9966", "#FF6666", "#99FFCC", "#0033CC", "#666633"};

            streamTitleAutoCompleteOptions = new List<string>();
            streamTitleAutoCompleteOptions.AddRange(DataFileManager.GetStreamTitleAutoCompleteStrings());

            streamGameAutoCompleteOptions = new List<string>();
            streamGameAutoCompleteOptions.AddRange(DataFileManager.GetStreamGameAutoCompleteStrings());

            emoticons = null;

            bool allowEmoticons = true;
            bool refreshEmoticons = false;
            string[] commandLineArgs = Environment.GetCommandLineArgs();

            foreach (string arg in commandLineArgs)
            {
                if (string.Compare(arg, "-noemote", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    allowEmoticons = false;
                }
                else if (string.Compare(arg, "-refreshemoticons", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    refreshEmoticons = true;
                }
            }

            if (allowEmoticons)
            {
                try
                {
                    emoticons = DataFileManager.GetEmoticons(refreshEmoticons);
                }
                catch { }
            }
        }

        # region Private Properties

        private List<string> visibleStreams;
        private User user;
        private string[] potentialNicknameColors;
        private List<string> streamTitleAutoCompleteOptions;
        private List<string> streamGameAutoCompleteOptions;
        private List<Emoticon> emoticons;

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
                Utility.GetAccessToken(this.user);

                try
                {
                    if (!string.IsNullOrEmpty(user.AccessToken) && !string.IsNullOrEmpty(user.Name))
                    {
                        visibleStreams.Add(e.ChannelName);

                        if (e.OpenInNewTab)
                        {
                            Controls.Stream stream = new Controls.Stream(this.user, 
                                e.ChannelName, 
                                this.user.AccessToken,
                                e.ShowStreamFeed, 
                                this.potentialNicknameColors, 
                                this.streamTitleAutoCompleteOptions, 
                                this.streamGameAutoCompleteOptions, 
                                this.emoticons);

                            ClosableTab tab = new ClosableTab
                            {
                                Title = e.ChannelName,
                                VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch,
                                Content = stream
                            };

                            tab.Closing += delegate(object tabClosingSender, EventArgs tabClosingArgs)
                            {
                                visibleStreams.Remove(e.ChannelName);
                                stream.Disconnect();
                            };

                            mainTabs.Items.Add(tab);
                        }
                        else
                        {
                            Controls.Stream stream = new Controls.Stream(this.user, 
                                e.ChannelName, 
                                this.user.AccessToken, 
                                e.ShowStreamFeed, 
                                this.potentialNicknameColors, 
                                this.streamTitleAutoCompleteOptions,
                                this.streamGameAutoCompleteOptions,
                                this.emoticons);

                            IconBitmapDecoder ibd = new IconBitmapDecoder(new Uri("pack://application:,,,/LivestreamBuddyNew;component/livestream-ICON.ico"), BitmapCreateOptions.None, BitmapCacheOption.Default);
                            LinearGradientBrush brush = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FF515151"), Colors.LightGray, new Point(.5, 0), new Point(.5, 1));

                            Window newWindow = new Window
                            {
                                Width = 525,
                                MinWidth = 525,
                                Height = 675,
                                MinHeight = 675,
                                Title = e.ChannelName,
                                Icon = ibd.Frames[0],
                                Background = brush,
                                Content = new Border { Padding = new Thickness(13, 13, 13, 13), Child = stream }
                            };

                            newWindow.Closing += delegate(object windowClosingSender, System.ComponentModel.CancelEventArgs windowClosingArgs)
                            {
                                visibleStreams.Remove(e.ChannelName);
                                stream.Disconnect();
                            };

                            newWindow.Show();
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    Utility.ClearUserData(this.user);

                    MessageBox.Show("Something went wrong. Try again.");
                }
            }
        }

        # endregion
    }
}
