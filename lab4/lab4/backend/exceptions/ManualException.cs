using System;

namespace lab4.backend.exceptions
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