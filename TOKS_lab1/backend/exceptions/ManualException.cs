using System;

namespace TOKS_lab1.backend.exceptions
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