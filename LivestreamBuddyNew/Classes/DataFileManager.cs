using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestreamBuddyNew
{
    public static class DataFileManager
    {
        private const string titleAutoCompleteFileName = "TitleAutoComplete.txt";
        private const string gameAutoCompleteFileName = "GameAutoComplete.txt";
        private const string channelAutoCompleteFileName = "ChannelAutoComplete.txt";
        private const string emoticonsCacheFileName = "Emoticons.txt";
        private const string favoriteChannelsFileName = "FavoriteChannels.txt";

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

                    reader.Close();
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
    }
}
