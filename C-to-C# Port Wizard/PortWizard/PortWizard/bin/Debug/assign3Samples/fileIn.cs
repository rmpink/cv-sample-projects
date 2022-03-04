/*
* fileIn.c
*
* a simple source file demonstrating file input
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
			// local variables
			int	fileLine = 1;
			FileStream fp;
			char[] inBuff = new char[100];
			
			// open the file
			fp = File.Open ("test.txt", FileMode.OpenOrCreate, FileAccess.Read);
			if (fp == null) 
			{
				Console.WriteLine ("**Error: Unable to open file : test.txt for reading");
				return;
			}
			
			/* Walk through the file line by line counting and displaying
			the file contents */
			Console.WriteLine("The following are the lines read from file \"test.txt\"");
			while ( fp.Read(Encoding.ASCII.GetBytes(inBuff, 0, inBuff.Length), 0,  100) != 0)
			{
			Console.Write ("{0}) [Length : {1}] {2}", fileLine, inBuff.Length, inBuff);
				fileLine++;
			}
			Console.WriteLine("<<< EOF");
			
			fp.Close();
			
			return;
		}

	}
}
