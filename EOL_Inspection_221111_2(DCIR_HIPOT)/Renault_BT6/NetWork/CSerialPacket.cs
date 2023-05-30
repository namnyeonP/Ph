using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renault_BT6.외부모듈
{

    public enum enumSerialDataType
    {
        STRING,
        BYTE,
    }

    public enum enumSerialPacketType
    {
        RECV,
        SEND,
    }

    public class CSerialPacket
    {
        private string _strMessage = string.Empty;
        private enumSerialPacketType _enMessageType;
        private enumSerialDataType _enDataType;
        private byte[] _byBytes;

        public enumSerialPacketType MessageType
        {
            get
            {
                return this._enMessageType;
            }
            set
            {
                this._enMessageType = value;
            }
        }

        public enumSerialDataType DataType
        {
            get
            {
                return this._enDataType;
            }
            set
            {
                this._enDataType = value;
            }
        }

        public string Message
        {
            get
            {
                return this._strMessage;
            }
            set
            {
                this._strMessage = value;
            }
        }

        public byte[] Bytes
        {
            get
            {
                return this._byBytes;
            }
            set
            {
                this._byBytes = value;
            }
        }

        public CSerialPacket(enumSerialPacketType enType, string strMessage, Encoding encoder)
        {
            this._enMessageType = enType;
            this._enDataType = enumSerialDataType.STRING;
            this._strMessage = strMessage;
            this._byBytes = this.GetEncodedBytes(encoder);
        }

        public CSerialPacket(enumSerialPacketType enType, byte[] byData, int iLength, Encoding encoder)
        {
            this._enMessageType = enType;
            this._enDataType = enumSerialDataType.BYTE;
            this._byBytes = new byte[iLength];
            Buffer.BlockCopy((Array)byData, 0, (Array)this._byBytes, 0, iLength);
            this._strMessage = this.GetEncodedString(encoder);
        }

        public CSerialPacket(enumSerialPacketType enType, byte[] byData, Encoding encoder)
        {
            this._enMessageType = enType;
            this._enDataType = enumSerialDataType.BYTE;
            this._byBytes = byData;
            this._strMessage = this.GetEncodedString(encoder);
        }

        private byte[] GetEncodedBytes(Encoding encoder)
        {
            return encoder.GetBytes(this._strMessage.ToCharArray(), 0, this._strMessage.Length);
        }

        private string GetEncodedString(Encoding encoder)
        {
            return encoder.GetString(this._byBytes, 0, this._byBytes.Length);
        }

        public byte[] GetRawByteFromString()
        {
            byte[] numArray = new byte[this._strMessage.Length];
            for (int index = 0; index < this._strMessage.Length; ++index)
                numArray[index] = (byte)this._strMessage[index];
            return numArray;
        }

        public string GetStringFromRawByte()
        {
            StringBuilder stringBuilder = new StringBuilder(this._byBytes.Length);
            for (int index = 0; index < this._byBytes.Length; ++index)
                stringBuilder.Append((char)this._byBytes[index]);
            return stringBuilder.ToString();
        }

        public string GetBytesLog()
        {
            StringBuilder stringBuilder = new StringBuilder(this._byBytes.Length);
            for (int index = 0; index < this._byBytes.Length; ++index)
            {
                stringBuilder.Append(string.Format("{0:X2} ", (object)this._byBytes[index]));
                stringBuilder.Append(' ');
            }
            return stringBuilder.ToString();
        }

        public string GetPacketLog()
        {
            return this._strMessage;
        }
    }
}
