using System;
using System.Collections.Generic;
using lab5.Backend.Exceptions;

namespace lab5.Backend
{
    public class SerialPackage : IComparable <SerialPackage>
    {
        private const byte StartStopByte = 0x7E;
        private const byte EscapeByte = 0x7D;
        private const byte StartStopByteSecondInEscape = 0x5E;
        private const byte EscapeByteSecondInEscape = 0x5D;

        private const byte MessageMaxLen = 0xFF;

        #region Fields

        public bool IsToken { get; private set; } = false;
        public byte DestinationAddress { get; private set; } = 0;
        public byte SourceAddress { get; private set; } = 0;
        public string Info { get; private set; } = string.Empty;
        public bool IsRecognized { get; set; } = false;
        public bool IsDataGetted { get; set; } = false;

        #endregion

        private SerialPackage()
        {
        }

        #region Static methods

        /// <summary>
        /// Parse packet from byte sequence
        /// </summary>
        /// <param name="packetBytes">Received packet bytes</param>
        /// <param name="result">Parsing result</param>
        /// <returns>True, if package was successfully parsed, else false</returns>
        public static bool TryParsePackage(IEnumerable<byte> packetBytes, out SerialPackage result)
        {
            //TODO: write
            result = null;
            return false;
        }

        /// <summary>
        /// Generate package
        /// </summary>
        /// <param name="sourceId">Package source ID</param>
        /// <param name="destId">Package destination ID</param>
        /// <param name="message">Message to write</param>
        /// <returns>Generated package</returns>
        public static SerialPackage GeneratePackage(byte sourceId, byte destId, string message)
        {
            if (message.Length > MessageMaxLen)
            {
                throw new BadMessageLengthException();
            }

            return new SerialPackage
            {
                DestinationAddress = destId,
                SourceAddress = sourceId,
                Info = message
            };
        }

        /// <summary>
        /// Generate token package
        /// </summary>
        /// <returns>Generated package</returns>
        public static SerialPackage GenerateToken()
        {
            return new SerialPackage { IsToken = true };
        }

        #endregion

        /// <summary>
        /// Generate byte array to write data in serial port
        /// </summary>
        /// <returns>Byte array to write to serial port</returns>
        public byte[] BytesToWrite()
        {
            //TODO
            return new byte[0];
        }

        /// <summary>
        /// Compare with other SerialPackage use with priority
        /// </summary>
        /// <param name="other">Second comparable operand</param>
        /// <returns>Compare result</returns>
        public int CompareTo(SerialPackage other)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}