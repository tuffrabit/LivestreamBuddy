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
using LivestreamBuddy;

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
            Application.Current.MainWindow.Closing += MainWindow_Closing;

            bool getFeaturedChannels = true;

            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (string.Compare(arg, "-nofeaturedchannels", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    getFeaturedChannels = false;
                }
            }

            channels = new ObservableCollection<ChannelInfo>();

            addStreamToFavoritesList("teamxim");

            foreach (string channel in DataFileManager.GetFavoriteChannels())
            {
                addStreamToFavoritesList(channel);
            }

            streamManager = new StreamManager();

            if (getFeaturedChannels)
            {
                foreach (LivestreamBuddy.Stream stream in streamManager.GetFeaturedStreams())
                {
                    addStreamToFavoritesList(stream.Channel.Name);
                }
            }

            grdChannels.ItemsSource = channels;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();
        }

        # region Public Properties

        public delegate void StreamOpenHandler(object sender, StreamOpenEventArgs e);
        public event StreamOpenHandler OnStreamOpen;

        # endregion

        # region Private Properties

        private ObservableCollection<ChannelInfo> channels;
        private StreamManager streamManager;
        private BackgroundWorker worker;

        # endregion

        # region Private Methods

        private void openStream(string channelName)
        {
            if (OnStreamOpen != null)
            {
                StreamOpenEventArgs args = new StreamOpenEventArgs { ChannelName = channelName.Trim(), OpenInNewTab = (bool)chkStreamOpenStyle.IsChecked };
                OnStreamOpen(this, args);
            }
        }

        private void addStreamToFavoritesList(string streamName)
        {
            channels.Add(new ChannelInfo { Name = streamName });
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
            List<LivestreamBuddy.Stream> streams = null;

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

                addStreamToFavoritesList(window.Value.ToLower());
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
            List<LivestreamBuddy.Stream> streams = e.UserState as List<LivestreamBuddy.Stream>;
            BitmapImage offlineImage = new BitmapImage(new Uri("pack://application:,,,/LivestreamBuddy;component/Resources/offline.png"));
            BitmapImage onlineImage = new BitmapImage(new Uri("pack://application:,,,/LivestreamBuddy;component/Resources/online.png"));

            foreach (ChannelInfo channel in channels)
            {
                bool found = false;

                foreach (LivestreamBuddy.Stream stream in streams)
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
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
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
                openStream(txtGoToChannel.Text);
            }
        }

        private void grdChannels_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ChannelInfo channel = null;

            try
            {
                channel = channels[grdChannels.SelectedIndex];
            }
            catch { }

            if (channel != null)
            {
                openStream(channel.Name);
            }
        }

        # endregion
    }

    public class StreamOpenEventArgs : EventArgs
    {
        public string ChannelName { get; set; }

        public bool OpenInNewTab { get; set; }
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
