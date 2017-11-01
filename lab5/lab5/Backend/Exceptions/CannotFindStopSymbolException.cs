using System;

namespace lab5.Backend.Exceptions
{
    public class CannotFindStopSymbolException : ManualException
    {
        private new const string Message = "Cannot find stop symbol";

        public CannotFindStopSymbolException()
            : base(Message)
        {
        }

        public CannotFindStopSymbolException(Exception inner)
            : base(Message, inner)
        {
        }
    }
}