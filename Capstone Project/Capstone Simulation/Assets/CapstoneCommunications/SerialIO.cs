using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using TLVPackaging;
using UnityEngine;

namespace CapstoneCommunications
{
    public class SerialIO
    {
        public const byte FRAME_START = 0x12;           // Start byte 0x12
        public const byte FRAME_END = 0x13;             // End byte 0x13
        public const byte FRAME_ESCAPE = 0x7D;          // ESC 0x7D
        public const byte FRAME_ESCAPE_MASK = 0x20;     // Mask used to escape escaped chars
        public const byte END_OF_STREAM = 0xFF;         // End of stream as per MSDN is -1.
        public const int MAX_FRAME_SIZE = 1024;
        private int _bytesRead = 0;
        private bool _frameStart = false;
        private bool _frameEnd = false;
        private bool _escapeNextByte = false;
        private byte[] _readBuffer = new byte[SerialIO.MAX_FRAME_SIZE];      // If frame is larger than this size, it is dropped

        private SerialPort _serial = null;


        public void Init(string portName, int baudRate)
        {
            this._serial = new SerialPort(portName);
            this._serial.BaudRate = baudRate;
            // this._serial.ReadTimeout = timeOut;

            this._serial.Open();
        }

        public void ShutDown()
        {
            if (this._serial.IsOpen == true)
            {
                this._serial.Close();
            }
        }

        public void WriteFrame(byte[] buffer, int bufferLength)
        {
            int bytesWritten = 0;
            bool startFrameSent = false;
            List<byte> packedBytes = new List<byte>();


            while (bytesWritten < bufferLength)
            {
                if (startFrameSent == false)
                {
                    // Start of frame, pack start byte
                    packedBytes.Add(SerialIO.FRAME_START);
                    startFrameSent = true;
                }
                else if ((buffer[bytesWritten] == SerialIO.FRAME_START) ||
                         (buffer[bytesWritten] == SerialIO.FRAME_END) ||
                         (buffer[bytesWritten] == SerialIO.FRAME_ESCAPE) ||
                         (buffer[bytesWritten] == SerialIO.END_OF_STREAM))
                {
                    // Buffer contains an FCS flag, escape it.
                    packedBytes.Add(FRAME_ESCAPE);
                    packedBytes.Add((byte)((int)buffer[bytesWritten] ^ (int)SerialIO.FRAME_ESCAPE_MASK));
                    bytesWritten++;
                }
                else
                {
                    packedBytes.Add(buffer[bytesWritten]);
                    bytesWritten++;
                }

                if (bytesWritten == bufferLength)
                {
                    // End of Frame. Write End of Frame byte
                    packedBytes.Add(FRAME_END);
                }
            }

            byte[] packedByteArray = packedBytes.ToArray();
            this._serial.Write(packedByteArray, 0, packedByteArray.Length);
        }

        public void ResetState()
        {
            this._bytesRead = 0;
            this._frameStart = false;
            this._frameEnd = false;
            this._escapeNextByte = false;
        }

        public byte[] GetFrame()
        {
            byte[] newBuffer = null;

            if ((this._frameStart == true) && (this._frameEnd == true))
            {
                newBuffer = new byte[this._bytesRead];
                Buffer.BlockCopy(this._readBuffer, 0, newBuffer, 0, this._bytesRead);
                this.ResetState();
            }

            return newBuffer;
        }

        public byte[] ReadFrame()
        {
            byte[] completedFrame = null;
            byte curByte = 0;

            do
            {
                // Check if message completed before reading additional informatopn
                if ((completedFrame = this.GetFrame()) == null)
                {
                    curByte = (byte)this._serial.ReadByte();

                    if (curByte != SerialIO.END_OF_STREAM)
                    {
                        if (curByte == SerialIO.FRAME_START)
                        {
                            //	Serial.println("Frame start");

                            if (this._frameStart == true)
                            {
                                // Already reading a frame, encountered a new frame's start, drop old data
                                // and start the new frame
                                this._bytesRead = 0;
                                this._frameEnd = false;
                                this._escapeNextByte = false;
                                Debug.Log("Drop old frame, starting new one");
                            }

                            this._frameStart = true;
                        }
                        else if (this._frameStart == true)
                        {
                            // Frame has begun, start adding bytes to buffer
                            if (curByte != SerialIO.FRAME_END)
                            {
                                // Check if byte is escape character
                                if (curByte == SerialIO.FRAME_ESCAPE)
                                {
                                    this._escapeNextByte = true;
                                }
                                else if (this._escapeNextByte == true)
                                {
                                    // This byte was escaped, unescape it and write it to buffer
                                    this._readBuffer[this._bytesRead] = (byte)((byte)curByte ^ (byte)FRAME_ESCAPE_MASK);
                                    this._bytesRead++;
                                    this._escapeNextByte = false; // Escape byte was processed so reset this flag.
                                }
                                else
                                {
                                    // regular non-control / non-escaped byte
                                    this._readBuffer[this._bytesRead] = curByte;
                                    this._bytesRead++;
                                }
                            }
                            else
                            {
                                this._frameEnd = true;
                            }
                        }
                    }
                    else
                    {
                        // Stream ended, throw exception
                        
                        break;//throw new Exception("Stream ended somehow");
                    }
                }
            } while ((curByte != SerialIO.END_OF_STREAM) && (this._bytesRead < SerialIO.MAX_FRAME_SIZE) && (completedFrame == null));       // Buffer length is the size, while bytes read also acts as the index


            return completedFrame;
        }

        public void WriteMessage(Message message)
        {
            byte[] messageBytes = TLVPackage.PackTLVMessage(message);
            this.WriteFrame(messageBytes, messageBytes.Length);
        }

        //TODO: maybe handle multiple frames
        public Message ReadMessage()
        {
            byte[] messageBuffer = null;
            Message message = null;
            MessageState ms = new MessageState();

            messageBuffer = this.ReadFrame();

            if (messageBuffer != null)
            {
                // A frame was read. Parse out message and return it
                TLVPackage.UnpackTLVMessageFromStream(messageBuffer, 0, messageBuffer.Length, ms);

                if (ms.MessageComplete == true)
                {
                    message = ms.GetMessage();
                }
            }

            return message;
        }
    }
}