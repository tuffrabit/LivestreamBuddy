using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestreamBuddy
{
    public class Stream
    {
        public Int64 Id { get; set; }

        public string Game { get; set; }

        public bool IsOnline { get; set; }

        public Int64 ViewerCount { get; set; }

        public Channel Channel { get; set; }
    }
}
