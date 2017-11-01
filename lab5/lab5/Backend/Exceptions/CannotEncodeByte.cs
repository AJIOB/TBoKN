using System;

namespace lab5.Backend.Exceptions
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