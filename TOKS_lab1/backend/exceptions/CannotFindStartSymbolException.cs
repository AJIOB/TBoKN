using System;

namespace TOKS_lab1.backend.exceptions
{
    public class CannotFindStartSymbolException : ManualException
    {
        private const string DefaultMessage = "Cannot find start symbol";

        public CannotFindStartSymbolException()
            : base(DefaultMessage)
        {
        }

        public CannotFindStartSymbolException(Exception inner)
            : base(DefaultMessage, inner)
        {
        }
    }
}