using System;

namespace TLVPackaging
{
    [Serializable]
    public class MessageCorruptException : Exception
    {
        public MessageCorruptException()
        {
        }

        public MessageCorruptException(string message)
            : base(message)
        {
        }

        public MessageCorruptException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
