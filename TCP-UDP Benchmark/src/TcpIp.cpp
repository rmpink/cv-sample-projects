///
/// \file TcpIp.cpp
/// \author Dr.Pink and Commissar Hieronimus
///



#include "../inc/TcpIp.h"
#include <string.h>
#include <stdio.h>
#include <stdlib.h>

#include <sys/types.h>

#ifdef _WIN32
#include <Windows.h>
#elif defined __linux__
#include <netdb.h>
#include <netinet/in.h>
#include <netinet/tcp.h>
#include <unistd.h>
#endif


///
/// \brief <b>Brief Description:</b> TcpIp default constructor.
/// \details <b>Detailed Description:</b>
///	Default constructor for the TcpIp class. It initializes the comSocket and portNum to known values.
///
TcpIp::TcpIp(void)
{
	this->comSocket = -1;
	this->portNum = 0;
	this->connected = false;

#ifdef _WIN32
	WSAStartup(MAKEWORD(1,1), &wsa_data);
#endif
}



///
/// \brief <b>Brief Description:</b> Initiates a connection to another computer.
/// \details <b>Detailed Description:</b>
///	Connection method initiating a connection to another computer. Connects to the ip address specified on the port
/// specified and prints the status of the connection to the screen.
///
/// \params[in] ipAddress <b> const char * const </b> The IP address of the computer to connect to.
/// \params[in] portNum <b> int </b> The port number the connection will be made on.
/// 
/// \returns TBD <b> int </b> TBD
///
int TcpIp::Connect(const char * const ipAddress, int portNum)
{
	this->portNum = portNum;

	struct sockaddr_in serverAddr;
	struct hostent *server;

	int returnStatus = 0;
	
	memset(&serverAddr, 0, sizeof(serverAddr));
	
	
	printf("Init socket... ");
	fflush(stdout);
	
	if ((this->comSocket = (socket(AF_INET, SOCK_STREAM, 0))) != -1)
	{
		printf("Success\n");
		fflush(stdout);
		
		printf("Init hostname... ");
		fflush(stdout);
	
		if ((server = gethostbyname(ipAddress)) != NULL)
		{
			printf("Success\n");
			fflush(stdout);
			
			
			serverAddr.sin_family = AF_INET;
			serverAddr.sin_port = htons(portNum);
			memcpy(&serverAddr.sin_addr, server->h_addr, server->h_length);
		
		
			printf("Connecting... ");
			fflush(stdout);
		
			if (connect(this->comSocket, (struct sockaddr *) &serverAddr, sizeof(serverAddr)) != -1)
			{
				printf("Success\n");
				fflush(stdout);
			
				this->connected = true;
				returnStatus = 1;
				this->connected = true;
				// Hurrah we connected nao
			}
			else
			{
				// Failed to connect.
				printf("Failed\n");
				fflush(stdout);
			}
		}
		else
		{
			// Failed to get hostname.
			printf("Failed\n");
			fflush(stdout);
		}
	}
	else
	{
		// Failed to get socket.
		printf("Failed\n");
		fflush(stdout);
	}

	return returnStatus;
}


bool TcpIp::IsConnected() {

	return this->connected;
}


///
/// \brief <b>Brief Description:</b> Starts a listen server.
/// \details <b>Detailed Description:</b>
///	Starts a listen server for listening to and accepting incomming connections on the port number specified. Currently only
/// accepts 1 connection at a time.
///
/// \params[in] portNum <b> int </b> The port number the listen server will listen on.
/// 
/// \returns TBD <b> int </b> TBD
///
int TcpIp::Listen(int portNum)
{
	if (this->connected == false)
	{
		this->comSocket = 0;		// Reinit to 0
		this->portNum = portNum;
	
#ifdef __linux__
		socklen_t clientLength = 0;
#elif defined _WIN32
		int clientLength = 0;
#endif
		struct sockaddr_in serverAddr;
		struct sockaddr_in clientAddr;
	
		memset(&serverAddr, 0, sizeof(serverAddr));
		memset(&clientAddr, 0, sizeof(clientAddr));

	
		serverAddr.sin_family = AF_INET;
		serverAddr.sin_addr.s_addr = INADDR_ANY;
		serverAddr.sin_port = htons(portNum);
	
		printf("Init socket... ");
		fflush(stdout);

		if ((this->comSocket = socket(AF_INET, SOCK_STREAM, 0)) != -1)
		{
		
			// Lets try to ensure maximum TCP/IP speed by disabling the Nagle algorithm.
			int sockOpt = 1;
			setsockopt(this->comSocket, IPPROTO_TCP, TCP_NODELAY, (const char*)&sockOpt, sizeof(int));
		
			printf("Success\n");
		
			printf("Binding... ");
			fflush(stdout);
	
			if (bind(this->comSocket, (struct sockaddr *) &serverAddr, sizeof(serverAddr)) != -1)
			{
				printf("Success\n");
			
				printf("Listening... ");
				fflush(stdout);
		
				if (listen(this->comSocket, LISTEN_QUEUE) != -1)
				{			
					this->connected = true;
				
					clientLength = sizeof(clientAddr);
				
					int clientSocket = accept(this->comSocket, (struct sockaddr *) &clientAddr, &clientLength);
					this->connections.push_back(clientSocket);
				
					printf("WOPA\n");
				}
				else
				{
					// Failed to listen.
					printf("Failed\n");
					fflush(stdout);
				}
			}
			else
			{
				// Failed to bind socket.
				printf("Failed\n");
				fflush(stdout);
			}
		}
		else
		{
			// Failed to init socket.
			printf("Failed\n");
			fflush(stdout);
		}
	}

	return 0;
}



///
/// \brief <b>Brief Description:</b> Close current connection.
/// \details <b>Detailed Description:</b>
///	Closes the connection if it is open. Applies to listening and connecting.
///
void TcpIp::Disconnect(void)
{
	if (this->connected == true)
	{
		this->SendMsg(DISCONNECT, 0, "");
	
#ifdef _WIN32
		closesocket(this->connections[0]);
		closesocket(this->comSocket);
#elif defined __linux__
		close(this->connections[0]);
		close(this->comSocket);
#endif
		
		this->connections.clear();
		this->connected = false;
		printf("Connection closed\n");
	}
}



///
/// \brief <b>Brief Description:</b> Reads a message from the TCP/IP stack
/// \details <b>Detailed Description:</b>
/// Attempts to read a message if it exists from the TCP/IP stack. If a message exists, it will determine the type and length, 
/// and then allocate enough memory to finally store the message. A pointer to the message is returned to the caller which must
/// be free'd by the caller at some point.
///
/// \params[out] size <b> unsigned short * </b> The length of the message received.
/// \params[out] message <b> char * </b> The contents of the message in a byte array.
///
/// \returns messageType <b> int </b> The type of message received. -1 Means error.
///
int TcpIp::ReadMsg(unsigned short *size, char **message)
{
	//printf("Reading msg\n");

	char *recvMessage = NULL;
	char messageHeader[MESSAGE_HEADER_SIZE] = "";
	char messageType = 0;
	unsigned short messageLength = 0;	
	int bytesRead = 0;
	bool awaitNewMessage = true;

	if (this->connected == true)
	{
		if (awaitNewMessage == true)
		{	
			int headerBytesRead = 0;
			

			while(headerBytesRead < 3)
			{
				if ((bytesRead = recv(this->connections[0], messageHeader + headerBytesRead, 1, 0)) > 0)
				{
					//printf("Bytes read %d\n", bytesRead);
					headerBytesRead += bytesRead;
				}
				else
				{
					// Failed to read header.
					//printf("Failed to read header\n");
					messageType = MSG_ERROR;
					break;
				}
			}

			if (messageType != MSG_ERROR)
			{
				//printf("header bytes %d\n", headerBytesRead);
				messageType = messageHeader[0];
				messageLength = messageHeader[2] << 8 | messageHeader[1];
				*size = messageLength;
			
				//printf("Message Type: %d Message Length: %d\n", messageType, messageLength);
				fflush(stdout);	

				awaitNewMessage = false;
			}
		}
		
		if (awaitNewMessage == false)
		{
			unsigned short messageBytesRead = 0;
		
			if ((recvMessage = (char*) malloc(messageLength + 1)) != NULL)
			{
				memset(recvMessage, 0, messageLength + 1);
			
				while(messageBytesRead < messageLength)
				{	
					//printf("Waiting for new message\n");
					if ((bytesRead = recv(this->connections[0], recvMessage + messageBytesRead, 
						(messageLength - messageBytesRead), 0)) > 0)
					{
						messageBytesRead += bytesRead;
					}
					else
					{
						messageType = MSG_ERROR;
						//printf("Error reading message %d\n", bytesRead);
						fflush(stdout);
						break;
					}
					
					//printf("bytes read: %d  length: %d\n", messageBytesRead, messageLength);
					fflush(stdout);
				}
				
				awaitNewMessage = true;
			}
			else
			{
				printf("Failed to allocate memory for message\n");
				messageType = MSG_ERROR;
			}	
		}
		
		*message = recvMessage;
	}
	
	return messageType;
}



///
/// \brief <b>Brief Description:</b> Sends a TLV message.
/// \details <b>Detailed Description:</b>
/// Formats a message into the TLV and sends it across the network.
///
/// \params[in] portNum <b> int </b> The port number the listen server will listen on.
/// 
/// \returns bytes <b> int </b> The number of bytes sent.
///
int TcpIp::SendMsg(char type, unsigned short length, const char *message)
{
	int bytesSent = 0;
	char *sendMessage = NULL;
	
	if ((sendMessage = (char *) malloc(length + MESSAGE_HEADER_SIZE)) !=NULL)
	{
		// Sacrifice the upper bits to the bit god.
		char lowerLength = (char) length;
		char upperLength = (char) (length >> 8);
	
		sendMessage[0] = type;
		sendMessage[1] = lowerLength;
		sendMessage[2] = upperLength;
		strncpy(sendMessage + MESSAGE_HEADER_SIZE, message, length);
		bytesSent = send(this->comSocket, sendMessage, length + MESSAGE_HEADER_SIZE, 0);
		
		free(sendMessage);
		
		//printf("%d bytes sent\n", bytesSent);
	}
	else
	{
		//printf("Failed to build send message\n");
	}
	
	return bytesSent;
}
