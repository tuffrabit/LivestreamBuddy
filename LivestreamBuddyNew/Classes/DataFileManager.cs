using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LobsterKnifeFight;

namespace LivestreamBuddyNew
{
    public static class DataFileManager
    {
        private const string titleAutoCompleteFileName = "TitleAutoComplete.txt";
        private const string gameAutoCompleteFileName = "GameAutoComplete.txt";
        private const string emoticonsCacheFileName = "Emoticons.txt";
        private const string favoriteChannelsFileName = "FavoriteChannels.txt";
        private const string identitiesFileName = "identities.txt";

        public static string[] GetFavoriteChannels()
        {
            List<string> favoriteChannels = new List<string>();

            using (FileStream stream = File.Open(favoriteChannelsFileName, FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        favoriteChannels.Add(line);
                    }
                }
            }

            return favoriteChannels.ToArray();
        }

        public static void AddFavoriteChannel(string newChannel)
        {
            bool exists = false;

            foreach (string channel in GetFavoriteChannels())
            {
                if (string.Compare(newChannel, channel, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                using (StreamWriter writer = File.AppendText(favoriteChannelsFileName))
                {
                    writer.WriteLine(newChannel);
                }
            }
        }

        public static void RemoveFavoriteChannel(string channelName)
        {
            List<string> channels = new List<string>();

            foreach (string channel in GetFavoriteChannels())
            {
                if (string.Compare(channelName, channel, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    channels.Add(channel);
                }
            }

            using (StreamWriter writer = File.CreateText(favoriteChannelsFileName))
            {
                foreach (string channel in channels)
                {
                    writer.WriteLine(channel);
                }
            }
        }

        public static LobsterKnifeFight.User GetUser()
        {
            LobsterKnifeFight.User user = new LobsterKnifeFight.User();

            using (FileStream stream = File.Open(identitiesFileName, FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(' ');

                        switch (values[0])
                        {
                            case "name":
                                if (values.Length > 1)
                                {
                                    user.Name = values[1];
                                }
                                else
                                {
                                    user.Name = string.Empty;
                                }

                                break;
                            case "accesstoken":
                                if (values.Length > 1)
                                {
                                    user.AccessToken = values[1];
                                }
                                else
                                {
                                    user.AccessToken = string.Empty;
                                }
                                
                                break;
                            case "user_read":
                                user.AccessTokens.Add(LobsterKnifeFight.UserScope.UserRead, new AccessToken { Token = values[1], Retrieved = new DateTime(Convert.ToInt64(values[2])) });
                                break;
                            case "user_blocks_edit":
                                user.AccessTokens.Add(LobsterKnifeFight.UserScope.UserBlocksEdit, new AccessToken { Token = values[1], Retrieved = new DateTime(Convert.ToInt64(values[2])) });
                                break;
                            case "user_blocks_read":
                                user.AccessTokens.Add(LobsterKnifeFight.UserScope.UserBlocksRead, new AccessToken { Token = values[1], Retrieved = new DateTime(Convert.ToInt64(values[2])) });
                                break;
                            case "user_follows_edit":
                                user.AccessTokens.Add(LobsterKnifeFight.UserScope.UserFollowsEdit, new AccessToken { Token = values[1], Retrieved = new DateTime(Convert.ToInt64(values[2])) });
                                break;
                            case "channel_read":
                                user.AccessTokens.Add(LobsterKnifeFight.UserScope.ChannelRead, new AccessToken { Token = values[1], Retrieved = new DateTime(Convert.ToInt64(values[2])) });
                                break;
                            case "channel_editor":
                                user.AccessTokens.Add(LobsterKnifeFight.UserScope.ChannelEditor, new AccessToken { Token = values[1], Retrieved = new DateTime(Convert.ToInt64(values[2])) });
                                break;
                            case "channel_commercial":
                                user.AccessTokens.Add(LobsterKnifeFight.UserScope.ChannelCommercial, new AccessToken { Token = values[1], Retrieved = new DateTime(Convert.ToInt64(values[2])) });
                                break;
                            case "channel_stream":
                                user.AccessTokens.Add(LobsterKnifeFight.UserScope.ChannelStream, new AccessToken { Token = values[1], Retrieved = new DateTime(Convert.ToInt64(values[2])) });
                                break;
                            case "channel_subscriptions":
                                user.AccessTokens.Add(LobsterKnifeFight.UserScope.ChannelSubscriptions, new AccessToken { Token = values[1], Retrieved = new DateTime(Convert.ToInt64(values[2])) });
                                break;
                            case "channel_check_subscriptions":
                                user.AccessTokens.Add(LobsterKnifeFight.UserScope.ChannelCheckSubscription, new AccessToken { Token = values[1], Retrieved = new DateTime(Convert.ToInt64(values[2])) });
                                break;
                            case "chat_login":
                                user.AccessTokens.Add(LobsterKnifeFight.UserScope.ChatLogin, new AccessToken { Token = values[1], Retrieved = new DateTime(Convert.ToInt64(values[2])) });
                                break;
                        }
                    }
                }
            }

            return user;
        }

        public static void SetUser(LobsterKnifeFight.User user)
        {
            if (user != null)
            {
                using (StreamWriter writer = File.CreateText(identitiesFileName))
                {
                    writer.WriteLine("name " + user.Name);
                    writer.WriteLine("accesstoken " + user.AccessToken);

                    foreach (KeyValuePair<UserScope, AccessToken> kvp in user.AccessTokens)
                    {
                        writer.WriteLine(string.Format("{0} {1} {2}", EnumHelper.GetUserScope(kvp.Key), kvp.Value.Token, kvp.Value.Retrieved.Ticks.ToString()));
                    }
                }
            }
        }

        public static string[] GetStreamTitleAutoCompleteStrings()
        {
            return getAutoCompleteStrings(titleAutoCompleteFileName);
        }

        public static string[] GetStreamGameAutoCompleteStrings()
        {
            return getAutoCompleteStrings(gameAutoCompleteFileName);
        }

        private static string[] getAutoCompleteStrings(string fileName)
        {
            List<string> strings = new List<string>();

            if (!File.Exists(fileName))
            {
                File.Create(fileName);
            }

            using (StreamReader reader = File.OpenText(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    strings.Add(line);
                }
            }

            return strings.ToArray();
        }

        public static void AddStringToStreamTitleAutoComplete(string value)
        {
            addStringToAutoComplete(titleAutoCompleteFileName, value);
        }

        public static void AddStringToStreamGameAutoComplete(string value)
        {
            addStringToAutoComplete(gameAutoCompleteFileName, value);
        }

        private static void addStringToAutoComplete(string fileName, string value)
        {
            string[] strings = getAutoCompleteStrings(fileName);
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
                    }
                }
                catch { }
            }
        }

        public static List<Emoticon> GetEmoticons(bool refresh = false)
        {
            List<Emoticon> emoticons = new List<Emoticon>();

            if (!File.Exists(emoticonsCacheFileName))
            {
                File.Create(emoticonsCacheFileName);
                refresh = true;
            }

            if (refresh)
            {
                using (StreamWriter writer = File.CreateText(emoticonsCacheFileName))
                {
                    ChatManager chatManager = new ChatManager();

                    foreach (Emoticon emoticon in chatManager.GetEmoticons())
                    {
                        writer.WriteLine(emoticon.Regex + "," + emoticon.Url);
                    }
                }
            }

            using (StreamReader reader = File.OpenText(emoticonsCacheFileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');

                    emoticons.Add(new Emoticon { Regex = new Regex(parts[0].Trim(), RegexOptions.Compiled), Url = parts[1].Trim() });
                }
            }

            return emoticons;
        }
    }
}
