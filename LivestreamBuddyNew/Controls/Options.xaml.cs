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

namespace LivestreamBuddyNew.Controls
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : UserControl
    {
        public Options()
        {
            InitializeComponent();

            this.UserOptions = DataFileManager.GetOptions();

            chkStreamOpenStyle.IsChecked = this.UserOptions.OpenStreamsInNewTab;
            chkStreamViewShow.IsChecked = this.UserOptions.ShowStreamFeedWhenOpening;
            chkStreamShowChatTimestamps.IsChecked = this.UserOptions.ShowTimestampsInChat;
            chkEnableDebugLogging.IsChecked = this.UserOptions.EnableDebugLogging;
        }

        # region Public Members

        public LivestreamBuddyNew.Options UserOptions { get; set; }

        # endregion

        # region Event Handlers

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            this.UserOptions.OpenStreamsInNewTab = chkStreamOpenStyle.IsChecked.Value;
            this.UserOptions.ShowStreamFeedWhenOpening = chkStreamViewShow.IsChecked.Value;
            this.UserOptions.ShowTimestampsInChat = chkStreamShowChatTimestamps.IsChecked.Value;
            this.UserOptions.EnableDebugLogging = chkEnableDebugLogging.IsChecked.Value;

            DataFileManager.SetOptions(this.UserOptions);
            DoOnSaved();
        }

        # endregion

        # region Custom Events

        public delegate void OptionsSavedHandler(object sender, EventArgs e);
        public event OptionsSavedHandler OnSaved;
        private void DoOnSaved()
        {
            if (OnSaved != null)
            {
                OnSaved(this, null);
            }
        }

        # endregion
    }
}
