///
/// \file Main.cpp
/// \author Dr.Pink and Commissar Hieronimus
///




#include "../inc/Benchmark.h"
#include "../inc/TcpIp.h"
#include "../inc/Udp.h"

#include <limits.h>



int main(int argc, char *argv[])
{
	Benchmark* bm = new Benchmark(argc, argv);
	args testArgs = bm->GetArgs();

	if ( !testArgs.isServer ) {

		printf("\n==========================================\n");
		printf(  " Test #1: 500,000 blocks of 1,001 bytes\n");
		printf(  "==========================================\n");
		int testTime = bm->RunTest( TCP_TEST, 1001, 500000);

		double blocksPerSec = 500000 / (float)testTime;
		double kBytesPerSec  = (500000*1001) / (float)testTime / 1024;
		printf("Test Complete!\t(Duration: %d seconds)\n\tAvg Blocks/s: %0.2f\n\tAvg KBytes/s: %0.2f\n", testTime, blocksPerSec, kBytesPerSec);

		printf("\n==========================================\n");
		printf(  " Test #2: 250,000 blocks of 2,001 bytes\n");
		printf(  "==========================================\n");
		testTime = bm->RunTest( TCP_TEST, 2001, 250000);
		
		blocksPerSec = 250000 / (float)testTime;
		kBytesPerSec  = (250000*2001) / (float)testTime / 1024;
		printf("Test Complete!\t(Duration: %d seconds)\n\tAvg Blocks/s: %0.2f\n\tAvg KBytes/s: %0.2f\n", testTime, blocksPerSec, kBytesPerSec);

		printf("\n==========================================\n");
		printf(  " Test #3: 100,000 blocks of 5,010 bytes\n");
		printf(  "==========================================\n");
		testTime = bm->RunTest( TCP_TEST, 5010, 100000);
		
		blocksPerSec = 100000 / (float)testTime;
		kBytesPerSec  = (100000*5010) / (float)testTime / 1024;
		printf("Test Complete!\t(Duration: %d seconds)\n\tAvg Blocks/s: %0.2f\n\tAvg KBytes/s: %0.2f\n", testTime, blocksPerSec, kBytesPerSec);

		printf("\n==========================================\n");
		printf(  " Test #4: 50,000 blocks of 10,001 bytes\n");
		printf(  "==========================================\n");
		testTime = bm->RunTest( TCP_TEST, 10001, 50000);
		
		blocksPerSec = 50000 / (float)testTime;
		kBytesPerSec  = (50000*10001) / (float)testTime / 1024;
		printf("Test Complete!\t(Duration: %d seconds)\n\tAvg Blocks/s: %0.2f\n\tAvg KBytes/s: %0.2f\n", testTime, blocksPerSec, kBytesPerSec);

		/* UDP TESTY THINGS
		printf("\n==========================================\n");
		printf(  " Test #1: 500,000 blocks of 1,001 bytes\n");
		printf(  "==========================================\n");
		int testTime = bm->RunTest( UDP_TEST, 1001, 500000);

		double blocksPerSec = 500000 / (float)testTime;
		double kBytesPerSec  = (500000*1001) / (float)testTime / 1024;
		printf("Test Complete!\t(Duration: %d seconds)\n\tAvg Blocks/s: %0.2f\n\tAvg KBytes/s: %0.2f\n", testTime, blocksPerSec, kBytesPerSec);

		printf("\n==========================================\n");
		printf(  " Test #2: 250,000 blocks of 2,001 bytes\n");
		printf(  "==========================================\n");
		testTime = bm->RunTest( UDP_TEST, 2001, 250000);
		
		blocksPerSec = 250000 / (float)testTime;
		kBytesPerSec  = (250000*2001) / (float)testTime / 1024;
		printf("Test Complete!\t(Duration: %d seconds)\n\tAvg Blocks/s: %0.2f\n\tAvg KBytes/s: %0.2f\n", testTime, blocksPerSec, kBytesPerSec);

		printf("\n==========================================\n");
		printf(  " Test #3: 100,000 blocks of 5,010 bytes\n");
		printf(  "==========================================\n");
		testTime = bm->RunTest( UDP_TEST, 5010, 100000);
		
		blocksPerSec = 100000 / (float)testTime;
		kBytesPerSec  = (100000*5010) / (float)testTime / 1024;
		printf("Test Complete!\t(Duration: %d seconds)\n\tAvg Blocks/s: %0.2f\n\tAvg KBytes/s: %0.2f\n", testTime, blocksPerSec, kBytesPerSec);

		printf("\n==========================================\n");
		printf(  " Test #4: 50,000 blocks of 10,001 bytes\n");
		printf(  "==========================================\n");
		testTime = bm->RunTest( UDP_TEST, 10001, 50000);
		
		blocksPerSec = 50000 / (float)testTime;
		kBytesPerSec  = (50000*10001) / (float)testTime / 1024;
		printf("Test Complete!\t(Duration: %d seconds)\n\tAvg Blocks/s: %0.2f\n\tAvg KBytes/s: %0.2f\n", testTime, blocksPerSec, kBytesPerSec);
		*/

	} else {

		TcpIp* serverTCP = new TcpIp();
		//Udp*   serverUDP = new Udp();
		int    msgLength = 0;
		char*  buffer = NULL;

		//printf("UDP Connection Socket: %d/t||/tTCP/IP Connection Socket: %d\n", testArgs.portNum, testArgs.portNum+1);
		
		//serverUDP->BindSocket( testArgs.portNum );
		serverTCP->Listen( testArgs.portNum );

		for ( ; ; ) {

			//msgLength = serverUDP->ReadMsg( buffer, SHRT_MAX );
			unsigned short msgLength = 0;
			int ret = 0;

			if ((ret = serverTCP->ReadMsg(&msgLength, &buffer)) ==  -1)
			{
				serverTCP->Disconnect();
				serverTCP->Listen(testArgs.portNum);
			}

			free(buffer);
		}
	}
	
	return 0;
}