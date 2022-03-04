///
/// \file Udp.cpp
/// \author Dr.Pink and Commissar Hieronimus
///



#include "../inc/Udp.h"
#include <sys/types.h>
#include <string.h>
#include <stdio.h>


#ifdef _WIN32
#include <Windows.h>
#elif defined __linux__
#include <netdb.h>
#include <netinet/in.h>
#endif


///
/// \brief <b>Brief Description:</b> Udp default constructor.
/// \details <b>Detailed Description:</b>
/// Default constructor for the Udp class. It initializes the comSocket and portNum to known values.
///
Udp::Udp() {

	this->comSocket = -1;		///< Socket that the connection is made through
	this->portNum = 0;			///< Port number that messages are sent or received from.

#ifdef _WIN32
	WSAStartup(MAKEWORD(1,1), &wsa_data);
#endif
}



///
/// \brief <b>Brief Description:</b> Binds a socket and port number and looks up a host name.
///
/// \details <b>Detailed Description:</b>
/// Binds a socket and port number and looks up for a host name based on the ip address and port number provided.
///
/// \params[in] ipAddress <b> const char * const </b> The ip address we are using to get the host.
/// \params[in] portNum <b> int </b> The port number the socket will bound on.
 
/// \returns returnStatus <b> int </b> Status of securing a host. 0 = Failed, 1 = Success.
///
int Udp::SecureHost(const char * const ipAddress, int portNum) {

	this->portNum = portNum;

	struct sockaddr_in serverAddr;
	struct hostent *server;

	int returnStatus = 0;
	
	memset(&serverAddr, 0, sizeof(serverAddr));
	
	printf("UDP Socket Connection\n");
	printf("Init socket... ");
	fflush(stdout);
	
	if ((this->comSocket = (socket(AF_INET, SOCK_DGRAM, 0))) != -1)
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

			returnStatus = 1;
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


///
/// \brief <b>Brief Description:</b> Binds a UDP socket and port number.
/// \details <b>Detailed Description:</b>
/// Binds a socket and port number based on the port number provided for use with the UDP protocol.
///
/// \params[in] portNum <b> int </b> The port number the socket will bound on.
/// 
/// \returns returnStatus <b> int </b> The status of binding the socket. 0 = Failed, 1 = Success.
///
int Udp::BindSocket(int portNum) {
	
	this->comSocket = 0;		// Reinit to 0
	this->portNum = portNum;

	int returnStatus = 0;
	
#ifdef _WIN32
	int clientLength = 0;
#elif defined __linux__
	socklen_t clientLength = 0;
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

	if ((this->comSocket = socket(AF_INET, SOCK_DGRAM, 0)) != -1)
	{
		printf("Binding... ");
		fflush(stdout);
	
		if (bind(this->comSocket, (struct sockaddr *) &serverAddr, sizeof(serverAddr)) != -1)
		{
			printf("Success\n");
			fflush(stdout);
			
			printf("Waiting on Port %d...\n", portNum);

			returnStatus = 1;
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
		printf("Failed");
		fflush(stdout);
	}

	return returnStatus;
}



///
/// \brief <b>Brief Description:</b> Reads a UDP message from a socket.
/// \details <b>Detailed Description:</b>
/// Reads a message of a socket using the UDP protocol. The message is stored in the buffer provided.
///
/// \params[in] buffer <b> int </b> The buffer the message will be stored in.
/// \params[in] buf_size <b> int </b> The expected size of the message.
 
/// \returns bytesRead <b> int </b> The number of bytes read.
///
int Udp::ReadMsg( char* buffer, int buf_size ) {

	struct sockaddr_in serverAddr;
	struct sockaddr_in clientAddr;

#ifdef _WIN32
	int clientLength = sizeof(clientAddr);
#elif defined __linux__
	socklen_t clientLength = sizeof(clientAddr);
#endif
	
	memset(&serverAddr, 0, sizeof(serverAddr));
	memset(&clientAddr, 0, sizeof(clientAddr));

	
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_addr.s_addr = INADDR_ANY;
	serverAddr.sin_port = htons(this->portNum);
	
	//printf("Waiting on incoming message.\n");
	//fflush(stdout);

	int recvlen = recvfrom( this->comSocket, buffer, buf_size, 0, (struct sockaddr*)&clientAddr, &clientLength);
	buffer[recvlen-2] = '\0';

	recvlen -= 2;

	return recvlen;
}



///
/// \brief <b>Brief Description:</b> Sends a UDP message.
/// \details <b>Detailed Description:</b>
/// Sends a message using using the UDP protocol to the address specified.
///
/// \params[in] ipAddress <b> const char * const </b> The address we are sending the message to.
/// \params[in] type <b> int </b> The type of message we sending.
/// \params[in] length <b> short </b> The length of the message we are sending.
/// \params[in] message <b> const char * </b> The buffer containing the message we are sending.
///
/// \returns bytesSent <b> int </b> The number of bytes sent.
///
int Udp::SendMsg(const char * const ipAddress, int type, short length, const char *message) {

	struct hostent *hp;     /* host information */
	struct sockaddr_in serverAddr;    /* server address */
	char* newMsg = '\0';

	/* fill in the server's address and data */
	memset((char*)&serverAddr, 0, sizeof(serverAddr));
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_port = htons(this->portNum);

	/* look up the address of the server given its name */
	hp = gethostbyname(ipAddress);

	if (!hp) {

		fprintf(stderr, "could not obtain address of %s\n", ipAddress);
		fflush(stdout);
	} else {

		/* put the host's address into the server address structure */
		memcpy((void *)&serverAddr.sin_addr, hp->h_addr_list[0], hp->h_length);

		newMsg = new char[ length + 8 ];

#ifdef _WIN32
		//sprintf_s( newMsg, sizeof(message) + 8, "%02d%06d%s", type, length, message );
		sprintf( newMsg, "%02d%06d%s", type, length, message );
#elif defined __linux__
		sprintf( newMsg, "%02d%06d%s", type, length, message );
#endif
		//	"07061212hello, this is a 61,212 byte message! blah..."

		if ( sendto(this->comSocket, newMsg, strlen(newMsg), 0, (struct sockaddr *)&serverAddr, sizeof(serverAddr)) != -1) {

			// hurray, it was sent!
		}
	}

	return strlen(newMsg) - 2;
}
