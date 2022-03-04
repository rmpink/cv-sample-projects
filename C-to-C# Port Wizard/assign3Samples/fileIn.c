/*
 * fileIn.c
 *
 * a simple source file demonstrating file input
 */

#include <stdio.h>
#include <string.h>

int main (void)
{
	// local variables
	int	fileLine = 1;
	FILE 	*fp;
	char	inBuff[100];

	// open the file
	fp = fopen ("test.txt", "r");
	if (fp == NULL) 
	{
		printf ("**Error: Unable to open file : test.txt for reading\n");
		return 1;
	}

	/* Walk through the file line by line counting and displaying
	   the file contents */
	printf("The following are the lines read from file \"test.txt\"\n");
	while (fgets(inBuff, 100, fp) != NULL)
	{
		printf ("%2d) [Length : %3d] %s", fileLine, strlen(inBuff), inBuff);
		fileLine++;
	}
	printf("<<< EOF\n");

	fclose (fp);

	return 0;
}
