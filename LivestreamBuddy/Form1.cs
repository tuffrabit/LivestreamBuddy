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
using Gecko;
using Gecko.DOM;
using System.Text.RegularExpressions;

namespace LivestreamBuddy
{
    public partial class Form1 : Form
    {
        private BackgroundWorker worker;
        private List<string> giveawayExcludeList;
        private object messagesLock;
        private Queue<string> messages;
        private Dictionary<string, string> nicknameColors;
        private string[] potentialNicknameColors;
        private int lastColorUsed;
        private StartupInfo startInfo;
        private User user;
        private int lastChannelSync;
        private volatile bool shouldListenThreadStop;
        private string[] titleAutoCompleteStrings;
        private string[] gameAutoCompleteStrings;
        private string currentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private Regex urlRegex = new Regex(@"(?i)\b((?:[a-z][\w-]+:(?:/{1,3}|[a-z0-9%])|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'.,<>?«»“”‘’]))");

        private const string titleAutoCompleteFileName = "TitleAutoComplete.txt";
        private const string gameAutoCompleteFileName = "GameAutoComplete.txt";

        public Form1()
        {
            InitializeComponent();

            Gecko.Xpcom.Initialize("xulrunner");
            geckoMainOutput.HandleCreated += geckoMainOutput_HandleCreated;
            geckoMainOutput.Navigating += geckoMainOutput_Navigating;
            geckoMainOutput.DocumentCompleted += geckoMainOutput_DocumentCompleted;

            txtStreamTitle.ReadOnly = true;
            txtStreamGame.ReadOnly = true;
            btnStreamUpdate.Enabled = false;
            btnGiveaway.Enabled = false;
            btnRunCommercial.Enabled = false;
            cmbCommercialLength.Enabled = false;
            cmbCommercialLength.SelectedIndex = 0;

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            giveawayExcludeList = new List<string>();
            messagesLock = new object();
            messages = new Queue<string>();
            nicknameColors = new Dictionary<string, string>();
            potentialNicknameColors = new string[] { "#0000FF", "#FF0000", "#00FF00", "#9900CC", "#FF99CC", 
                                                        "#990000", "#3399FF", "#99CCFF", "#FF0033", "#33FF00", 
                                                        "#9933FF", "#FF3399", "#663300", "#669999", "#00FFFF", 
                                                        "#CC0000", "#FF9933", "#33F33", "#9999FF", "#CC0066", 
                                                        "#CC9966", "#FF6666", "#99FFCC", "#0033CC", "#666633"};

            lastColorUsed = -1;
            user = new User();
            lastChannelSync = Environment.TickCount;
            shouldListenThreadStop = false;

            titleAutoCompleteStrings = getAutoCompleteStrings(titleAutoCompleteFileName);
            gameAutoCompleteStrings = getAutoCompleteStrings(gameAutoCompleteFileName);
            updateStreamAutoCompleteFields();
        }

        private void updateStreamAutoCompleteFields()
        {
            txtStreamTitle.AutoCompleteCustomSource = getAutoCompleteSource(titleAutoCompleteFileName);
            txtStreamTitle.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtStreamTitle.AutoCompleteSource = AutoCompleteSource.CustomSource;

            txtStreamGame.AutoCompleteCustomSource = getAutoCompleteSource(gameAutoCompleteFileName);
            txtStreamGame.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtStreamGame.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        private void clearMessages()
        {
            GeckoElement body = geckoMainOutput.Document.GetElementById("body");
            GeckoElement pnlChat = geckoMainOutput.Document.GetElementById("pnlChat");

            body.RemoveChild(pnlChat);
            pnlChat = geckoMainOutput.Document.CreateElement("div");
            pnlChat.SetAttribute("id", "pnlChat");
            pnlChat.SetAttribute("class", "pnlChat");
            body.AppendChild(pnlChat);

            resizeOutputPanel();
        }

        private void messagesWriteLine(string line)
        {
            GeckoElement newLine = geckoMainOutput.Document.CreateElement("p");
            newLine.AppendChild(geckoMainOutput.Document.CreateTextNode(line));

            messagesWriteLine(newLine);
        }

        private void messagesWriteLine(GeckoElement newLine)
        {
            GeckoElement pnlChat = geckoMainOutput.Document.GetElementById("pnlChat");
            
            pnlChat.AppendChild(newLine);
            runOutputScript("pnlChat_ScrollToBottom()");
        }

        private void runOutputScript(string script)
        {
            GeckoButtonElement doStuff = geckoMainOutput.Document.GetElementById("doStuff") as GeckoButtonElement;

            doStuff.SetAttribute("onclick", "javascript:" + script + ";");
            doStuff.Click();
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

        private string getNextColor()
        {
            lastColorUsed++;

            if (lastColorUsed > (potentialNicknameColors.Length - 1))
            {
                lastColorUsed = 0;
            }

            return potentialNicknameColors[lastColorUsed];
        }

        private GeckoElement getNewChatLine(string nick, string message, string color)
        {
            GeckoElement newChatLine = null;

            MatchCollection urls = urlRegex.Matches(message);

            this.Invoke(new Action(() =>
            {
                newChatLine = geckoMainOutput.Document.CreateElement("div");
                GeckoElement nameField = geckoMainOutput.Document.CreateElement("span");
                GeckoElement messageField = geckoMainOutput.Document.CreateElement("span");

                nameField.SetAttribute("style", "color:" + color + ";font-weight:bold;");
                nameField.AppendChild(geckoMainOutput.Document.CreateTextNode(nick + ": "));

                if (urls.Count > 0)
                {
                    int currentUrlIndex = 0;
                    Match currentUrl = urls[currentUrlIndex];
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < message.Length; i++)
                    {
                        char currentChar = message[i];

                        if (currentUrl != null && i == currentUrl.Index)
                        {
                            if (sb.Length > 0)
                            {
                                messageField.AppendChild(geckoMainOutput.Document.CreateTextNode(sb.ToString()));
                                sb.Clear();
                            }

                            GeckoElement urlNode = geckoMainOutput.Document.CreateElement("a");
                            string url = currentUrl.Value;

                            if (!url.StartsWith("http://"))
                            {
                                url = "http://" + url;
                            }

                            urlNode.SetAttribute("href", url);
                            urlNode.AppendChild(geckoMainOutput.Document.CreateTextNode(currentUrl.Value));
                            messageField.AppendChild(urlNode);

                            i = currentUrl.Index + currentUrl.Length - 1;

                            if (++currentUrlIndex >= urls.Count)
                            {
                                currentUrl = null;
                            }
                            else
                            {
                                currentUrl = urls[currentUrlIndex];
                            }
                        }
                        else
                        {
                            sb.Append(currentChar);
                        }
                    }

                    if (sb.Length > 0)
                    {
                        messageField.AppendChild(geckoMainOutput.Document.CreateTextNode(sb.ToString()));
                    }
                }
                else
                {
                    messageField.AppendChild(geckoMainOutput.Document.CreateTextNode(message));
                }

                //messageField.AppendChild(geckoMainOutput.Document.CreateTextNode(message));

                newChatLine.AppendChild(nameField);
                newChatLine.AppendChild(messageField);
            }));
            

            return newChatLine;
        }

        private void resizeOutputPanel()
        {
            runOutputScript(string.Format("pnlChat_Resize({0}, {1})", (geckoMainOutput.Size.Width - 10).ToString(), (geckoMainOutput.Size.Height - 10).ToString()));
        }

        private void loadChat()
        {
            GeckoElement pnlChat = geckoMainOutput.Document.GetElementById("pnlChat");
            GeckoElement objectNode = geckoMainOutput.Document.CreateElement("object");
            GeckoElement allowFullScreenNode = geckoMainOutput.Document.CreateElement("param");
            GeckoElement allowScriptAccessNode = geckoMainOutput.Document.CreateElement("param");
            GeckoElement allowNetworkingNode = geckoMainOutput.Document.CreateElement("param");
            GeckoElement movieNode = geckoMainOutput.Document.CreateElement("param");
            GeckoElement flashvarsNode = geckoMainOutput.Document.CreateElement("param");

            objectNode.SetAttribute("type", "application/x-shockwave-flash");
            objectNode.SetAttribute("height", "100%");
            objectNode.SetAttribute("width", "100%");
            objectNode.SetAttribute("id", "live_embed_player_flash");
            objectNode.SetAttribute("data", "http://www.twitch.tv/widgets/live_embed_player.swf?channel=" + startInfo.Channel + "");
            objectNode.SetAttribute("bgcolor", "#000000");

            allowFullScreenNode.SetAttribute("name", "allowFullScreen");
            allowFullScreenNode.SetAttribute("value", "false");

            allowScriptAccessNode.SetAttribute("name", "allowScriptAccess");
            allowScriptAccessNode.SetAttribute("value", "false");

            allowNetworkingNode.SetAttribute("name", "allowNetworking");
            allowNetworkingNode.SetAttribute("value", "all");

            movieNode.SetAttribute("name", "movie");
            movieNode.SetAttribute("value", "http://www.twitch.tv/widgets/live_embed_player.swf");

            flashvarsNode.SetAttribute("name", "flashvars");
            flashvarsNode.SetAttribute("value", "hostname=www.twitch.tv&channel=" + startInfo.Channel + "&auto_play=true&start_volume=25");

            objectNode.AppendChild(allowFullScreenNode);
            objectNode.AppendChild(allowScriptAccessNode);
            objectNode.AppendChild(allowNetworkingNode);
            objectNode.AppendChild(movieNode);
            objectNode.AppendChild(flashvarsNode);
            pnlChat.AppendChild(objectNode);
        }

        # region AutoSuggestSource

        private string[] getAutoCompleteStrings(string fileName)
        {
            List<string> strings = new List<string>();

            if (!File.Exists(fileName))
            {
                using (FileStream fs = File.Create(fileName))
                {
                    fs.Close();
                }
            }

            using (StreamReader reader = File.OpenText(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    strings.Add(line);
                }

                reader.Close();
            }

            return strings.ToArray();
        }

        private AutoCompleteStringCollection getAutoCompleteSource(string fileName)
        {
            var source = new AutoCompleteStringCollection();

            if (fileName == titleAutoCompleteFileName)
            {
                source.AddRange(titleAutoCompleteStrings);
            }
            else if (fileName == gameAutoCompleteFileName)
            {
                source.AddRange(gameAutoCompleteStrings);
            }

            return source;
        }

        private void addStringToAutoComplete(string fileName, string value)
        {
            string[] strings = null;

            if (fileName == titleAutoCompleteFileName)
            {
                strings = titleAutoCompleteStrings;
            }
            else if (fileName == gameAutoCompleteFileName)
            {
                strings = gameAutoCompleteStrings;
            }

            bool alreadyExists = false;

            foreach (string fileValue in strings)
            {
                if (string.CompareOrdinal(fileValue, value) == 0)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (alreadyExists == false)
            {
                try
                {
                    using (StreamWriter writer = File.AppendText(fileName))
                    {
                        writer.WriteLine(value);
                        writer.Close();
                    }
                }
                catch { }
            }
        }

        # endregion

        # region Events

        void geckoMainOutput_HandleCreated(object sender, EventArgs e)
        {
            geckoMainOutput.Navigate("file://" + currentPath + "\\chat.html");
        }

        void geckoMainOutput_DocumentCompleted(object sender, EventArgs e)
        {
            resizeOutputPanel();
        }

        void geckoMainOutput_Navigating(object sender, Gecko.Events.GeckoNavigatingEventArgs e)
        {
            if (!e.Uri.AbsolutePath.Contains("chat.html"))
            {
                e.Cancel = true;

                try
                {
                    Process.Start(e.Uri.OriginalString);
                }
                catch { }
            }
        }

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
                    clearMessages();
                    btnConnect.Enabled = false;
                    shouldListenThreadStop = false;
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
            client.Encoding = Encoding.UTF8;
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
                    clearMessages();
                    lstViewers.Items.Add("Getting viewer list...");
                    btnConnect.Enabled = true;
                    btnConnect.Text = "Disconnect";
                    btnGiveaway.Enabled = true;
                    txtUsername.Enabled = false;
                    txtPassword.Enabled = false;
                    txtChannel.Enabled = false;

                    break;
                case ProgressReportType.Error:
                    messagesWriteLine((string)info.Data);

                    break;
                case ProgressReportType.Message:
                    messagesWriteLine((GeckoElement)info.Data);

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

            clearMessages();
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

                                    GeckoElement newChatLine = getNewChatLine(startInfo.Username, message, "#000000");

                                    worker.ReportProgress(0, new ProgressReport()
                                    {
                                        Type = ProgressReportType.Message,
                                        Data = newChatLine
                                    });
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
            string nickColor = "#000000";

            if (nicknameColors.ContainsKey(e.Data.Nick))
            {
                nickColor = nicknameColors[e.Data.Nick];
            }
            else
            {
                nickColor = getNextColor();
                nicknameColors.Add(e.Data.Nick, nickColor);
            }

            GeckoElement newChatLine = getNewChatLine(e.Data.Nick, e.Data.Message, nickColor);

            worker.ReportProgress(0, new ProgressReport()
            {
                Type = ProgressReportType.Message,
                Data = newChatLine
            });
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
            
        }

        private void txtChannel_TextChanged(object sender, EventArgs e)
        {
            if (txtChannel.Text.Length > 0)
            {
                txtStreamTitle.ReadOnly = false;
                txtStreamGame.ReadOnly = false;
                btnStreamUpdate.Enabled = true;
                btnRunCommercial.Enabled = true;
                cmbCommercialLength.Enabled = true;
            }
        }

        private void btnStreamUpdate_Click(object sender, EventArgs e)
        {
            LivestreamBuddy.ChannelManager channelManager = new ChannelManager();
            user.Scope = UserScope.ChannelEditor;

            if (string.IsNullOrEmpty(user.AccessToken))
            {
                AuthForm bob = new AuthForm(user);
                bob.ShowDialog(this);
            }

            try
            {
                channelManager.UpdateChannel(user, txtChannel.Text, txtStreamTitle.Text, txtStreamGame.Text);
                MessageBox.Show("Update successful.");

                addStringToAutoComplete(titleAutoCompleteFileName, txtStreamTitle.Text);
                addStringToAutoComplete(gameAutoCompleteFileName, txtStreamGame.Text);

                titleAutoCompleteStrings = getAutoCompleteStrings(titleAutoCompleteFileName);
                gameAutoCompleteStrings = getAutoCompleteStrings(gameAutoCompleteFileName);

                updateStreamAutoCompleteFields();
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

        private void btnRunCommercial_Click(object sender, EventArgs e)
        {
            LivestreamBuddy.ChannelManager channelManager = new ChannelManager();
            user.Scope = UserScope.ChannelEditor;

            if (string.IsNullOrEmpty(user.AccessToken))
            {
                AuthForm bob = new AuthForm(user);
                bob.ShowDialog(this);
            }

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

                channelManager.RunCommercial(user, txtChannel.Text, length);
                MessageBox.Show("Running commercial.");
            }
            catch
            {
                MessageBox.Show("Commercial run failed.");
            }
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            resizeOutputPanel();
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
        UpdateChannelInfo, 
        Error, 
        Query
    }
}
