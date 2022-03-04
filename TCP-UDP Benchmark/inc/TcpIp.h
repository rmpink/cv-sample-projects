///
/// \file TcpIp.h
/// \author Dr.Pink and Commissar Hieronimus
///



#include <vector>


#define LISTEN_QUEUE 5
#define MESSAGE_HEADER_SIZE 3
#define DISCONNECT 4
#define MSG_ERROR -1


#ifdef _WIN32
#include <winsock.h>
#endif


/// \class TcpIp
/// \brief Creates or listens to a Tcp/IP connection for sending and recieving data.
///
/// \detailed
/// The TcpIp class contains the methods required for establishing a TCP/IP connection. It is capable
/// of either acting as a server and listening for connections, or attempting a connection to an already running
/// server. In addition, 2 methods are provided for sending and receiving messages. Currently only 1 connection
/// is supported.
///
class TcpIp
{
private:

std::vector<int> connections;	///< For future use with multiple connections.
int comSocket;					///< The socket used for listening or connecting to another computer.
int portNum;					///< The port number the current connection is on.
bool connected;					///< Determines whether or not the server is running/client is connected.

#ifdef _WIN32
WSAData wsa_data;
#endif

public:

/////////////////
// Constructor //
/////////////////
TcpIp(void);

////////////////////////
// Connection methods //
////////////////////////
int Connect(const char * const ipAddress, int portNum);
int Listen(int portNum);
void Disconnect(void);


///////////////
// Accessors //
///////////////
bool IsConnected(void);


/////////////////////////////
// Message sending methods //
/////////////////////////////
int ReadMsg(unsigned short *size, char **message);
int SendMsg(char type, unsigned short length, const char *message);

};
