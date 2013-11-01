using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
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
using Newtonsoft.Json;

namespace LivestreamBuddyNew.Controls
{
    /// <summary>
    /// Interaction logic for Stream.xaml
    /// </summary>
    public partial class Stream : UserControl
    {
        public Stream()
        {
            InitializeComponent();

            viewModel = new StreamViewModel();
            DataContext = viewModel;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
            this.options = DataFileManager.GetOptions();
            viewStreamWindow = null;

            nicknameColors = new Dictionary<string, string>();
            lastColorUsed = -1;
            urlRegex = new Regex(@"(?i)\b((?:[a-z][\w-]+:(?:/{1,3}|[a-z0-9%])|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'.,<>?«»“”‘’]))", RegexOptions.Compiled);
            excludeList = new List<string>();

            webChat.NativeViewInitialized += webChat_NativeViewInitialized;
            webChat.SizeChanged += webChat_SizeChanged;

            viewStreamPreviousHeight = 0;
        }

        public Stream(User user,
            string channelName,
            string accessToken,
            string[] potentialNicknameColors,
            List<string> streamTitleAutoCompleteOptions,
            List<string> streamGameAutoCompleteOptions,
            List<Emoticon> emoticons)
            : this()
        {
            this.user = user;
            this.channelName = channelName;
            this.accessToken = accessToken;
            this.potentialNicknameColors = potentialNicknameColors;
            this.streamTitleAutoCompleteOptions = streamTitleAutoCompleteOptions;
            this.streamGameAutoCompleteOptions = streamGameAutoCompleteOptions;
            this.emoticons = emoticons;

            viewModel.TitleAutoCompleteOptions = streamTitleAutoCompleteOptions;
            viewModel.GameAutoCompleteOptions = streamGameAutoCompleteOptions;

            if (this.options.ShowStreamFeedWhenOpening)
            {
                pnlViewStream.Visibility = System.Windows.Visibility.Visible;
                btnShowHideViewStream.Content = "Hide";
                viewStream = new ViewStream(channelName, false, viewStreamMinimumHeight);
            }
            else
            {
                pnlViewStream.Visibility = System.Windows.Visibility.Collapsed;
                btnShowHideViewStream.Content = "Show";
                viewStream = new ViewStream(channelName, false, viewStreamMinimumHeight, false);
                viewStreamPreviousHeight = viewStreamMinimumHeight;
            }

            pnlViewStream.Children.Add(viewStream);
        }

        # region Public Methods

        public void Disconnect()
        {
            viewStream.Shutdown();

            if (viewStreamWindow != null)
            {
                viewStreamWindow.Close();
            }

            if (this.client != null)
            {
                if (!webChat.IsDisposed)
                {
                    webChat.Dispose();
                }

                this.client.Disconnect();
            }
        }

        # endregion

        # region Private Methods

        private void addToViewers(string newViewer)
        {
            addToViewers(new string[] { newViewer });
        }

        private void addToViewers(string[] newViewers)
        {
            List<string> viewers = new List<string>();

            if (viewModel.Viewers != null)
            {
                viewers.AddRange(viewModel.Viewers);
                viewers.AddRange(newViewers.Where(v => !viewers.Contains(v)));
            }
            else
            {
                viewers.AddRange(newViewers);
            }

            viewers.Sort();
            viewModel.Viewers = viewers;
            lblViewerCount.Text = viewers.Count.ToString();
        }

        private void removeFromViewers(string viewer)
        {
            if (viewModel.Viewers != null)
            {
                List<string> viewers = new List<string>();
                viewers.AddRange(viewModel.Viewers);
                viewers.RemoveAll(v => string.Compare(v, viewer, StringComparison.OrdinalIgnoreCase) == 0);

                viewModel.Viewers = viewers;
                lblViewerCount.Text = viewers.Count.ToString();
            }
        }

        private string getNextColor()
        {
            lastColorUsed++;

            if (lastColorUsed > (potentialNicknameColors.Length - 1))
            {
                lastColorUsed = 0;
            }

            return potentialNicknameColors[lastColorUsed];
        }

        private void writeChatLine(string nickname, string message, string nickColor)
        {
            if (!webChat.IsDisposed)
            {
                string timestamp = string.Empty;

                if (this.options.ShowTimestampsInChat)
                {
                    DateTime now = DateTime.Now;
                    timestamp = string.Format("[{0}]", now.ToString("H:mm"));
                }

                webChat.ExecuteJavascript("chatMessageTwo(\"" + timestamp + "\", \"" + nickname + "\", \"" + HttpUtility.JavaScriptStringEncode(message) + "\", \"" + nickColor + "\");");
            }
        }

        private void showHideStreamFeed(bool show)
        {
            if (!show)
            {
                viewStreamPreviousHeight = pnlViewStream.ActualHeight;
                pnlViewStream.Visibility = System.Windows.Visibility.Collapsed;
                viewStream.Hide();
                btnShowHideViewStream.Content = "Show";
            }
            else
            {
                pnlViewStream.Visibility = System.Windows.Visibility.Visible;
                viewStream.Show();

                if (viewStreamPreviousHeight == 0)
                {
                    viewStreamPreviousHeight = pnlMainDock.ActualHeight * .469;
                }
                else if (viewStreamPreviousHeight < viewStreamMinimumHeight)
                {
                    viewStreamPreviousHeight = viewStreamMinimumHeight;
                }

                pnlViewStream.Height = viewStreamPreviousHeight;
                btnShowHideViewStream.Content = "Hide";
            }
        }

        # endregion

        # region Private Members

        private User user;
        private LivestreamBuddyNew.Options options;
        private string channelName;
        private string accessToken;
        private IrcClientHelper client;
        private StreamViewModel viewModel;
        private string[] potentialNicknameColors;
        private Dictionary<string, string> nicknameColors;
        private int lastColorUsed;
        private List<string> streamTitleAutoCompleteOptions;
        private List<string> streamGameAutoCompleteOptions;
        private Regex urlRegex;
        private List<Emoticon> emoticons;
        private List<string> excludeList;
        private const double viewStreamMinimumHeight = 245;
        private double viewStreamPreviousHeight;
        private ViewStream viewStream;
        ViewStreamWindow viewStreamWindow;

        # endregion

        # region Event Handlers

        void webChat_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (webChat.IsDocumentReady)
            {
                webChat.ExecuteJavascript("resizeMessagesPanel(" + (webChat.ActualWidth - 10).ToString() + ", " + (webChat.ActualHeight - 10).ToString() + ")");
            }
        }

        void webChat_NativeViewInitialized(object sender, Awesomium.Core.WebViewEventArgs e)
        {
            webChat.DocumentReady += webChat_DocumentReady;

            if (webChat.IsLive)
            {
                string currentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                webChat.Source = new Uri("file://" + currentPath + "\\chat.html");
            }
        }

        void webChat_DocumentReady(object sender, Awesomium.Core.UrlEventArgs e)
        {
            webChat.DocumentReady -= webChat_DocumentReady;

            JSObject jsobject = webChat.CreateGlobalJavascriptObject("jsobject");
            jsobject.Bind("HostRaiseLinkClick", false, webChat_OnHostRaiseLinkClick);

            if (emoticons != null && options.ShowEmoticonsInChat)
            {
                webChat.ExecuteJavascript("var emoticons = " + JsonConvert.SerializeObject(emoticons) + ";");
            }
            else
            {
                webChat.ExecuteJavascript("var emoticons = [];");
            }

            webChat.ExecuteJavascript("resizeMessagesPanel(-1, -1)");
            webChat.ExecuteJavascript("document.getElementById('pnlMessages').style.fontSize=" + options.ChatTextSize.ToString() + ";");
            webChat.ExecuteJavascript("infoMessage('Joining channel...');");

            client = new IrcClientHelper();
            client.OnError += client_OnError;
            client.OnChannelJoin += client_OnChannelJoin;
            client.OnDisconnected += client_OnDisconnected;
            client.OnMessage += client_OnMessage;
            client.OnUserListCompleted += client_OnUserListCompleted;
            client.OnUserJoin += client_OnUserJoin;
            client.OnUserPart += client_OnUserPart;

            client.Connect(this.channelName, this.user.Name, this.accessToken);
        }

        void client_OnDisconnected(object sender, EventArgs e)
        {
            if (!webChat.IsDisposed)
            {
                webChat.ExecuteJavascript("errorMessage('Disconnected from chat. Close the stream chat and try re-opening it.');");
            }
        }

        void client_OnUserPart(object sender, IRCUserEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                removeFromViewers(e.Nickname);
            }));

        }

        void client_OnUserJoin(object sender, IRCUserEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                addToViewers(e.Nickname);
            }));
        }

        void client_OnUserListCompleted(object sender, IRCUserListEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                addToViewers(e.Nicknames);
            }));
        }

        void client_OnError(object sender, IRCErrorEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                switch (e.Error)
                {
                    case IRCErrors.LoginUnsuccessful:
                        webChat.ExecuteJavascript("errorMessage('Login unsuccessful.');");
                        Utility.GetAccessToken(this.user, true);

                        try
                        {
                            if (!string.IsNullOrEmpty(this.user.AccessToken))
                            {
                                webChat.ExecuteJavascript("infoMessage('Trying again...');");
                                client.Reconnect(this.user.AccessToken);
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                        catch
                        {
                            Utility.ClearUserData(this.user);

                            MessageBox.Show("Something went horribly wrong. Close the stream chat and try re-opening it.");
                        }

                        break;
                    case IRCErrors.ListenThreadError:
                        webChat.ExecuteJavascript("errorMessage('IrcClient crashed. Chat will not function until you close and then re-open the stream.');");

                        break;
                }
            }));
        }

        void client_OnChannelJoin(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                webChat.ExecuteJavascript("infoMessage('Success!');");
                webChat.ExecuteJavascript("clearMessages();");
            }));
        }

        void client_OnMessage(object sender, IRCMessageEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                string nickColor = "#000000";

                if (nicknameColors.ContainsKey(e.Nickname))
                {
                    nickColor = nicknameColors[e.Nickname];
                }
                else
                {
                    nickColor = getNextColor();
                    nicknameColors.Add(e.Nickname, nickColor);
                }

                writeChatLine(e.Nickname, e.Message, nickColor);
            }));
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (client != null && e.Key == Key.Enter)
            {
                client.SendMessage(txtMessage.Text);
                writeChatLine(this.user.Name, txtMessage.Text, "#000000");
                txtMessage.Clear();
            }
        }

        private void webChat_OnHostRaiseLinkClick(object sender, JavascriptMethodEventArgs args)
        {
            if (args.Arguments.Length > 0 && args.Arguments[0].IsString)
            {
                try
                {
                    Process.Start((string)args.Arguments[0]);
                }
                catch { }
            }
        }

        void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.Disconnect();
            }
        }

        private void UpdateStreamClick(object sender, RoutedEventArgs e)
        {
            Utility.GetAccessToken(this.user);

            try
            {
                if (!string.IsNullOrEmpty(this.user.AccessToken))
                {
                    ChannelManager channelManager = new ChannelManager();

                    try
                    {
                        channelManager.UpdateChannel(user, this.channelName, txtStreamTitle.Text, txtStreamGame.Text);
                        MessageBox.Show("Update successful.");

                        if (!streamTitleAutoCompleteOptions.Contains(txtStreamTitle.Text))
                        {
                            streamTitleAutoCompleteOptions.Add(txtStreamTitle.Text);
                            DataFileManager.AddStringToStreamTitleAutoComplete(txtStreamTitle.Text);
                            viewModel.TitleAutoCompleteOptions = streamTitleAutoCompleteOptions;
                        }

                        if (!streamGameAutoCompleteOptions.Contains(txtStreamGame.Text))
                        {
                            streamGameAutoCompleteOptions.Add(txtStreamGame.Text);
                            DataFileManager.AddStringToStreamGameAutoComplete(txtStreamGame.Text);
                            viewModel.GameAutoCompleteOptions = streamGameAutoCompleteOptions;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Update failed.");
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

        private void GiveawayClick(object sender, RoutedEventArgs e)
        {
            IconBitmapDecoder ibd = new IconBitmapDecoder(new Uri("pack://application:,,,/LivestreamBuddyNew;component/livestream-ICON.ico"), BitmapCreateOptions.None, BitmapCacheOption.Default);
            LinearGradientBrush brush = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FF515151"), Colors.LightGray, new Point(.5, 0), new Point(.5, 1));
            Controls.Giveaway giveaway = new Giveaway(viewModel.Viewers, ref excludeList);

            Window newWindow = new Window
            {
                Width = 525,
                Height = 425,
                Title = this.channelName + " Giveaway",
                Icon = ibd.Frames[0],
                Background = brush,
                Content = new Border { Padding = new Thickness(13, 13, 13, 13), Child = giveaway }
            };

            newWindow.Closing += delegate(object windowClosingSender, System.ComponentModel.CancelEventArgs windowClosingArgs)
            {
                giveaway.Finalize();
            };

            newWindow.Show();
        }

        private void pnlMainDock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Height > 0)
            {
                double newHeight = e.NewSize.Height * .469;

                if (newHeight >= viewStreamMinimumHeight)
                {
                    pnlViewStream.Height = newHeight;
                }
                else
                {
                    pnlViewStream.Height = viewStreamMinimumHeight;
                }
            }
        }

        private void btnShowHideViewStream_Click(object sender, RoutedEventArgs e)
        {
            showHideStreamFeed(!pnlViewStream.IsVisible);
        }

        private void btnPopoutViewStream_Click(object sender, RoutedEventArgs e)
        {
            viewStreamWindow = new ViewStreamWindow(this.channelName, viewStreamMinimumHeight);

            viewStreamWindow.Closed += delegate(object windowClosedSender, EventArgs windowClosedArgs)
            {
                showHideStreamFeed(true);
                btnShowHideViewStream.IsEnabled = true;
                btnPopoutViewStream.IsEnabled = true;
            };

            btnShowHideViewStream.IsEnabled = false;
            btnPopoutViewStream.IsEnabled = false;
            showHideStreamFeed(false);
            viewStreamWindow.Show();
        }

        private void RunCommercial(object sender, RoutedEventArgs e)
        {
            Utility.GetAccessToken(this.user);

            try
            {
                if (!string.IsNullOrEmpty(this.user.AccessToken))
                {
                    ChannelManager channelManager = new ChannelManager();

                    try
                    {
                        CommercialLength length = CommercialLength.SixtySeconds;

                        switch (cmbCommercialLength.SelectedIndex)
                        {
                            case 0:
                                length = CommercialLength.ThirtySeconds;
                                break;
                            case 1:
                                length = CommercialLength.SixtySeconds;
                                break;
                            case 2:
                                length = CommercialLength.NinetySeconds;
                                break;
                        }

                        channelManager.RunCommercial(user, this.channelName, length);
                        MessageBox.Show("Running commercial.");
                    }
                    catch
                    {
                        MessageBox.Show("Commercial run failed.");
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

        # endregion
    }

    public class StreamViewModel : INotifyPropertyChanged
    {
        private List<string> viewers;
        public List<string> Viewers
        {
            get { return viewers; }
            set
            {
                viewers = value;
                RaisePropertyChanged("Viewers");
            }
        }

        private List<string> titleAutoCompleteOptions;
        public List<string> TitleAutoCompleteOptions
        {
            get { return titleAutoCompleteOptions; }
            set
            {
                titleAutoCompleteOptions = value;
                RaisePropertyChanged("TitleAutoCompleteOptions");
            }
        }

        private List<string> gameAutoCompleteOptions;
        public List<string> GameAutoCompleteOptions
        {
            get { return gameAutoCompleteOptions; }
            set
            {
                gameAutoCompleteOptions = value;
                RaisePropertyChanged("GameAutoCompleteOptions");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string caller)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }
    }

    public class EmoticonMatch
    {
        public Match Match { get; set; }

        public string Replacement { get; set; }

        public EmoticonMatch() { }

        public EmoticonMatch(Match match, string replacement)
        {
            this.Match = match;
            this.Replacement = replacement;
        }
    }
}
