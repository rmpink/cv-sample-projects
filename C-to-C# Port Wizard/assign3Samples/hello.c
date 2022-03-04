/*
 * hello.c
 *
 * a simple file demonstrating console output
 */

#include <stdio.h>
#include <string.h>

int main (void)
{
	int x = 1;
	std::string strX = "2";
	
	printf ("This is a sample piece of source code ... ");
	printf ("Hello world!\n");
	
	x = atoi(strX);
	
	return 0;
}

