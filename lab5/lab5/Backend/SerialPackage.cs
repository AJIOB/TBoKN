using System;
using System.Collections.Generic;
using System.Text;
using lab5.Backend.Exceptions;

namespace lab5.Backend
{
    public class SerialPackage : IComparable<SerialPackage>
    {
        private const byte StartStopByte = 0x7E;

        private const int TokenBitNum = 4;
        private const int IsRecognizedBitNum = 7;
        private const int IsDataGettedBitNum = 6;

        private const byte MessageMaxLen = 0xFF;

        private static readonly Encoding LocalEncoging = Encoding.UTF8;

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
            if (LocalEncoging.GetByteCount(message) > MessageMaxLen)
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
            List<byte> packetBytes = new List<byte>();

            //SD (Starting delimiter)
            packetBytes.Add(StartStopByte);

            //AC (Access Control)
            packetBytes.Add(SetBitInByte(IsToken, TokenBitNum));

            //DA, SA
            packetBytes.Add(DestinationAddress);
            packetBytes.Add(SourceAddress);

            //LEN, INFO
            byte[] encodedInfo = LocalEncoging.GetBytes(Info);
            packetBytes.Add((byte)encodedInfo.Length);
            packetBytes.AddRange(encodedInfo);

            //ED (Ending delimiter)
            packetBytes.Add(StartStopByte);

            //FS (Frame status)
            packetBytes.Add((byte) (SetBitInByte(IsDataGetted, IsDataGettedBitNum) | SetBitInByte(IsRecognized, IsRecognizedBitNum)));

            return packetBytes.ToArray();
        }

        /// <summary>
        /// Compare with other SerialPackage use with priority
        /// </summary>
        /// <param name="other">Second comparable operand</param>
        /// <returns>Compare result. If it is less than zero => this instance is less than value. If zero => this instance is equal to value. Greater than zero => this instance is greater than value</returns>
        public int CompareTo(SerialPackage other)
        {
            if (IsToken && other.IsToken)
            {
                return 0;
            }
            if (IsToken)
            {
                return -1;
            }
            if (other.IsToken)
            {
                return 1;
            }
            return SourceAddress.CompareTo(other);
        }

        /// <summary>
        /// Set bit in empty (0x00) byte to required value
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <param name="pos">Bit position</param>
        /// <returns></returns>
        public static byte SetBitInByte(bool value, int pos)
        {
            return (byte)(Convert.ToInt32(value) << pos);
        }
    }
}