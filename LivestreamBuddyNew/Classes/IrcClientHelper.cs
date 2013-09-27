using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrcDotNet;

namespace LivestreamBuddyNew
{
    public class IrcClientHelper
    {
        # region Private Properties

        private IrcClient client;
        private string username;
        private string channelName;

        private bool isWaitingOnWho;
        private List<string> whoResults;

        private bool isWaitingOnUserList;
        private List<string> userList;

        # endregion

        public IrcClientHelper(string channelName, string username)
        {
            this.channelName = channelName;
            this.username = username;
            this.isWaitingOnWho = false;
            this.whoResults = new List<string>();
            this.isWaitingOnUserList = true;
            this.userList = new List<string>();

            client = new IrcClient();
            client.TextEncoding = Encoding.UTF8;
            client.Connected += client_Connected;
            client.RawMessageReceived += client_RawMessageReceived;

            client.Connect("irc.twitch.tv", 6667, false, new IrcUserRegistrationInfo { UserName = username, NickName = "tuffrabit", Password = "oauth:1qc1hf0ahth535j3cb43szdayecaiai" });
        }

        # region Public Methods

        public void LeaveChannel()
        {
            if (this.client.IsConnected)
            {
                this.client.SendRawMessage("PART #" + this.channelName);
            }
        }

        public void SendWHO()
        {
            if (this.client.IsConnected && this.isWaitingOnWho == false)
            {
                this.isWaitingOnWho = true;
                this.client.SendRawMessage("WHO #" + this.channelName);
            }
        }

        public void Disconnect()
        {
            LeaveChannel();

            if (this.client.IsConnected)
            {
                this.client.Disconnect();
            }

            this.client.Dispose();
        }

        # endregion

        # region Private Methods

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

        void client_RawMessageReceived(object sender, IrcRawMessageEventArgs e)
        {
            switch (e.Message.Command)
            {
                case "PRIVMSG":
                    if (e.Message.Source.Name != "jtv")
                    {
                        DoOnPrivMsg(e.Message.Source.Name, e.Message.Parameters[1]);
                    }

                    break;
                case "JOIN":
                    string nickname = e.Message.Source.Name;

                    if (username == nickname)
                    {
                        DoOnChannelJoin();
                    }
                    else
                    {
                        DoOnUserJoin(nickname);
                    }

                    break;
                case "PART":
                    DoOnUserPart(e.Message.Source.Name);

                    break;
                case "MODE":
                    bool isModerator = false;

                    if (e.Message.Parameters[1] == "+o")
                    {
                        isModerator = true;
                    }

                    DoOnUserMode(e.Message.Parameters[2], isModerator);

                    break;
                case "352":
                    if (this.isWaitingOnWho == true)
                    {
                        this.whoResults.Add(e.Message.Parameters[2]);
                    }

                    break;
                case "315":
                    this.isWaitingOnWho = false;
                    DoOnUserListCompleted(this.whoResults.ToArray());
                    this.whoResults.Clear();

                    break;
                case "353":
                    if (getParametersLength(e.Message.Parameters) > 3)
                    {
                        foreach (string nick in e.Message.Parameters[3].Split(' '))
                        {
                            this.userList.Add(nick);
                        }
                    }

                    break;
                case "366":
                    this.isWaitingOnUserList = false;
                    DoOnUserListCompleted(this.userList.ToArray());
                    this.userList.Clear();

                    break;
                default:
                    Console.WriteLine("<IN> - " + e.RawContent);

                    break;
            }
        }

        void client_Connected(object sender, EventArgs e)
        {
            client.SendRawMessage("JOIN #" + this.channelName);
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

        public delegate void PrivMsgHandler(object sender, IRCPrivMsgEventArgs e);
        public event PrivMsgHandler OnPrivMsg;
        private void DoOnPrivMsg(string nickname, string message)
        {
            if (OnPrivMsg != null)
            {
                OnPrivMsg(this, new IRCPrivMsgEventArgs { Nickname = nickname, Message = message });
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

    public class IRCPrivMsgEventArgs : EventArgs
    {
        public string Nickname { get; set; }
        public string Message { get; set; }
    }
}
