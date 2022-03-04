/*
* fileOut.c
*
* a simple source file demonstrating file output
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
			FileStream fp;
			
			fp = File.Open ("testOutput.txt", FileMode.OpenOrCreate, FileAccess.Write);
			if (fp == null) 
			{
				Console.WriteLine ("**Error: Unable to open file : testOutput.txt for writing");
				return;
			}
			
			fp.Write(Encoding.ASCII.GetBytes( "hello world\n"), 0, 12);
			fp.Close();
			
			Console.WriteLine("File testOutput.txt has been created");
			
			return;
		}

	}
}
