using System;

namespace lab4.backend.exceptions
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