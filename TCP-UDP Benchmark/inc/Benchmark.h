///
/// \file Benchmark.h
/// \author Dr.Pink and Commissar Hieronimus
///


#ifndef _IAD_BENCHMARK_
#define _IAD_BENCHMARK_


#include <string>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>


#define UDP_TEST 0
#define TCP_TEST 1


///
/// \struct args
/// \brief Stores the parsed command line arguments.
///
struct args
{
	bool isServer;
	int portNum;
	std::string serverAddr;
};


/// \class Benchmark
/// \brief Provides benchmark tests for UDP and TCP communications.
///
/// \detailed
/// Contains a series of tests for benchmark UDP and TCP communications against each other. The benchmarks are performed
/// by timing how long a series of a data blocks take to be transmitted from one computer to the next.
///
class Benchmark {

public:

	/////////////////////////////
	// Consturctors/Destructor //
	/////////////////////////////
	Benchmark( int argc, char** argv );
	~Benchmark( void );
	
	//////////////////////////
	// Benchmarking methods //
	//////////////////////////
	bool Start( void );
	bool Stop( void );
	int GetTimeElapsed( void );
	int RunTest( int _protocol, int _msgSize, int _numBlocks );

	
	//////////////////////
	// Input arguments //
	/////////////////////
	bool IsClient( void );
	args GetArgs( void );

private:
	bool mIsRunning;	///< Benchmark is running and time is being kept.

    int mStartTime;		///< Time the benchmark was started.
    int mStopTime;		///< Time the benchmark was stopped.

    args mArguments;	///< Arguments for running the benchmark.

    args ParseArgs(int argc, char *argv[]);
    void GenerateMessage( char* buffer, int msgSize );
	
};


#endif
