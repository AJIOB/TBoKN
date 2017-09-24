using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using TOKS_lab1.backend.exceptions;
using TOKS_lab1.Enums;

namespace TOKS_lab1.backend
{
    public class SerialPortCommunicator
    {
        private SerialPort _serialPort;
        private const int BitsInByte = 8;
        private const byte StartStopByte = 0x7E;

        private const byte BitStaffingCheckMask = 0x7E;
        private const byte BitStaffingAndMask = 0xFE;
        private const byte BitStaffingReplaceSymbol = 0x7F;
        private const byte GetLastBitMask = 0x01;
        private readonly List<bool> _receivedBuffer = new List<bool>();

        public byte MyId { get; set; } = 0;
        public byte PartnerId { get; set; } = 0;

        public delegate void ReceivedEventHandler(object sender, EventArgs e);

        public bool IsOpen => _serialPort != null;

        /// <summary>
        /// Sending info to serial port
        /// </summary>
        /// <param name="s">Sending info</param>
        public void Send(string s)
        {
            InternalLogger.Log.Debug($"Sending string: \"{s}\"");
            var dataToSend = GeneratePacket(Encoding.UTF8.GetBytes(s).ToArray()).ToArray();
            _serialPort.Write(dataToSend, 0, dataToSend.Length);
        }

        /// <summary>
        /// Closing current connection
        /// </summary>
        public void Close()
        {
            if (!IsOpen) return;
            _serialPort.Close();
            _serialPort = null;
        }

        /// <summary>
        /// Open port with settings
        /// </summary>
        /// <param name="portName">Port name</param>
        /// <param name="baudRate">Baudrate</param>
        /// <param name="parity">Parity</param>
        /// <param name="dataBits">Number of data bits</param>
        /// <param name="stopBits">Number of stop bits</param>
        /// <param name="receivedEventHandler">Callback to run to when something is received</param>
        public void Open(string portName, EBaudrate baudRate, Parity parity, EDataBits dataBits, StopBits stopBits,
            ReceivedEventHandler receivedEventHandler)
        {
            if (IsOpen) return;
            _serialPort = new SerialPort(portName, (int) baudRate, parity, (int) dataBits, stopBits);
            _serialPort.Open();
            if (receivedEventHandler != null)
                _serialPort.DataReceived +=
                    delegate(object sender, SerialDataReceivedEventArgs args) { receivedEventHandler(sender, args); };
            _receivedBuffer.Clear();
        }

        /// <summary>
        /// Read existiong string
        /// </summary>
        /// <returns>Existing string</returns>
        public string ReadExisting()
        {
            //Reading data from serial port
            while (_serialPort.BytesToRead > 0)
            {
                var received = _serialPort.ReadByte();
                if (received < 0)
                {
                    InternalLogger.Log.Info("End of the stream was read");
                    break;
                }
                _receivedBuffer.AddRange(BytesToBools(new[] {(byte) received}));
            }

            IEnumerable<byte> data = null;
            try
            {
                int index;
                data = ParsePacket(_receivedBuffer, out index);
                _receivedBuffer.RemoveRange(0, index);
            }
            catch (CannotFindStopSymbolException)
            {
                //All is good
            }
            catch (CannotFindStartSymbolException)
            {
                _receivedBuffer.Clear();
                throw;
            }
            return data != null ? Encoding.UTF8.GetString(data.ToArray()) : "";
        }

        /// <summary>
        /// Encode byte array to bit array with adding bit staffing
        /// </summary>
        /// <param name="inputBytes">Bytes to convert with bit staffing</param>
        /// <returns>Converted input value</returns>
        private IEnumerable<bool> Encode(IEnumerable<byte> inputBytes)
        {
            var res = new List<bool>();
            var buffer = new bool[BitsInByte];

            foreach (var b in inputBytes)
            {
                var isFindByteToStuffing = (((b & BitStaffingAndMask) ^ BitStaffingCheckMask) == 0);
                var bitArray = (isFindByteToStuffing
                    ? new BitArray(new[] {BitStaffingReplaceSymbol})
                    : new BitArray(new[] {b}));

                if (bitArray.Length != BitsInByte)
                {
                    throw new CannotEncodeByte(
                        $"Bad length. Length was {bitArray.Length}, requred {BitsInByte}");
                }

                bitArray.CopyTo(buffer, 0);
                res.AddRange(buffer);
                if (isFindByteToStuffing)
                {
                    res.Add((b & GetLastBitMask) == 1);
                }
            }

            return res;
        }

        /// <summary>
        /// Decode bit array to byte array with deletinig bit staffing
        /// </summary>
        /// <param name="inputBits">Bits to decode with bit staffing</param>
        /// <param name="index">Index of first non-used element (number of used bits)</param>
        /// <returns>Decoded input value</returns>
        private IEnumerable<byte> Decode(IEnumerable<bool> inputBits, out int index)
        {
            var res = new List<byte>();
            var buffer = new List<bool>();
            index = 0;

            foreach (var b in inputBits)
            {
                buffer.Add(b);
                index++;
                if (buffer.Count < BitsInByte)
                {
                    continue;
                }

                var bitArray = new BitArray(buffer.ToArray());

                if (bitArray.Length != BitsInByte)
                {
                    //Escape decoding
                    res.Add((byte) ((BitStaffingReplaceSymbol & BitStaffingAndMask) | (b ? 0x1 : 0x0)));
                }
                else
                {
                    var byteBuff = new byte[1];
                    bitArray.CopyTo(byteBuff, 0);
                    if (byteBuff[0] == BitStaffingReplaceSymbol)
                    {
                        //Escape was found. Need one more bit to decode
                        continue;
                    }
                    if (byteBuff[0] == StartStopByte)
                    {
                        //stop symbol was found
                        return res;
                    }

                    //simple byte was found
                    res.Add(byteBuff[0]);
                }
                buffer.Clear();
            }

            throw new CannotFindStopSymbolException();
        }

        /// <summary>
        /// Check is packet to me
        /// </summary>
        /// <param name="packet">Packet to check</param>
        /// <returns>True if packet addressed to me, else false</returns>
        private bool IsPacketToMe(IEnumerable<byte> packet)
        {
            return (packet.First() == MyId);
        }

        /// <summary>
        /// Delete address metadata from data
        /// </summary>
        /// <param name="data">Data to delete address metadata</param>
        /// <returns>String without address metadata if data adressed to myId, else empty array</returns>
        private IEnumerable<byte> DeleteAddressMetadata(IEnumerable<byte> data)
        {
            var modifiedData = data.ToList();
            if (!IsPacketToMe(modifiedData))
            {
                return new byte[0];
            }

            modifiedData.RemoveRange(0, 2);
            return modifiedData;
        }

        /// <summary>
        /// Wrap address metadata to data
        /// </summary>
        /// <param name="data">Data to wrap</param>
        /// <returns>Wrapped string</returns>
        private IEnumerable<byte> WrapAddressMetadata(IEnumerable<byte> data)
        {
            var modifiedData = data.ToList();

            modifiedData.Insert(0, MyId);
            modifiedData.Insert(0, PartnerId);

            return modifiedData;
        }

        /// <summary>
        /// Generate packet from data
        /// </summary>
        /// <param name="data">Data to add to packet</param>
        /// <returns>Generated packet</returns>
        private IEnumerable<byte> GeneratePacket(IEnumerable<byte> data)
        {
            var encodedMeta = Encode(WrapAddressMetadata(data)).ToList();
            encodedMeta.InsertRange(0, BytesToBools(new []{StartStopByte}));
            encodedMeta.AddRange(BytesToBools(new []{StartStopByte}));
            return BoolsToBytes(encodedMeta);
        }

        /// <summary>
        /// Parse packet
        /// </summary>
        /// <param name="packet">Packet to parse</param>
        /// <param name="index">Index of first unused bool element in packet</param>
        /// <returns>Data from packet if packet addressed to me, else return null</returns>
        private IEnumerable<byte> ParsePacket(IEnumerable<bool> packet, out int index)
        {
            var listedPackage = packet.ToList();
            index = 0;
            bool isStartFound = false;
            while (listedPackage.Count >= BitsInByte)
            {
                if (BoolsToBytes(listedPackage.GetRange(index, BitsInByte)).ToArray()[0] == StartStopByte)
                {
                    isStartFound = true;
                    break;
                }
                index++;
            }
            if (!isStartFound)
            {
                throw new CannotFindStartSymbolException();
            }
            /*if (listedPackage.Last() != StartStopByte)
            {
                throw new CannotFindStopSymbolException();
            }*/

            index += BitsInByte;
            listedPackage.RemoveRange(0, index);

            int decodeIndex;

            var res = DeleteAddressMetadata(Decode(listedPackage, out decodeIndex));
            index += decodeIndex;
            return res;
        }

        /// <summary>
        /// Converts byte array to bool (bit) array 
        /// </summary>
        /// <param name="data">Data to convert</param>
        /// <returns>Bit array</returns>
        private IEnumerable<bool> BytesToBools(IEnumerable<byte> data)
        {
            var bitArray = new BitArray(data.ToArray());
            var bits = new bool[bitArray.Length];
            bitArray.CopyTo(bits, 0);
            return bits;
        }

        /// <summary>
        /// Converts bool (bit) array to byte array 
        /// </summary>
        /// <param name="data">Data to convert</param>
        /// <returns>Byte array</returns>
        private IEnumerable<byte> BoolsToBytes(IEnumerable<bool> data)
        {
            var bitArray = new BitArray(data.ToArray());
            var bytes = new byte[(bitArray.Length + BitsInByte - 1) / BitsInByte];
            bitArray.CopyTo(bytes, 0);
            return bytes;
        }
    }
}