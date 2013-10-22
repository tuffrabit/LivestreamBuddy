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
        }

        public bool OpenStreamsInNewTab { get; set; }

        public bool ShowStreamFeedWhenOpening { get; set; }

        public bool ShowTimestampsInChat { get; set; }
    }
}
