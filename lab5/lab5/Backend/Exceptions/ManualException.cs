using System;

namespace lab5.Backend.Exceptions
{
    public class ManualException : Exception
    {
        protected ManualException()
        {
        }

        protected ManualException(string message)
            : base(message)
        {
        }

        protected ManualException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}