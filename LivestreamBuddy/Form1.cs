/*Copyright (C) 2013 Robert A. Boucher Jr. (TuFFrabit)

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Meebey.SmartIrc4net;

namespace LivestreamBuddy
{
    public partial class Form1 : Form
    {
        private BackgroundWorker worker;
        private List<string> giveawayExcludeList;
        private Queue<string> messages;
        private StartupInfo info;

        public Form1()
        {
            InitializeComponent();

            btnGiveaway.Enabled = false;

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            giveawayExcludeList = new List<string>();

            messages = new Queue<string>();
        }

        private void messagesWriteLine(string line)
        {
            txtMessages.Text += line + Environment.NewLine;
            txtMessages.SelectionStart = txtMessages.Text.Length;
            txtMessages.ScrollToCaret();
        }

        private int reportWorkerProgressForViewerList(IrcClient client, string channelName)
        {
            Channel channel = client.GetChannel(channelName);

            worker.ReportProgress(0, new ProgressReport(ProgressReportType.GetViewers, channel.Users));

            return channel.Users.Count;
        }

        # region Events

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text.ToLower() == "connect")
            {
                if (string.IsNullOrEmpty(txtUsername.Text))
                {
                    MessageBox.Show("You must provide a username.");
                }
                else if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("You must provide a password.");
                }
                else if (string.IsNullOrEmpty(txtChannel.Text))
                {
                    MessageBox.Show("You must provide a channel.");
                }
                else
                {
                    giveawayExcludeList.Clear();
                    lstViewers.Items.Clear();
                    txtMessages.Clear();
                    btnConnect.Enabled = false;
                    messagesWriteLine("Connecting...");
                    worker.RunWorkerAsync(new StartupInfo(txtUsername.Text, txtPassword.Text, txtChannel.Text));
                }
            }
            else if (btnConnect.Text.ToLower() == "disconnect")
            {
                worker.CancelAsync();
            }
        }

        private void btnGiveaway_Click(object sender, EventArgs e)
        {
            List<string> viewers = new List<string>();

            foreach (object item in lstViewers.Items)
            {
                viewers.Add((string)item);
            }

            frmGiveaway giveawayForm = new frmGiveaway(viewers, ref giveawayExcludeList);
            giveawayForm.ShowDialog(this);
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                lock (messages)
                {
                    messages.Enqueue(txtMessage.Text);
                }

                txtMessage.Clear();
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker theWorker = sender as BackgroundWorker;
            StartupInfo startupInfo = e.Argument as StartupInfo;
            IrcClient client = new IrcClient();

            info = startupInfo;
            client.OnConnected += client_OnConnected;
            client.OnError += client_OnError;
            client.ActiveChannelSyncing = true;

            try
            {
                client.Connect("irc.twitch.tv", 6667);
            }
            catch (ConnectionException ex)
            {
                theWorker.ReportProgress(0, new ProgressReport(ProgressReportType.Error, ex.Message));
            }
        }        

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressReport info = e.UserState as ProgressReport;

            switch (info.Type)
            {
                case ProgressReportType.Connect:
                    messagesWriteLine("Connected!");
                    messagesWriteLine("Logging in...");

                    break;
                case ProgressReportType.Login:
                    messagesWriteLine("Logged in!");
                    messagesWriteLine("Joining channel " + txtChannel.Text + "...");

                    break;
                case ProgressReportType.ChannelJoin:
                    messagesWriteLine("Channel joined!");
                    Thread.Sleep(75);
                    txtMessages.Clear();
                    lstViewers.Items.Add("Getting viewer list...");
                    btnConnect.Enabled = true;
                    btnConnect.Text = "Disconnect";
                    btnGiveaway.Enabled = true;

                    break;
                case ProgressReportType.Error:
                    messagesWriteLine((string)info.Data);

                    break;
                case ProgressReportType.Message:
                    messagesWriteLine((string)info.Data);

                    break;
                case ProgressReportType.GetViewers:
                    Hashtable users = info.Data as Hashtable;

                    if (users.Count > 1)
                    {
                        lstViewers.Items.Clear();

                        foreach (DictionaryEntry user in users)
                        {
                            ChannelUser viewer = (ChannelUser)user.Value;

                            lstViewers.Items.Add(viewer.Nick);
                        }

                        lblViewerCount.Text = "Viewer Count: " + lstViewers.Items.Count;
                    }

                    break;
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnGiveaway.Enabled = false;
            btnConnect.Enabled = true;
            btnConnect.Text = "Connect";

            txtMessages.Clear();
            lblViewerCount.Text = "Viewer Count:";
            lstViewers.Items.Clear();
        }

        void client_OnConnected(object sender, EventArgs e)
        {
            IrcClient client = sender as IrcClient;

            if (client.IsConnected)
            {
                worker.ReportProgress(0, new ProgressReport(ProgressReportType.Connect, null));
                
                client.Login(info.Username, info.Username, 0, info.Username.ToLower(), info.Password);
                worker.ReportProgress(0, new ProgressReport(ProgressReportType.Login, null));
                
                client.RfcJoin(info.Channel, Priority.Critical);                

                var tokenSource = new CancellationTokenSource();
                CancellationToken ct = tokenSource.Token;

                var listonTask = Task.Factory.StartNew(() =>
                    {
                        ct.ThrowIfCancellationRequested();

                        while (true)
                        {
                            if (!ct.IsCancellationRequested)
                            {
                                client.ListenOnce();
                            }
                        }
                    }
                , tokenSource.Token);

                bool isJoined = false;
                int numberOfViewers = 0;

                while (true)
                {
                    if ((worker.CancellationPending == true))
                    {
                        break;
                    }

                    Thread.Sleep(100);

                    if (isJoined)
                    {
                        if (numberOfViewers == 1)
                        {
                            numberOfViewers = reportWorkerProgressForViewerList(client, info.Channel);
                        }

                        lock (messages)
                        {
                            if (messages.Count > 0)
                            {
                                foreach (string message in messages)
                                {
                                    client.SendMessage(SendType.Message, info.Channel, message);
                                    worker.ReportProgress(0, new ProgressReport(ProgressReportType.Message, info.Username + ": " + message));
                                }

                                messages.Clear();
                            }
                        }
                    }
                    else if (client.IsJoined(info.Channel))
                    {
                        isJoined = true;
                        worker.ReportProgress(0, new ProgressReport(ProgressReportType.ChannelJoin, null));
                        client.RfcNames(info.Channel, Priority.Critical);
                        numberOfViewers = reportWorkerProgressForViewerList(client, info.Channel);

                        client.OnChannelMessage += client_OnChannelMessage;
                        client.OnChannelActiveSynced += client_OnChannelActiveSynced;
                        client.OnJoin += client_OnJoin;
                        client.OnAway += client_OnAway;
                        client.OnQuit += client_OnQuit;
                    }                    
                }

                tokenSource.Cancel();
                client.RfcQuit(Priority.Critical);
                client.Disconnect();
            }
        }

        void client_OnChannelMessage(object sender, IrcEventArgs e)
        {
            string message = e.Data.Nick + ": " + e.Data.Message;
            worker.ReportProgress(0, new ProgressReport(ProgressReportType.Message, message));

            if (e.Data.Message.ToLower() == "bobtart")
            {
                reportWorkerProgressForViewerList(sender as IrcClient, e.Data.Channel);

                /*IrcClient client = sender as IrcClient;
                Channel channel = client.GetChannel(e.Data.Channel);
                IDictionaryEnumerator userEnum = channel.Users.GetEnumerator();

                worker.ReportProgress(0, new ProgressReport(ProgressReportType.GetViewers, userEnum));*/
            }
        }

        void client_OnError(object sender, ErrorEventArgs e)
        {
            worker.ReportProgress(0, new ProgressReport(ProgressReportType.Error, e.ErrorMessage));
        }

        void client_OnChannelActiveSynced(object sender, IrcEventArgs e)
        {
            reportWorkerProgressForViewerList(sender as IrcClient, e.Data.Channel);
        }

        void client_OnJoin(object sender, JoinEventArgs e)
        {
            reportWorkerProgressForViewerList(sender as IrcClient, e.Channel);
        }

        void client_OnAway(object sender, AwayEventArgs e)
        {
            reportWorkerProgressForViewerList(sender as IrcClient, e.Data.Channel);
        }

        void client_OnQuit(object sender, QuitEventArgs e)
        {
            reportWorkerProgressForViewerList(sender as IrcClient, e.Data.Channel);
        }

        # endregion
    }

    public class StartupInfo
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Channel { get; set; }

        public StartupInfo() { }

        public StartupInfo(string username, string password, string channel)
        {
            this.Username = username;
            this.Password = password;
            this.Channel = "#" + channel;
        }
    }

    public class ProgressReport
    {
        public ProgressReportType Type { get; set; }

        public object Data { get; set; }

        public ProgressReport() { }

        public ProgressReport(ProgressReportType type, object data)
        {
            this.Type = type;
            this.Data = data;
        }
    }

    public enum ProgressReportType
    {
        Unknown, 
        Connect, 
        Login, 
        ChannelJoin, 
        GetViewers, 
        Message, 
        Error, 
        Query
    }
}
