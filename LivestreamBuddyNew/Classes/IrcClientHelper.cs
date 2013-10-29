using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meebey.SmartIrc4net;

namespace LivestreamBuddyNew
{
    public class IrcClientHelper
    {
        # region Private Properties

        private IrcClient client;
        private string username;
        private string channelName;
        private string accessToken;
        private Thread workerThread;
        private volatile bool shouldStop;
        private Options options;

        # endregion

        public IrcClientHelper()
        {
            client = new IrcClient();
            client.Encoding = Encoding.UTF8;

            client.OnConnected += client_OnConnected;
            client.OnDisconnected += client_OnDisconnected;
            client.OnChannelMessage += client_OnChannelMessage;
            client.OnJoin += client_OnJoin;
            client.OnPart += client_OnPart;
            client.OnModeChange += client_OnModeChange;
            client.OnQueryNotice += client_OnQueryNotice;
            client.OnNames += client_OnNames;

            this.workerThread = null;
            this.shouldStop = false;
            this.options = DataFileManager.GetOptions();
        }

        # region Public Methods

        public void LeaveChannel()
        {
            if (this.client.IsConnected)
            {
                client.RfcQuit(Priority.Critical);
            }
        }

        public void SendMessage(string message)
        {
            if (this.client.IsConnected)
            {
                this.client.SendMessage(SendType.Message, "#" + this.channelName, message);
            }
        }

        public void Connect(string channelName, string username, string accessToken)
        {
            if (!this.client.IsConnected)
            {
                this.shouldStop = false;
                this.channelName = channelName;
                this.username = username;
                this.accessToken = accessToken;
                this.client.Connect("irc.twitch.tv", 6667);
            }
            else
            {
                loginJoinAndStartAllTheThings();
            }
        }

        public void Reconnect(string accessToken)
        {
            if (this.client.IsConnected)
            {
                this.accessToken = accessToken;
                this.client.Reconnect(false);
            }
        }

        public void Disconnect()
        {
            if (workerThread != null)
            {
                this.shouldStop = true;
                this.client.RequestStop();
                LeaveChannel();
                this.workerThread.Join();

                if (this.client.IsConnected)
                {
                    this.client.Disconnect();
                }
            }
        }

        # endregion

        # region Private Methods

        private void loginJoinAndStartAllTheThings()
        {
            this.client.Login(this.username, this.username, 0, this.username, "oauth:" + this.accessToken);
            this.client.RfcJoin("#" + this.channelName);

            this.workerThread = new Thread(delegate()
            {
                try
                {
                    while (!shouldStop)
                    {
                        this.client.ListenOnce();
                    }
                }
                catch (Exception ex)
                {
                    Utility.Log(this.GetType(), "loginJoinAndStartAllTheThings()", ex.Message, ex.StackTrace);
                    DoOnError(IRCErrors.ListenThreadError);
                }
            });

            workerThread.Start();
            while (!workerThread.IsAlive) ;
            Thread.Sleep(1);
        }

        private string getNicknameFromRaw(string raw)
        {
            int firstColon = raw.IndexOf(':');
            int firstExclamationMark = raw.IndexOf('!');

            return raw.Substring(firstColon + 1, firstExclamationMark - firstColon - 1);
        }

        private int getParametersLength(IList<string> parameters)
        {
            int length = 0;

            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i] == null)
                {
                    length = i + 1;
                }
            }

            return length;
        }

        # endregion

        # region Event Handlers

        void client_OnJoin(object sender, JoinEventArgs e)
        {
            try
            {
                if (this.username == e.Who)
                {
                    DoOnChannelJoin();
                }
                else
                {
                    DoOnUserJoin(e.Who);
                }
            }
            catch (Exception ex)
            {
                Utility.Log(this.GetType(), "client_OnJoin()", ex.Message, ex.StackTrace);
                DoOnError(IRCErrors.JoinError);
            }
        }

        void client_OnPart(object sender, PartEventArgs e)
        {
            try
            {
                DoOnUserPart(e.Who);
            }
            catch (Exception ex)
            {
                Utility.Log(this.GetType(), "client_OnPart()", ex.Message, ex.StackTrace);
                DoOnError(IRCErrors.PartError);
            }
        }

        void client_OnChannelMessage(object sender, IrcEventArgs e)
        {
            try
            {
                DoOnMessage(e.Data.Nick, e.Data.Message);
            }
            catch (Exception ex)
            {
                Utility.Log(this.GetType(), "client_OnChannelMessage()", ex.Message, ex.StackTrace);
                DoOnError(IRCErrors.ChannelMessageError);
            }
        }

        void client_OnNames(object sender, NamesEventArgs e)
        {
            try
            {
                DoOnUserListCompleted(e.UserList);
            }
            catch (Exception ex)
            {
                Utility.Log(this.GetType(), "client_OnNames()", ex.Message, ex.StackTrace);
                DoOnError(IRCErrors.NamesError);
            }
        }

        void client_OnModeChange(object sender, IrcEventArgs e)
        {
            try
            {
                switch (e.Data.Type)
                {
                    case ReceiveType.ChannelModeChange:
                        bool isModerator = false;

                        if (e.Data.RawMessageArray.Length >= 5)
                        {
                            if (e.Data.RawMessageArray[3] == "+o")
                            {
                                isModerator = true;
                            }

                            DoOnUserMode(e.Data.RawMessageArray[4], isModerator);
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                Utility.Log(this.GetType(), "client_OnModeChange()", ex.Message, ex.StackTrace);
                DoOnError(IRCErrors.ModeError);
            }
        }

        void client_OnQueryNotice(object sender, IrcEventArgs e)
        {
            switch (e.Data.Message.ToLower())
            {
                case "login unsuccessful":
                    DoOnError(IRCErrors.LoginUnsuccessful);
                    break;
            }
        }

        void client_OnConnected(object sender, EventArgs e)
        {
            if (this.options.EnableDebugLogging)
            {
                Utility.Log(this.GetType(), "client_OnConnected()", "Connected to #" + this.channelName, string.Empty);
            }

            loginJoinAndStartAllTheThings();
        }

        void client_OnDisconnected(object sender, EventArgs e)
        {
            if (this.options.EnableDebugLogging)
            {
                Utility.Log(this.GetType(), "client_OnDisconnected()", "Disconnected from #" + this.channelName, string.Empty);
            }

            DoDisconnected();
        }

        # endregion

        # region Custom Events

        public delegate void ChannelJoinHandler(object sender, EventArgs e);
        public event ChannelJoinHandler OnChannelJoin;
        private void DoOnChannelJoin()
        {
            if (OnChannelJoin != null)
            {
                OnChannelJoin(this, null);
            }
        }

        public delegate void DisconnectedHandler(object sender, EventArgs e);
        public event DisconnectedHandler OnDisconnected;
        private void DoDisconnected()
        {
            if (OnDisconnected != null)
            {
                OnDisconnected(this, null);
            }
        }

        public delegate void UserJoinHandler(object sender, IRCUserEventArgs e);
        public event UserJoinHandler OnUserJoin;
        private void DoOnUserJoin(string nickname)
        {
            if (OnUserJoin != null)
            {
                OnUserJoin(this, new IRCUserEventArgs { Nickname = nickname });
            }
        }

        public delegate void UserPartHandler(object sender, IRCUserEventArgs e);
        public event UserPartHandler OnUserPart;
        private void DoOnUserPart(string nickname)
        {
            if (OnUserPart != null)
            {
                OnUserPart(this, new IRCUserEventArgs { Nickname = nickname });
            }
        }

        public delegate void UserModeHandler(object sender, IRCUserModeEventArgs e);
        public event UserModeHandler OnUserMode;
        private void DoOnUserMode(string nickname, bool isModerator = false)
        {
            if (OnUserMode != null)
            {
                OnUserMode(this, new IRCUserModeEventArgs { Nickname = nickname, IsModerator = isModerator });
            }
        }

        public delegate void UserListCompletedHandler(object sender, IRCUserListEventArgs e);
        public event UserListCompletedHandler OnUserListCompleted;
        private void DoOnUserListCompleted(string[] nicknames)
        {
            if (OnUserListCompleted != null)
            {
                OnUserListCompleted(this, new IRCUserListEventArgs { Nicknames = nicknames });
            }
        }

        public delegate void MessageHandler(object sender, IRCMessageEventArgs e);
        public event MessageHandler OnMessage;
        private void DoOnMessage(string nickname, string message)
        {
            if (OnMessage != null)
            {
                OnMessage(this, new IRCMessageEventArgs { Nickname = nickname, Message = message });
            }
        }

        public delegate void ErrorHandler(object sender, IRCErrorEventArgs e);
        public event ErrorHandler OnError;
        private void DoOnError(IRCErrors error)
        {
            if (OnError != null)
            {
                OnError(this, new IRCErrorEventArgs { Error = error });
            }
        }

        # endregion
    }

    public class IRCUserEventArgs : EventArgs
    {
        public string Nickname { get; set; }
    }

    public class IRCUserModeEventArgs : EventArgs
    {
        public string Nickname { get; set; }
        public bool IsModerator { get; set; }
    }

    public class IRCUserListEventArgs : EventArgs
    {
        public string[] Nicknames { get; set; }
    }

    public class IRCMessageEventArgs : EventArgs
    {
        public string Nickname { get; set; }
        public string Message { get; set; }
    }

    public class IRCErrorEventArgs : EventArgs
    {
        public IRCErrors Error { get; set; }
    }
}
