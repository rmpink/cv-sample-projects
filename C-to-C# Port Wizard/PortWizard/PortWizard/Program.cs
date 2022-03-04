///
///	\file Program.cs
///	Contains the test harness for the Parsy class.
///
///	\author	Dan Hieronimus, Ryan Pink
///



using System;
using System.IO;
using System.Text;

namespace PortWizard
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 4)
            {
                try
                {
                    PortFile(args[1], args[3]);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
            else if (args.Length == 1)
            {
                Console.WriteLine("usage: portwizard [-i filepath] [-o filepath]");
	            Console.WriteLine("\t-i filepath: The path to the input source file (*.c)");
                Console.WriteLine("\t-o filepath: The path to the output source file (*.cs)");
            }
            else
            {
                Console.WriteLine("Error: Incorrect arguments. Type 'portwizard' for usage. {0}", args.Length);
            }
        }


        ///
        /// \brief <b>Brief Description:</b> Reads in C source file, converts to C#, and writes to file
        /// \details <b>Detailed Description:</b>
        /// This method reads in a raw C source file (*.c) and passes in the source code, line by line,
        /// into the Parsy static class. It then appends the newly converted C# string to the output
        /// buffer. Upon completing the conversion, it writes to the given ouput path.
        ///
        /// \params[in] _in <b> string </b> The filepath of the incoming C source file
        /// \params[in] _out <b> string </b> The filepath of the outgoing C# source file
        ///
        static void PortFile( string _in, string _out ) {

            if (File.Exists(_in))
            {
                string[] inSrc = File.ReadAllLines(_in);
                StringBuilder sbOut = new StringBuilder();
                
                foreach (string line in inSrc)
                {
                    sbOut.AppendLine(Parsy.ConvertToCSharp(line));
                }

                sbOut.AppendLine("\n\t}\n}");

                File.WriteAllText(_out, sbOut.ToString());
            }
            else
            {
                throw new FileNotFoundException("Input source file '" + _in + "' does not exist...");
            }
        }
    }
}
