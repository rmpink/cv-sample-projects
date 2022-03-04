using System;
using System.Net;
using System.Text;

namespace TLVPackaging
{
    public class Message
    {
        private byte _messageType = 0;
        private byte[] _messageData = null;
        private EndPoint _sender = null;

        public byte MessageType
        {
            get
            {
                return this._messageType;
            }
        }

        public byte[] MessageData
        {
            get
            {
                return this._messageData;
            }
        }

        public int MessageLength
        {
            get
            {
                int messageLength = 0;

                if (this._messageData != null)
                {
                    messageLength = this._messageData.Length;
                }

                return messageLength;
            }
        }

        public EndPoint Sender
        {
            get
            {
                return this._sender;
            }
            set
            {
                this._sender = value;
            }
        }


        public Message(byte type, string message)
        {
            byte[] messageData = Encoding.UTF8.GetBytes(message);

            this._messageType = type;
            this._messageData = new byte[messageData.Length];

            Buffer.BlockCopy(messageData, 0, this._messageData, 0, messageData.Length);
        }

        public Message(byte type, int message)
        {
            byte[] messageData = BitConverter.GetBytes(message);

            this._messageType = type;
            this._messageData = new byte[messageData.Length];

            Buffer.BlockCopy(messageData, 0, this._messageData, 0, messageData.Length);
        }

        public Message(byte type, byte[] messageData)
        {
            this._messageType = type;
            this._messageData = new byte[messageData.Length];

            Buffer.BlockCopy(messageData, 0, this._messageData, 0, messageData.Length);
        }

        public Message(byte type, byte[] messageData, int size)
        {
            this._messageType = type;
            this._messageData = new byte[size];

            Buffer.BlockCopy(messageData, 0, this._messageData, 0, size);
        }
    }
}
