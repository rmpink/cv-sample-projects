using System;
using System.Collections.Generic;
using System.Text;



namespace TLVPackaging
{
    public static class TLVPackage
    {
        public const int MAX_NORMAL_TLV_MESSGAE_LENGTH = 104857600;    // 100 Megabytes.
        public const int TLV_HEADER_SIZE = 5;

        public static byte[] PackTLVMessage(Message message)
        {
            return TLVPackage.PackTLVMessage(message.MessageType, message.MessageData);
        }

        ///
        /// \brief <b>Brief Description:</b> Packages data into a TLV message.
        /// \details <b>Detailed Description:</b>
        ///	Takes a message type and some message data and packages it into a TLV message.
        ///	
        /// \Throws <b> ArgumentOutOfRangeException </b> Message length was greater than TLVPackage.MAX_NORMAL_TLV_MESSGAE_LENGTH
        /// 
        /// \params[in] type <b> byte </b> The message type. The meaning of types is up to the implementation.
        /// \params[in] message <b> string </b> The message data.
        /// 
        /// \returns <b> byte[] </b> The packed TLV message as a byte array.
        ///
        public static byte[] PackTLVMessage(byte type, string message)
        {
            byte[] packedMessage = null;

            // Create TLV stream
            byte[] messageData = Encoding.UTF8.GetBytes(message);

            // Pack it
            packedMessage = TLVPackage.PackTLVMessage(type, messageData);

            return packedMessage;
        }


        public static byte[] PackTLVMessage(byte type, byte[] message)
        {
            byte[] packedMessage = null;


            if (message.Length <= TLVPackage.MAX_NORMAL_TLV_MESSGAE_LENGTH)
            {
                int fullMessageLength = TLV_HEADER_SIZE + message.Length;

                // Create our packed message array. This array will contain the header and the data.
                packedMessage = new byte[fullMessageLength];

                // Add the message type
                packedMessage[0] = type;

                // Add the message length
                Buffer.BlockCopy(BitConverter.GetBytes(message.Length), 0, packedMessage, 1, 4);
                Buffer.BlockCopy(message, 0, packedMessage, TLV_HEADER_SIZE, message.Length);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Message length must be less than TLVPackage.MAX_NORMAL_TLV_MESSGAE_LENGTH");
            }

            return packedMessage;
        }

        public static byte[] PackTLVMessages(List<Message> messages)
        {
            byte[] packedMessages = null;

            packedMessages = TLVPackage.PackTLVMessages(messages.ToArray());

            return packedMessages;
        }

        public static byte[] PackTLVMessages(Message[] messages)
        {
            List<byte> packedMessageList = new List<byte>();

            foreach (Message curMessage in messages)
            {
                packedMessageList.AddRange(TLVPackage.PackTLVMessage(curMessage));
            }

            return packedMessageList.ToArray();
        }

        public static List<Message> UnpackTLVMessages(Message message)
        {
            return TLVPackage.UnpackTLVMessages(message.MessageData);
        }

        public static List<Message> UnpackTLVMessages(byte[] readBuffer)
        {
            int readBufferIndex = 0;
            List<Message> messages = new List<Message>();
            MessageState messageState = new MessageState();     ///< Use for tracking the state of each message as it is parsed.

            while (readBufferIndex < readBuffer.Length)
            {
                readBufferIndex += TLVPackage.UnpackTLVMessageFromStream(readBuffer, readBufferIndex, readBuffer.Length, messageState);

                if (messageState.MessageComplete == true)
                {
                    messages.Add(messageState.GetMessage());
                    messageState.ResetMessageState();
                }
            }

            return messages;
        }


        public static int UnpackTLVMessageFromStream(byte[] readBuffer, int startIndex, int endIndex, MessageState messageState)
        {
            int bytesRead = 0;

            if (startIndex < endIndex)
            {
                if (messageState.typeRead == false)
                {
                    messageState.messageType = readBuffer[startIndex + bytesRead];
                    messageState.typeRead = true;
                    bytesRead++;
                }

                if (messageState.LengthRead == false)
                {
                    int bytesToRead = TLVPackage.NumBytesCanRead(startIndex + bytesRead, endIndex, messageState.RemainingLengthBytes);

                    Buffer.BlockCopy(readBuffer, startIndex + bytesRead, messageState.lengthBuffer, messageState.lengthBytesRead, bytesToRead);

                    messageState.lengthBytesRead += bytesToRead;
                    bytesRead += bytesToRead;

                    if (messageState.LengthRead == true)
                    {
                        messageState.messageLength = BitConverter.ToInt32(messageState.lengthBuffer, 0);
                        messageState.messageLength = System.Net.IPAddress.NetworkToHostOrder(messageState.messageLength);

                        if (messageState.messageLength < TLVPackage.MAX_NORMAL_TLV_MESSGAE_LENGTH)
                        {
                            messageState.messageData = new byte[messageState.messageLength];
                        }
                        else
                        {
                            // Uh oh, unrecoverable situation, throw an exception
                            throw new MessageCorruptException("Message likely corrupt. Length greater than: " + TLVPackage.MAX_NORMAL_TLV_MESSGAE_LENGTH);
                        }
                    }
                }

                if (messageState.HeaderRead == true)
                {
                    int bytesToRead = TLVPackage.NumBytesCanRead(startIndex + bytesRead, endIndex, messageState.RemainingDataBytes);

                    Buffer.BlockCopy(readBuffer, startIndex + bytesRead, messageState.messageData, messageState.dataBytesRead, bytesToRead);
                    messageState.dataBytesRead += bytesToRead;
                    bytesRead += bytesToRead;
                }
            }

            return bytesRead;
        }




        // Logic dump tool, determins the maximum number of bytes you can read from an array given a stareting index, ending index, and the maximum number
        // of bytes you want to read.
        private static int NumBytesCanRead(int curIndex, int endIndex, int bytesRequired)
        {
            int bytesCanRead = 0;
            int bytesAvailble = endIndex - curIndex;

            if (bytesAvailble >= bytesRequired)
            {
                bytesCanRead = bytesRequired;
            }
            else
            {
                bytesCanRead = bytesAvailble;
            }

            return bytesCanRead;
        }








        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////// Old strategies, keeping them for later benchmarking to see if the new strategies are faster ///////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /*
        public static List<Message> UnpackTLVMessages(byte[] readBuffer)
        {
            List<Message> unpackedMessages = null;

            unpackedMessages = TLVPackage.UnpackTLVMessages(readBuffer, 0, readBuffer.Length);

            return unpackedMessages;
        }


       

        //TODO: Needs an exception strategy, should I allow some messages to fail silently, or end the whole process
        public static List<Message> UnpackTLVMessages(byte[] readBuffer, int startIndex, int endIndex)
        {
            int readBufferIndex = startIndex;
            MessageState messageState = new MessageState();  // State object which tracks the progress of unpacking the current message.
            List<Message> unpackedMessages = new List<Message>();

            while (readBufferIndex < endIndex)
            {
                if (messageState.typeRead == false)
                {
                    messageState.messageType = readBuffer[readBufferIndex];
                    messageState.typeRead = true;
                    readBufferIndex++;
                }
                else if (messageState.LengthRead == false)
                {
                    Buffer.BlockCopy(readBuffer, readBufferIndex, messageState.lengthBuffer, 0, MessageState.MESSAGE_LENGTH_SIZE);
                    messageState.messageLength = BitConverter.ToInt32(messageState.lengthBuffer, 0);

                    messageState.lengthBytesRead = MessageState.MESSAGE_LENGTH_SIZE;
                    readBufferIndex += MessageState.MESSAGE_LENGTH_SIZE;

                    if (messageState.messageLength < TLVPackage.MAX_NORMAL_TLV_MESSGAE_LENGTH)
                    {
                        messageState.messageData = new byte[messageState.messageLength];
                    }
                    else
                    {
                        //TODO: Unrecoverable situation, should be passed to the server so the connection can be closed. (Possible 4th wall breakage?)
                        //TODO: Perhaps the perfect place to throw an exception
                        throw new Exception("Message length corrupt");
                    }
                }

                if (messageState.HeaderRead == true)
                {
                    Buffer.BlockCopy(readBuffer, readBufferIndex, messageState.messageData, 0, messageState.messageLength);
                    readBufferIndex += messageState.messageLength;
                    messageState.dataBytesRead += messageState.messageLength;

                    unpackedMessages.Add(messageState.GetMessage());
                    messageState.ResetMessageState();
                }
            }

            return unpackedMessages;
        }
        */
    }
}
