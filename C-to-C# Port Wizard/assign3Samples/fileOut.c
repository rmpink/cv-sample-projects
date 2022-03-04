/*
 * fileOut.c
 *
 * a simple source file demonstrating file output
 */

#include <stdio.h>
#include <stdlib.h>

int main (void)
{
	FILE *fp;

	fp = fopen ("testOutput.txt", "w");
	if (fp == NULL) 
	{
		printf ("**Error: Unable to open file : testOutput.txt for writing\n");
		return 1;
	}

	fprintf (fp, "hello world\n");
	fclose (fp);

	printf("File testOutput.txt has been created\n");

	return 0;
}
