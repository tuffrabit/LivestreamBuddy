using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LivestreamBuddyNew
{
    public class Options
    {
        public Options()
        {
            // Set default values
            OpenStreamsInNewTab = true;
            ShowStreamFeedWhenOpening = true;
            ShowTimestampsInChat = false;
            EnableDebugLogging = false;
            ChatTextSize = 14;
            ShowEmoticonsInChat = true;
            LogAllIRCTraffic = false;
        }

        public bool OpenStreamsInNewTab { get; set; }

        public bool ShowStreamFeedWhenOpening { get; set; }

        public bool ShowTimestampsInChat { get; set; }

        public bool EnableDebugLogging { get; set; }

        public int ChatTextSize { get; set; }

        public bool ShowEmoticonsInChat { get; set; }

        public bool LogAllIRCTraffic { get; set; }
    }
}
