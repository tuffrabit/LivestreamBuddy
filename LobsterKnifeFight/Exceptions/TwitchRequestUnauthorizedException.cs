using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LobsterKnifeFight
{
    public class TwitchRequestUnauthorizedException : Exception, ISerializable
    {
        public TwitchRequestUnauthorizedException() { }

        public TwitchRequestUnauthorizedException(string message) : base(message) { }

        public TwitchRequestUnauthorizedException(string message, Exception innerException) : base(message, innerException) { }

        protected TwitchRequestUnauthorizedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
