using System;

namespace TOKS_lab1.backend.exceptions
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