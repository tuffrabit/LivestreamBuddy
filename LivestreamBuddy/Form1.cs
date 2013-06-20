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
using LivestreamBuddy;
using System.Diagnostics;
using System.IO;

namespace LivestreamBuddy
{
    public partial class Form1 : Form
    {
        private BackgroundWorker worker;
        private List<string> giveawayExcludeList;
        private object messagesLock;
        private Queue<string> messages;
        private Dictionary<string, Color> nicknameColors;
        private Color[] potentialNicknameColors;
        private int lastColorUsed;
        private StartupInfo startInfo;
        private User user;
        private int lastChannelSync;
        private volatile bool shouldListenThreadStop;

        public Form1()
        {
            InitializeComponent();

            txtStreamTitle.ReadOnly = true;
            txtStreamGame.ReadOnly = true;
            btnStreamUpdate.Enabled = false;
            btnGiveaway.Enabled = false;

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            giveawayExcludeList = new List<string>();
            messagesLock = new object();
            messages = new Queue<string>();
            nicknameColors = new Dictionary<string, Color>();
            potentialNicknameColors = new Color[] {Color.Blue, Color.Red, Color.Green, Color.Purple, 
                Color.Pink, Color.Brown, Color.CornflowerBlue, Color.Aquamarine, Color.Maroon, Color.LightGreen, Color.Indigo, Color.HotPink, Color.Chocolate, 
                Color.DarkSlateGray, Color.Turquoise, Color.IndianRed, Color.Orange, Color.LimeGreen, 
                Color.Lavender, Color.DeepPink, Color.BurlyWood, Color.Crimson, Color.MintCream};
            lastColorUsed = -1;
            user = new User();
            lastChannelSync = Environment.TickCount;
            shouldListenThreadStop = false;
        }

        private void messagesWriteLine(string line)
        {
            messagesWriteLine(line, true, Color.Black);
        }

        private void messagesWriteLine(string line, bool isChatMessage, System.Drawing.Color color)
        {
            int length = txtMessages.TextLength;

            txtMessages.AppendText(line + Environment.NewLine);
            txtMessages.SelectionStart = txtMessages.Text.Length;
            txtMessages.ScrollToCaret();

            int separatorIndex = line.IndexOf(':');
            if (isChatMessage && separatorIndex > -1)
            {
                txtMessages.SelectionStart = length;
                txtMessages.SelectionLength = separatorIndex + 1;
                txtMessages.SelectionColor = color;
                txtMessages.SelectionFont = new Font(txtMessages.SelectionFont.FontFamily, txtMessages.SelectionFont.Size, FontStyle.Bold);
                txtMessages.SelectionLength = 0;
            }
        }

        private int reportWorkerProgressForViewerList(IrcClient client, string channelName)
        {
            Meebey.SmartIrc4net.Channel channel = client.GetChannel(channelName);

            worker.ReportProgress(0, new ProgressReport(ProgressReportType.GetViewers, channel.Users));

            return channel.Users.Count;
        }

        private void updateChannelInfo()
        {
            LivestreamBuddy.StreamManager streamManager = new StreamManager();
            LivestreamBuddy.Stream stream = streamManager.GetStream(txtChannel.Text);

            if (stream.IsOnline)
            {
                txtStreamTitle.Text = stream.Channel.Title;
                txtStreamGame.Text = stream.Game;
                lblViewerCount.Text = "Viewer Count: " + stream.ViewerCount;
            }
        }

        private Color getNextColor()
        {
            lastColorUsed++;

            if (lastColorUsed > (potentialNicknameColors.Length - 1))
            {
                lastColorUsed = 0;
            }

            return potentialNicknameColors[lastColorUsed];
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
                    shouldListenThreadStop = false;
                    messagesWriteLine("Connecting...", false, Color.Black);
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
                lock (messagesLock)
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

            startInfo = startupInfo;
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
                    messagesWriteLine("Connected!", false, Color.Black);
                    messagesWriteLine("Logging in...", false, Color.Black);

                    break;
                case ProgressReportType.Login:
                    messagesWriteLine("Logged in!", false, Color.Black);
                    messagesWriteLine("Joining channel " + txtChannel.Text + "...", false, Color.Black);

                    break;
                case ProgressReportType.ChannelJoin:
                    messagesWriteLine("Channel joined!", false, Color.Black);
                    Thread.Sleep(75);
                    txtMessages.Clear();
                    lstViewers.Items.Add("Getting viewer list...");
                    btnConnect.Enabled = true;
                    btnConnect.Text = "Disconnect";
                    btnGiveaway.Enabled = true;
                    txtUsername.Enabled = false;
                    txtPassword.Enabled = false;
                    txtChannel.Enabled = false;

                    break;
                case ProgressReportType.Error:
                    messagesWriteLine((string)info.Data, false, Color.Black);

                    break;
                case ProgressReportType.Message:
                    messagesWriteLine((string)info.Data, true, info.MessageColor);

                    break;
                case ProgressReportType.GetViewers:
                    Hashtable users = info.Data as Hashtable;

                    if (users.Count > 1)
                    {
                        lstViewers.BeginUpdate();
                        lstViewers.Items.Clear();

                        foreach (DictionaryEntry user in users)
                        {
                            ChannelUser viewer = (ChannelUser)user.Value;

                            lstViewers.Items.Add(viewer.Nick);
                        }

                        lstViewers.EndUpdate();
                    }

                    break;
                case ProgressReportType.UpdateChannelInfo:
                    updateChannelInfo();

                    break;
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtUsername.Enabled = true;
            txtPassword.Enabled = true;
            txtChannel.Enabled = true;
            btnGiveaway.Enabled = false;
            btnConnect.Enabled = true;
            btnConnect.Text = "Connect";

            txtMessages.Clear();
            lblViewerCount.Text = "Viewer Count:";
            lstViewers.Items.Clear();
            nicknameColors.Clear();
            lastColorUsed = -1;
        }

        void client_OnConnected(object sender, EventArgs e)
        {
            IrcClient client = sender as IrcClient;

            if (client.IsConnected)
            {
                worker.ReportProgress(0, new ProgressReport(ProgressReportType.Connect, null));
                
                client.Login(startInfo.Username, startInfo.Username, 0, startInfo.Username.ToLower(), startInfo.Password);
                worker.ReportProgress(0, new ProgressReport(ProgressReportType.Login, null));
                
                client.RfcJoin(startInfo.Channel, Priority.Critical);

                Thread listenThread = new Thread(delegate()
                    {
                        while (true)
                        {
                            if (shouldListenThreadStop)
                            {
                                break;
                            }

                            client.ListenOnce();
                        }
                    });

                listenThread.Start();

                bool isJoined = false;
                int numberOfViewers = 0;

                while (true)
                {
                    if ((worker.CancellationPending == true))
                    {
                        break;
                    }

                    Thread.Sleep(10);

                    if (isJoined)
                    {
                        if (numberOfViewers == 1)
                        {
                            numberOfViewers = reportWorkerProgressForViewerList(client, startInfo.Channel);
                        }

                        int ticks = Environment.TickCount;
                        if (ticks - lastChannelSync > 60000)
                        {
                            worker.ReportProgress(0, new ProgressReport(ProgressReportType.UpdateChannelInfo, null));
                            reportWorkerProgressForViewerList(client, startInfo.Channel);
                            lastChannelSync = ticks;
                        }

                        lock (messagesLock)
                        {
                            if (messages.Count > 0)
                            {
                                foreach (string message in messages)
                                {
                                    client.SendMessage(SendType.Message, startInfo.Channel, message);
                                    worker.ReportProgress(0, new ProgressReport(ProgressReportType.Message, startInfo.Username + ": " + message));
                                }

                                messages.Clear();
                            }
                        }
                    }
                    else if (client.IsJoined(startInfo.Channel))
                    {
                        isJoined = true;
                        worker.ReportProgress(0, new ProgressReport(ProgressReportType.ChannelJoin, null));
                        worker.ReportProgress(0, new ProgressReport(ProgressReportType.UpdateChannelInfo, null));
                        client.RfcNames(startInfo.Channel, Priority.Critical);
                        numberOfViewers = reportWorkerProgressForViewerList(client, startInfo.Channel);

                        client.OnChannelMessage += client_OnChannelMessage;
                        client.OnChannelActiveSynced += client_OnChannelActiveSynced;
                        client.OnJoin += client_OnJoin;
                        client.OnAway += client_OnAway;
                        client.OnQuit += client_OnQuit;
                    }                    
                }

                shouldListenThreadStop = true;
                client.RfcQuit(Priority.Critical);
                listenThread.Join();
                client.Disconnect();
            }
        }

        void client_OnChannelMessage(object sender, IrcEventArgs e)
        {
            Color nickColor = Color.Black;

            if (nicknameColors.ContainsKey(e.Data.Nick))
            {
                nickColor = nicknameColors[e.Data.Nick];
            }
            else
            {
                nickColor = getNextColor();
                nicknameColors.Add(e.Data.Nick, nickColor);
            }

            string message = e.Data.Nick + ": " + e.Data.Message;
            worker.ReportProgress(0, new ProgressReport()
            {
                Type = ProgressReportType.Message, 
                Data = message, 
                MessageColor = nickColor
            });

            if (e.Data.Message.ToLower() == "bobtart")
            {
                reportWorkerProgressForViewerList(sender as IrcClient, e.Data.Channel);
            }
        }

        void client_OnError(object sender, Meebey.SmartIrc4net.ErrorEventArgs e)
        {
            worker.ReportProgress(0, new ProgressReport(ProgressReportType.Error, Environment.NewLine + e.ErrorMessage + Environment.NewLine));
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

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            if (string.Compare(txtUsername.Text, txtChannel.Text, StringComparison.OrdinalIgnoreCase) == 0)
            {
                txtStreamTitle.ReadOnly = false;
                txtStreamGame.ReadOnly = false;
                btnStreamUpdate.Enabled = true;
            }
            else
            {
                txtStreamTitle.ReadOnly = true;
                txtStreamGame.ReadOnly = true;
                btnStreamUpdate.Enabled = false;
            }
        }

        private void txtChannel_TextChanged(object sender, EventArgs e)
        {
            if (string.Compare(txtUsername.Text, txtChannel.Text, StringComparison.OrdinalIgnoreCase) == 0)
            {
                txtStreamTitle.ReadOnly = false;
                txtStreamGame.ReadOnly = false;
                btnStreamUpdate.Enabled = true;
            }
            else
            {
                txtStreamTitle.ReadOnly = true;
                txtStreamGame.ReadOnly = true;
                btnStreamUpdate.Enabled = false;
            }
        }

        private void btnStreamUpdate_Click(object sender, EventArgs e)
        {
            LivestreamBuddy.ChannelManager channelManager = new ChannelManager();
            LivestreamBuddy.Channel channel = new Channel()
            {
                Title = txtStreamTitle.Text,
                CurrentStream = new Stream()
                {
                    Game = txtStreamGame.Text
                }
            };

            user.UserId = txtUsername.Text;
            user.Password = txtPassword.Text;
            user.Scope = UserScope.ChannelEditor;

            if (string.IsNullOrEmpty(user.AccessToken))
            {
                AuthForm bob = new AuthForm(user);
                bob.ShowDialog(this);
            }

            try
            {
                channelManager.UpdateChannel(user, channel);
                MessageBox.Show("Update successful.");
            }
            catch
            {
                MessageBox.Show("Update failed.");
            }
        }

        private void lblViewerCount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (worker.IsBusy)
            {
                updateChannelInfo();
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("help.html");
            Process.Start(sInfo);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            worker.CancelAsync();
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

        public Color MessageColor { get; set; }

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
        UpdateChannelInfo, 
        Error, 
        Query
    }
}
