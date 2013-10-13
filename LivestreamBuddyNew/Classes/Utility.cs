using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LobsterKnifeFight;

namespace LivestreamBuddyNew
{
    public static class Utility
    {
        public static void GetAccessToken(User user, UserScope[] scopes, bool getNewAccessToken = false)
        {
            if (getNewAccessToken || string.IsNullOrEmpty(user.AccessToken))
            {
                AuthWindow authWindow = new AuthWindow(user, scopes);
                bool? result = authWindow.ShowDialog();

                if (result.Value == true && !string.IsNullOrEmpty(authWindow.AccessToken))
                {
                    user.AccessToken = authWindow.AccessToken;
                }

                DataFileManager.SetUser(user);
            }
        }

        public static void GetAccessToken(User user, bool getNewAccessToken = false)
        {
            GetAccessToken(user, new UserScope[] { UserScope.UserRead, UserScope.ChatLogin, UserScope.ChannelEditor }, getNewAccessToken);
        }

        public static void ClearUserData(User user)
        {
            user.Name = string.Empty;
            user.AccessToken = string.Empty;
            user.AccessTokens.Clear();

            DataFileManager.SetUser(user);
        }

        public static void Log(Type originator, string methodName, string message, string stacktrace)
        {
            try
            {
                DateTime now = DateTime.Now;
                string logFilename = string.Format("log{0}{1}{2}.txt",
                    now.Year.ToString(),
                    now.Month.ToString(),
                    now.Day.ToString());

                using (StreamWriter writer = File.AppendText(logFilename))
                {
                    string originatorString = "Unknown";

                    if (originator != null)
                    {
                        originatorString = originator.ToString();
                    }

                    if (string.IsNullOrEmpty(methodName))
                    {
                        methodName = "Unknown()";
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0}:{1}:{2}:{3} - {4}.{5} - {6}",
                        now.Hour.ToString(),
                        now.Minute.ToString(),
                        now.Second.ToString(),
                        now.Millisecond.ToString(),
                        originatorString,
                        methodName,
                        message);

                    if (!string.IsNullOrEmpty(stacktrace))
                    {
                        sb.Append("\r\nStacktrace: " + stacktrace);
                    }

                    writer.WriteLine(sb.ToString());
                }
            }
            catch { }
        }
    }
}
