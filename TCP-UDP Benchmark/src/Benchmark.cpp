///
/// \file Benchmark.cpp
/// \author Dr.Pink and Commissar Hieronimus
///



#include "../inc/Benchmark.h"
#include <limits.h>
#include <time.h>
#include <stdio.h>

#include "../inc/TcpIp.h"
#include "../inc/Udp.h"

TcpIp* testTCP = new TcpIp();

///
/// \brief <b>Brief Description:</b> Benchmark default constructor.
/// \details <b>Detailed Description:</b>
/// Default constructor for the Benchmark class. It initializes the mStartTime, mStopTime, and mIsRunning flag to 0.
///
Benchmark::Benchmark(int argc, char** argv) {

	mStartTime = 0;
	mStopTime  = 0;
	
	mIsRunning = false;

	mArguments = ParseArgs(argc, argv);
}

///
/// \brief <b>Brief Description:</b> Starts the benchmark.
/// \details <b>Detailed Description:</b>
/// Stars the benchmark if it is not already running.
///
bool Benchmark::Start() {

	if ( !mIsRunning ) {
	
		mStopTime  = 0;
		
		mStartTime = (int)time(NULL);
		mIsRunning = true;
	}
	
	return mIsRunning;
}



///
/// \brief <b>Brief Description:</b> Stops the benchmark.
/// \details <b>Detailed Description:</b>
/// Stops the benchmark if it is running.
///
bool Benchmark::Stop() {

	if ( mIsRunning ) {
	
		mStopTime = (int)time(NULL);
		mIsRunning = false;
	}
	
	return !mIsRunning;
}



///
/// \brief <b>Brief Description:</b> Accessor method for the benchmark's elapses time.
/// \details <b>Detailed Description:</b>
/// Determines and returns the elapsed time of the benchmark.
///
/// \returns timeElapsed <b> int </b> The time elapsed since the benchmark was started.
///
int Benchmark::GetTimeElapsed() {

	int timeElapsed = 0;
	
	if ( !mIsRunning ) {
	
		timeElapsed = mStopTime - mStartTime;
	}
	
	return timeElapsed;
}



///
/// \brief <b>Brief Description:</b> Begins a benchmark test.
/// \details <b>Detailed Description:</b>
/// Starts performing a benchmark test on the specified protocol with the specified message size and number of messages. This
///	test is current flawed as it only tests the time to push the data the NIC and does not take into account the time it takes for
/// the data to reach the computer.
///
/// \params[in] _protocol <b> int </b> Which protocol to use. TCP/IP or UDP
/// \params[in] messageType <b> int </b> The size of the message to be sent.
/// \params[in] _numBlocks <b> int </b> The number of times to send the message.
///
/// \returns timeElapsed <b> int </b> The time it took to push all the data to the NIC.
///
int Benchmark::RunTest( int _protocol, int _msgSize, int _numBlocks ) {

	int timeElapsed = 0;

	if ( _protocol == UDP_TEST ) {

		Udp* testUDP = new Udp();

		if ( testUDP->SecureHost(mArguments.serverAddr.c_str(), mArguments.portNum) ) {

			char* buffer = (char*)malloc(_msgSize);
			GenerateMessage( buffer, _msgSize );

			Start();

			for ( int i = 0; i < _numBlocks; ++i ) {
			
				testUDP->SendMsg( mArguments.serverAddr.c_str(), 7, _msgSize, buffer);
			}

			Stop();
			timeElapsed = GetTimeElapsed();

			free( buffer );
		}


	} else if ( _protocol == TCP_TEST ) {

		//TcpIp* testTCP = new TcpIp();

		if ( !testTCP->IsConnected() ) {

			testTCP->Connect( mArguments.serverAddr.c_str(), mArguments.portNum );

			char *buffer = (char*)malloc(_msgSize);
			GenerateMessage( buffer, _msgSize );

			Start();

			for ( int i = 0; i < _numBlocks; ++i ) {
				
				testTCP->SendMsg(2, _msgSize, buffer);
			}

			Stop();
			timeElapsed = GetTimeElapsed();

			//testTCP->Disconnect();

			free( buffer );
		} else {

			char *buffer = (char*)malloc(_msgSize);
			GenerateMessage( buffer, _msgSize );

			Start();

			for ( int i = 0; i < _numBlocks; ++i ) {
				
				testTCP->SendMsg(2, _msgSize, buffer);
			}

			Stop();
			timeElapsed = GetTimeElapsed();

			//testTCP->Disconnect();

			free( buffer );
		}
	}

	return timeElapsed;
}



///
/// \brief <b>Brief Description:</b> Determines if benchmark is acting as client or server.
/// \details <b>Detailed Description:</b>
///	This method determines if the benchmark is currently running as the server or as the client and returns the result.
/// 
/// \returns !isServer <b> bool </b> Whether or not the benchmark is acting as the client or server.
///
bool Benchmark::IsClient() {

	return !mArguments.isServer;
}

///
/// \brief <b>Brief Description:</b> Accessor for the benchmark arugments.
/// \details <b>Detailed Description:</b>
///	Accessor method for returning the benchmark arguments.
/// 
/// \returns mArguments <b> args </b> An args struct containing the arguments used in the benchmark.
///
args Benchmark::GetArgs() {

	return mArguments;
}


///
/// \brief <b>Brief Description:</b> Parses expected arguments from the command line and returns them.
/// \details <b>Detailed Description:</b>
///	Parses the expected command line arugments and returns them in a struct.
///
/// \params[in] argc <b> int </b> The number of arguments provided on the command line.
/// \params[in] argv <b> char *arg[] </b> An array of the arguments provided.
/// 
/// \returns args <b> args </b> An args struct containing the arguments expected from the command line.
///
args Benchmark::ParseArgs(int argc, char *argv[])
{
	args arguments;

	arguments.isServer = true;	// Assume server.
	arguments.portNum = 0;
	arguments.serverAddr = "";
	
	
	// Lets parse the command args ARHHG
	for (int i = 1; i < argc; i++)
	{
		// Test for run method (server/client)
		if ((strlen(argv[i]) == 2) && (strcmp(argv[i], "-s") == 0))
		{
			// Is server
			arguments.isServer = true;	// Already assumed but whatever.
		}
		else if ((strlen(argv[i]) == 2) && (strcmp(argv[i], "-c") == 0))
		{
			// Is client
			arguments.isServer = false;
		}
		else if ((strlen(argv[i]) > 2) && (argv[i][0] == '-') && (argv[i][1] == 'p'))
		{
			// get port
			arguments.portNum = atoi(argv[i] + 2);
		}
		else if ((strlen(argv[i]) > 2) && (argv[i][0] == '-') && (argv[i][1] == 'a'))
		{
			// Get address
			arguments.serverAddr.assign(&argv[i][2]);
		}
		else
		{
			// Print usage
		}	
	}
	
	printf("Server address: %s Port: %d\n", arguments.serverAddr.c_str(), arguments.portNum);
	fflush(stdout);
	
	return arguments;
}



///
/// \brief <b>Brief Description:</b> Generates a test message.
/// \details <b>Detailed Description:</b>
///	Generates a test message of random characters which can be used for sending across the network.
///
/// \params[in] buffer <b> char * </b> The buffer where the randomly generated message will be stored.
/// \params[in] msgSize <b> int </b> The size of the message to be randomly generated.
///
void Benchmark::GenerateMessage( char* buffer, int msgSize ) {

	for ( int i = 0; i < msgSize; ++i ) {

		buffer[i] = (char)(i%10 + 48);
	}
}