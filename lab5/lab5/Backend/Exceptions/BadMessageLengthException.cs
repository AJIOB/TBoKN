using System;

namespace lab5.Backend.Exceptions
{
    public class BadMessageLengthException : ManualException
    {
        private const string DefaultMessage = "Message has bad length";

        public BadMessageLengthException()
            : base(DefaultMessage)
        {
        }

        public BadMessageLengthException(Exception inner)
            : base(DefaultMessage, inner)
        {
        }
    }
}