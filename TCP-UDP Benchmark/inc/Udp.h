///
/// \file Udp.h
/// \author Dr.Pink and Commissar Hieronimus
///


#ifndef _IAD_UDP_
#define _IAD_UDP_


#ifdef _WIN32
#include <winsock.h>
#endif


/// \class Udp
/// \brief Binds and socket and provides methods for sending and receiving datagram messages.
///
/// \detailed
/// The Udp class contains the methods required for binding a socket and sending and receiving datagram messages. It currently
/// only supports binding one socket at a time.
///
class Udp
{
private:

int comSocket;	///< The socket used for communicating with another computer.
int portNum;	///< The port the socket is bound to.

#ifdef _WIN32
WSAData wsa_data;
#endif

public:

Udp(void);

int SecureHost(const char * const ipAddress, int portNum);
int BindSocket(int portNum);

int ReadMsg(char* buffer, int buf_size);  // Figure out a return method
int SendMsg(const char* const ipAddress, int type, short length, const char *message);

};


#endif