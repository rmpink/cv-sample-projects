/*
* times.c
*
* a simple file demonstrating console input/output
*/

using System;
using System.IO;
using System.Text;


namespace Something
{
	class Program
	{
		static void Main()
		{
			// local declarations
			char[] buffer = new char[100];
			int table;
			int x;
			int product;
			
			// get the user input as to which times-table to generate ...
			Console.Write ("Specify which TIMES-TABLE you would like to generate: ");
			buffer = Console.ReadLine().ToCharArray();
			table = Convert.ToInt32(new string (buffer));
			
			// now generate it !
			for (x = 1; x <= 10; x++) 
			{
				product = x * table;
			Console.WriteLine ("{0} x {1} = {2}", x, table, product);
			}
			
			return;
		}

	}
}
