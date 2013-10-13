using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
using LobsterKnifeFight;

namespace LivestreamBuddyNew.Controls
{
    /// <summary>
    /// Interaction logic for Streams.xaml
    /// </summary>
    public partial class Channels : UserControl
    {
        public Channels()
        {
            InitializeComponent();
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            channels = new ObservableCollection<ChannelInfo>();
            onlineImage = new BitmapImage(new Uri("pack://application:,,,/LivestreamBuddyNew;component/Resources/check.png"));
            offlineImage = new BitmapImage(new Uri("pack://application:,,,/LivestreamBuddyNew;component/Resources/grayX.png"));

            addStreamToFavoritesList("teamxim");

            foreach (string channel in DataFileManager.GetFavoriteChannels())
            {
                addStreamToFavoritesList(channel, true);
            }

            streamManager = new StreamManager();

            grdChannels.ItemsSource = channels;
            firstReportDone = false;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();
        }

        public Channels(User user)
            : this()
        {
            this.User = user;
        }

        # region Public Properties

        public User User { get; set; }
        public delegate void StreamOpenHandler(object sender, StreamOpenEventArgs e);
        public event StreamOpenHandler OnStreamOpen;

        # endregion

        # region Private Properties

        private ObservableCollection<ChannelInfo> channels;
        private StreamManager streamManager;
        private BackgroundWorker worker;
        private BitmapImage onlineImage;
        private BitmapImage offlineImage;
        private bool firstReportDone;

        # endregion

        # region Private Methods

        private void openStream(string channelName)
        {
            if (OnStreamOpen != null)
            {
                StreamOpenEventArgs args = new StreamOpenEventArgs { ChannelName = channelName.Trim(), OpenInNewTab = (bool)chkStreamOpenStyle.IsChecked, ShowStreamFeed = (bool)chkStreamViewShow.IsChecked };
                OnStreamOpen(this, args);
            }
        }

        private bool streamExists(string channelName)
        {
            bool exists = false;

            foreach (ChannelInfo channelInfo in channels)
            {
                if (string.Compare(channelInfo.Name, channelName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }

        private void addStreamToFavoritesList(string streamName, bool isFavorite = false)
        {
            if (!streamExists(streamName))
            {
                channels.Add(new ChannelInfo { Name = streamName, IsFavoriteChannel = isFavorite });
            }
        }

        private void addStreamToFavoritesList(LobsterKnifeFight.Stream stream, bool isFavorite = false)
        {
            if (!streamExists(stream.Channel.Name))
            {
                BitmapImage indicator = offlineImage;

                if (stream.IsOnline)
                {
                    indicator = onlineImage;
                }

                channels.Add(new ChannelInfo
                    {
                        Name = stream.Channel.Name,
                        StreamTitle = stream.Channel.Title,
                        Game = stream.Game,
                        Viewers = stream.ViewerCount.ToString(),
                        OnlineIndicator = indicator, 
                        IsFavoriteChannel = isFavorite
                    });
            }
        }

        private string getChannelsFromList()
        {
            string channelsList = string.Empty;

            foreach (ChannelInfo channel in channels)
            {
                channelsList += channel.Name + ",";
            }

            return channelsList;
        }

        private void workerCheckChannelsStatus()
        {
            string channels = getChannelsFromList();
            List<LobsterKnifeFight.Stream> streams = null;

            if (!string.IsNullOrEmpty(channels))
            {
                streams = streamManager.GetStreams(channels);

                if (streams != null)
                {
                    worker.ReportProgress(0, streams);
                }
            }
        }

        # endregion

        # region Events

        private void btnAddToFavoriteChannel_Click(object sender, RoutedEventArgs e)
        {
            SingleTextBoxWindow window = new SingleTextBoxWindow("Add Favorite Channel", "Channel: ", "Add");

            window.Owner = Application.Current.MainWindow;

            if ((bool)window.ShowDialog())
            {
                DataFileManager.AddFavoriteChannel(window.Value);

                addStreamToFavoritesList(window.Value.ToLower(), true);
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            workerCheckChannelsStatus();

            while (!worker.CancellationPending)
            {
                Thread.Sleep(60000);

                workerCheckChannelsStatus();
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            List<LobsterKnifeFight.Stream> streams = e.UserState as List<LobsterKnifeFight.Stream>;

            foreach (ChannelInfo channel in channels)
            {
                bool found = false;

                foreach (LobsterKnifeFight.Stream stream in streams)
                {
                    if (string.Compare(channel.Name, stream.Channel.Name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        channel.StreamTitle = stream.Channel.Title;
                        channel.Game = stream.Game;
                        channel.Viewers = stream.ViewerCount.ToString();
                        channel.OnlineIndicator = onlineImage;

                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    channel.StreamTitle = string.Empty;
                    channel.Game = string.Empty;
                    channel.Viewers = string.Empty;
                    channel.OnlineIndicator = offlineImage;
                }
            }

            if (!firstReportDone)
            {
                btnShowFeaturedStreams.IsEnabled = true;
                btnShowFollowedStreams.IsEnabled = true;
                btnAddToFavoriteChannel.IsEnabled = true;
                btnGoToChannel.IsEnabled = true;
                firstReportDone = true;
            }
        }

        void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            worker.CancelAsync();
        }

        private void btnGoToChannel_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtGoToChannel.Text))
            {
                MessageBox.Show("You must provide a channel name.");
            }
            else
            {
                openStream(txtGoToChannel.Text.ToLower());
            }
        }

        private void grdChannels_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ChannelInfo channel = null;

            try
            {
                channel = (ChannelInfo)grdChannels.SelectedItem;
            }
            catch { }

            if (channel != null)
            {
                openStream(channel.Name);
            }
        }

        private void ShowFeaturedStreamsClick(object sender, RoutedEventArgs e)
        {
            foreach (LobsterKnifeFight.Stream stream in streamManager.GetFeaturedStreams())
            {
                addStreamToFavoritesList(stream);
            }
        }

        private void ShowFollowedStreamsClick(object sender, RoutedEventArgs e)
        {
            Utility.GetAccessToken(this.User);

            try
            {
                if (!string.IsNullOrEmpty(User.AccessToken))
                {
                    foreach (LobsterKnifeFight.Stream stream in streamManager.GetFollowedStreams(this.User))
                    {
                        addStreamToFavoritesList(stream);
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                Utility.ClearUserData(this.User);

                MessageBox.Show("Something went wrong. Try again.");
            }
        }

        private void RemoveChannelClick(object sender, RoutedEventArgs e)
        {
            if (grdChannels.SelectedIndex > -1)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to remove this channel?", "Confirm", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    ChannelInfo channelInfo = grdChannels.SelectedItem as ChannelInfo;

                    if (string.Compare("teamxim", channelInfo.Name, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        if (channelInfo.IsFavoriteChannel)
                        {
                            DataFileManager.RemoveFavoriteChannel(channelInfo.Name);
                        }

                        channels.Remove(channelInfo);
                    }
                }
            }
        }

        private void grdChannels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (grdChannels.SelectedIndex > -1)
            {
                btnRemoveChannel.IsEnabled = true;
            }
            else
            {
                btnRemoveChannel.IsEnabled = false;
            }
        }

        # endregion
    }

    public class StreamOpenEventArgs : EventArgs
    {
        public string ChannelName { get; set; }

        public bool OpenInNewTab { get; set; }

        public bool ShowStreamFeed { get; set; }
    }

    public class ChannelInfo : INotifyPropertyChanged
    {
        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }

        private string streamTitle = string.Empty;
        public string StreamTitle
        {
            get { return streamTitle; }
            set
            {
                streamTitle = value;
                RaisePropertyChanged("StreamTitle");
            }
        }

        private string game = string.Empty;
        public string Game
        {
            get { return game; }
            set
            {
                game = value;
                RaisePropertyChanged("Game");
            }
        }

        private string viewers = string.Empty;
        public string Viewers
        {
            get { return viewers; }
            set
            {
                viewers = value;
                RaisePropertyChanged("Viewers");
            }
        }

        private BitmapImage onlineIndicator = null;
        public BitmapImage OnlineIndicator
        {
            get { return onlineIndicator; }
            set
            {
                onlineIndicator = value;
                RaisePropertyChanged("OnlineIndicator");
            }
        }

        public bool IsFavoriteChannel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string caller)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }
    }
}
