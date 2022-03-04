///
/// \file 
/// \author Dan Hieronimus
///



using System;

using System.Net;

namespace TLVPackaging
{
    public class MessageState
	{
		public const int MESSAGE_TYPE_SIZE = 1;                     ///< Size in bytes of the type field.
        public const int MESSAGE_LENGTH_SIZE = sizeof(int);         ///< Size in bytes of the length field.


		// Tracking variables for determining how much of the message has been read.
        public int lengthBytesRead = 0;
        public int dataBytesRead = 0;

		public bool typeRead = false;
        public byte[] lengthBuffer = new byte[MESSAGE_LENGTH_SIZE];	

		public byte messageType = 0;
		public int messageLength = -1;
		public byte[] messageData = null;					        ///< Parsed out message data.


        public int RemainingLengthBytes
        {
            get
            {
                return MessageState.MESSAGE_LENGTH_SIZE - this.lengthBytesRead;
            }
        }

        public int RemainingDataBytes
        {
            get
            {
                int dataBytesReamining = 0;

                if (this.LengthRead == true)
                {
                    dataBytesReamining = this.messageLength - this.dataBytesRead;
                }

                return dataBytesReamining;
            }
        }

        public bool LengthRead
        {
            get
            {
                bool lengthRead = false;

                if (this.lengthBytesRead == MessageState.MESSAGE_LENGTH_SIZE)
                {
                    lengthRead = true;
                }

                return lengthRead;
            }
        }

        public bool HeaderRead
        {
            get
            {
                return this.typeRead & this.LengthRead;         ///< oh no you didn't!
            }
        }

        public bool MessageComplete
        {
            get
            {
                bool messageComplete = false;

                if ((this.typeRead == true) && (this.LengthRead == true) && (this.dataBytesRead == this.messageLength))
                {
                    messageComplete = true;
                }

                return messageComplete;
            }
        }


		///
		/// \brief <b>Brief Description:</b> Resets the state of the StateObject.
		/// \details <b>Detailed Description:</b>
		///	Resets the state of the StateObject to its default values as if it were just constructed.
		///
		public void ResetMessageState()
		{
			this.typeRead = false;
            this.lengthBytesRead = 0;
            this.dataBytesRead = 0;
			this.lengthBuffer = new byte[4];

			this.messageType = 0;
			this.messageLength = -1;
			this.messageData = null;
		}

        public Message GetMessage()
        {
            return new Message(this.messageType, this.messageData);
        }
        public Message GetMessage(EndPoint sender)
        {
            Message message = new Message(this.messageType, this.messageData);
            message.Sender = sender;

            return message;
        }

        public MessageState GetDeepCopy()
        {
            MessageState messageStateCopy = new MessageState();

            messageStateCopy.lengthBytesRead = this.lengthBytesRead;
            messageStateCopy.dataBytesRead = this.dataBytesRead;

            messageStateCopy.typeRead = this.typeRead;
            messageStateCopy.lengthBuffer = new byte[MESSAGE_LENGTH_SIZE];
            Buffer.BlockCopy(this.lengthBuffer, 0, messageStateCopy.lengthBuffer, 0, MESSAGE_LENGTH_SIZE);

            messageStateCopy.messageType = this.messageType;
            messageStateCopy.messageLength = this.messageLength;

            if (this.messageData != null)
            {
                messageStateCopy.messageData = new byte[this.messageData.Length];
                Buffer.BlockCopy(this.messageData, 0, messageStateCopy.messageData, 0, this.messageLength);
            }
            else
            {
                messageStateCopy.messageData = null;
            }

            return messageStateCopy;
        }
	}
}