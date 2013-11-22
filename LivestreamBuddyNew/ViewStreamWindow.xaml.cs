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
using System.Windows.Shapes;
using LivestreamBuddyNew.Controls;

namespace LivestreamBuddyNew
{
    /// <summary>
    /// Interaction logic for ViewStreamWindow.xaml
    /// </summary>
    public partial class ViewStreamWindow : Window
    {
        public ViewStreamWindow()
        {
            InitializeComponent();
        }

        public ViewStreamWindow(string channelName, double minimumHeight)
            : this()
        {
            this.Title = "View stream - " + channelName;
            this.ViewStream = new ViewStream(channelName, true, minimumHeight);
            container.Child = this.ViewStream;

            minimumHeight += 40;
            double width = minimumHeight * 1.48;

            this.Width = width;
            this.Width = width;
            this.Height = minimumHeight;
            this.MinHeight = minimumHeight;
        }

        public ViewStreamWindow(ViewStream viewStream, string channelName, double minimumHeight)
            : this()
        {
            this.Title = "View stream - " + channelName;
            this.ViewStream = viewStream;
            container.Child = this.ViewStream;

            minimumHeight += 40;
            double width = minimumHeight * 1.48;

            this.Width = width;
            this.Width = width;
            this.Height = minimumHeight;
            this.MinHeight = minimumHeight;
        }

        # region Public Members

        public ViewStream ViewStream;

        # endregion

        # region Private Members

        # endregion

        # region Event Handlers

        private void Window_Closed_1(object sender, EventArgs e)
        {
            container.Child = null;
        }

        # endregion
    }
}
