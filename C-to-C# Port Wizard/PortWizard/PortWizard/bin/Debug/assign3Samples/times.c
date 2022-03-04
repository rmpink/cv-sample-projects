/*
 * times.c
 *
 * a simple file demonstrating console input/output
 */

#include <stdio.h>
#include <stdlib.h>

int main (void)
{
	// local declarations
	char buffer[100];
	int table;
	int x;
	int product;

	// get the user input as to which times-table to generate ...
	printf ("Specify which TIMES-TABLE you would like to generate: ");
	gets (buffer);
	table = atoi (buffer);

	// now generate it !
	for (x = 1; x <= 10; x++) 
	{
		product = x * table;
		printf ("%3d x %3d = %4d\n", x, table, product);
	}

	return 0;
}
