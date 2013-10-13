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

namespace LivestreamBuddyNew.Controls
{
    /// <summary>
    /// Interaction logic for Giveaway.xaml
    /// </summary>
    public partial class Giveaway : UserControl
    {
        public Giveaway()
        {
            InitializeComponent();
        }

        public Giveaway(List<string> viewers, ref List<string> excludeList)
            : this()
        {
            this.Viewers = viewers;
            this.ExcludeList = excludeList;

            foreach (string exclude in this.ExcludeList)
            {
                txtExclude.Text += exclude + Environment.NewLine;
            }
        }

        # region Private Members

        private string winner;

        # endregion

        # region Public Members

        public List<string> Viewers { get; set; }

        public List<string> ExcludeList { get; set; }

        # endregion

        # region Private Methods

        private void populateExcludeList()
        {
            ExcludeList.Clear();

            string[] excludeList = txtExclude.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string exclude in excludeList)
            {
                ExcludeList.Add(exclude);
            }
        }

        # endregion

        # region Public Methods

        public void Finalize()
        {
            if (!string.IsNullOrEmpty(winner))
            {
                txtExclude.Text += winner + Environment.NewLine;
            }

            populateExcludeList();
        }

        # endregion

        # region Events

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Viewers.Count > 0)
            {
                populateExcludeList();

                foreach (string exclude in ExcludeList)
                {
                    Viewers.Remove(exclude);
                }

                Random random = new Random();
                int randomNumber = random.Next(0, Viewers.Count);

                winner = Viewers[randomNumber];

                System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

                int i = 0;
                string[] words = new string[] { "AND ", "THE ", "WINNER ", "IS ", ".", ".", "." };

                dispatcherTimer.Tick += delegate(object timerSender, EventArgs timerArgs)
                {
                    if (i < words.Length)
                    {
                        lblStatus.Text += words[i];
                        i++;

                        CommandManager.InvalidateRequerySuggested();
                    }
                    else
                    {
                        lblWinner.Text = winner;
                    }
                };

                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 750);
                dispatcherTimer.Start();
            }
        }

        # endregion
    }
}
