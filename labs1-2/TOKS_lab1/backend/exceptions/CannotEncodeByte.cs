using System;

namespace TOKS_lab1.backend.exceptions
{
    public class CannotEncodeByte : ManualException
    {
        private const string Prefix = "Cannot encode byte. ";
        public CannotEncodeByte()
        {
        }

        public CannotEncodeByte(string message)
            : base(Prefix + message)
        {
        }

        public CannotEncodeByte(string message, Exception inner)
            : base(Prefix + message, inner)
        {
        }
    }
}