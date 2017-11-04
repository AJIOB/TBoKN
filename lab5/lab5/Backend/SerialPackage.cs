using System.Collections.Generic;

namespace lab5.Backend
{
    public class SerialPackage
    {
        private const byte StartStopByte = 0x7E;
        private const byte EscapeByte = 0x7D;
        private const byte StartStopByteSecondInEscape = 0x5E;
        private const byte EscapeByteSecondInEscape = 0x5D;
        private const int ReadTimeoutInMs = 100;

        #region Fields

        public bool IsToken { get; private set; } = false;
        public byte DestinationAddress { get; private set; } = 0;
        public byte SourceAddress { get; private set; } = 0;
        public string Info { get; private set; } = string.Empty;
        public bool IsRecognized { get; private set; } = false;
        public bool IsDataGetted { get; private set; } = false;

        #endregion

        private SerialPackage()
        {
        }

        /// <summary>
        /// Parse packet from byte sequence
        /// </summary>
        /// <param name="packetBytes">packet bytes</param>
        /// <returns></returns>
        public static SerialPackage ParsePackage(IEnumerable<byte> packetBytes)
        {
            //TODO: write
            return new SerialPackage();
        }

        /// <summary>
        /// Generate byte array to write data in serial port
        /// </summary>
        /// <returns></returns>
        public byte[] BytesToWrite()
        {
            //TODO
            return new byte[0];
        }
    }
}