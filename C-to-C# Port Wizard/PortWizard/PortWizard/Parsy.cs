///
///	\file Parsy.cs
///	Contains the class definition for the Parsy Class.
///
///	\author	Dan Hieronimus, Ryan Pink
///



using System.Text;
using System.Text.RegularExpressions;


namespace PortWizard
{
	/// \class 	Parsy
	/// \brief	Represents a basic Porting wizard for porting C code into C# code.
	///
	/// \detailed
	/// The Parsy class handles all string-juggling while converting individual lines
	/// from C to C#. It calls its own helper methods internally to process each unique
	/// code component.
	///
    static class Parsy
    {
        static bool CommentFlag = false;    ///<	True when currently reading a multiline comment block
        static int TabLevel = 0;            ///<	Tracks the current indenting of the file



		///
		/// \brief <b>Brief Description:</b> Parses and converts an individual line of C code into C# code.
		/// \details <b>Detailed Description:</b>
		/// This method acts as the black-box interface between the calling function and the Parsy static class. It accepts
        /// a string of raw C code and calls private methods internally to process it. The calling function is then
        /// returned a fully-processed C# string.
		///
		/// \params[in] _lineIn <b> string </b> The line of C code to be parsed and converted.
		///
		/// \returns lineOut <b> static string </b> A line of C# code equivalent to the C code passed in as a parameter.
		///
        internal static string ConvertToCSharp(string _lineIn)
        {
            // Bootstrapping method that acts as the interface to Parsy.
            // Calls internal methods to process.
            string lineOut = _lineIn;
            CodeComponent cc = CodeComponent.ConsoleWrite;

            while (cc != CodeComponent.Verbatim)
            {
                cc = GetCodeComponent(lineOut);

                if (Regex.IsMatch(lineOut, @"/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*") && !CommentFlag)
                {
                    CommentFlag = true;
                    cc = CodeComponent.Verbatim;
                }

                if (Regex.IsMatch(lineOut, @"([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/") && CommentFlag) 
                {
                    CommentFlag = false;
                    cc = CodeComponent.Verbatim;
                }

                if (CommentFlag)
                {
                    cc = CodeComponent.Verbatim;
                }
                
                switch (cc)
                {
                    case CodeComponent.Array:
                        lineOut = ParseArray(lineOut);
                        break;
                    case CodeComponent.ConsoleRead:
                        lineOut = ParseConsoleRead(lineOut);
                        break;
                    case CodeComponent.ConsoleWrite:
                        // Do the appropriate replacements
                        lineOut = ParseConsoleWrite(lineOut);
                        break;
                    case CodeComponent.File:
                        lineOut = ParseFileDecl(lineOut);
                        break;
                    case CodeComponent.FileClose:
                        lineOut = ParseFileClose(lineOut);
                        break;
                    case CodeComponent.FileOpen:
                        lineOut = ParseFileOpen(lineOut);
                        break;
                    case CodeComponent.FileRead:
                        lineOut = ParseFileRead(lineOut);
                        break;
                    case CodeComponent.FileWrite:
                        lineOut = ParseFileWrite(lineOut);
                        break;
                    case CodeComponent.Library:
                        lineOut = ParseLibrary(lineOut);
                        break;
                    case CodeComponent.Main:
                        lineOut = ParseMain(lineOut);
                        TabLevel++;
                        break;
                    case CodeComponent.StrLength:
                        lineOut = ParseStringLength(lineOut);
                        break;
                    case CodeComponent.StrToInt:
                        lineOut = ParseStringToInt(lineOut);
                        break;
                    default:
                        lineOut = SetTabLevel(lineOut);
                        break;
                }
            }

            lineOut = Regex.Replace(lineOut, @"NULL", "null");
            lineOut = Regex.Replace(lineOut, @"return\s+\w+", "return");

            return lineOut;   // Return replacement
        }


        ///
        /// \brief <b>Brief Description:</b> Indents the converted C# code to conform with SET standards
        /// \details <b>Detailed Description:</b>
        /// This method accepts a fully converted line of C# code and applies the correct (?) tabbed indenting
        /// onto the beginning. It then returns the final string.
        ///
        /// \params[in] lineOut <b> string </b> The line of C# code to be tabbed in.
        ///
        /// \returns sb.ToString() <b> string </b> A masterpiece worthy of the Queen herself.
        ///
        private static string SetTabLevel(string lineOut)
        {
            if (lineOut.Contains("}") && !CommentFlag)
            {
                TabLevel--;
            } 
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < TabLevel; ++i)
            {
                if (!lineOut.Contains("namespace"))
                {
                    sb.Append("\t");
                }
            }

            if (lineOut.Contains("{") && !CommentFlag)
            {
                TabLevel++;
            }

            sb.Append(lineOut.TrimStart());

            return sb.ToString();
        }


        ///
        /// \brief <b>Brief Description:</b> Analyses the C code and determines what method to convert it
        /// \details <b>Detailed Description:</b>
        /// This method scans the raw C code and determines what C-specific command is present. It then
        /// returns an enumerated type representing that component.
        ///
        /// \params[in] _lineIn <b> string </b> The line of C code to be tabbed analysed.
        ///
        /// \returns cc <b> CodeComponent </b> CodeComponent enumerated type.
        ///
        private static CodeComponent GetCodeComponent(string _lineIn)
        {
            CodeComponent cc = CodeComponent.Verbatim;
            
            // REGEXY THINGS    char asdf[20]
            if (Regex.IsMatch(_lineIn, @"^\s*\w+\s+\w+(\[\s*\d*\s*\])"))
            {
                cc = CodeComponent.Array;
            }
            else if (Regex.IsMatch(_lineIn, @"^\s*gets\s*\(\s*[^\s].*\s*\)"))
            {
                cc = CodeComponent.ConsoleRead;
            }
            else if (Regex.IsMatch(_lineIn, @"^\s*printf\s*\(\s*"".*""\s*(,\s*.*\s*)*\s*\)") )
            {
                cc = CodeComponent.ConsoleWrite;
            }
            else if (Regex.IsMatch(_lineIn, @"FILE\s*\*\s*([^;])*"))
            {
                cc = CodeComponent.File;
            }
            else if (Regex.IsMatch(_lineIn, @"fclose\s*\(\s*([^\s\(\);])*\s*\)"))
            {
                cc = CodeComponent.FileClose;
            }
            else if (Regex.IsMatch(_lineIn, @"fopen\s*\(\s*[^\s].*\s*,\s*[^\s].*\s*\)"))
            {
                cc = CodeComponent.FileOpen;
            }
            else if (Regex.IsMatch(_lineIn, @"fgets\s*\(\s*[^\s].*\s*,\s*[^\s].*\s*,\s*[^\s].*\s*\)"))
            {
                cc = CodeComponent.FileRead;
            }
            else if (Regex.IsMatch(_lineIn, @"fprintf\s*\(\s*[^\s].*\s*,\s*[^\s].*\s*\)"))
            {
                cc = CodeComponent.FileWrite;
            }
            else if (Regex.IsMatch(_lineIn, @"#include\s*\<\s*([^\s\(\);])*\s*\>"))
            {
                cc = CodeComponent.Library;
            }
            else if (Regex.IsMatch(_lineIn, @"int\s*main\s*\(\s*void\s*\)"))    
            {
                cc = CodeComponent.Main;
            }
            else if (Regex.IsMatch(_lineIn, @"strlen\s*\(\s*([^\s\(\);])*\s*\)"))
            {
                cc = CodeComponent.StrLength;
            }
            else if (Regex.IsMatch(_lineIn, @"atoi\s*\(\s*([^\s\(\);])*\s*\)"))
            {
                cc = CodeComponent.StrToInt;
            }

            return cc;
        }


        ///
        /// \brief <b>Brief Description:</b> Converts array declarations to C# syntax
        /// \details <b>Detailed Description:</b>
        /// This method converts a line of C code representing an array declaration, and converts it into
        /// an array declaration using C# syntax. It also instantiates the array with a 'new';
        ///
        /// \params[in] _lineIn <b> string </b> The line of C# code to be translated.
        ///
        /// \returns ret <b> string </b> C#-equivalent array declaration string
        ///
        private static string ParseArray(string _lineIn)
        {
            string dataType = _lineIn.Trim().Split(' ','\t')[0];
            string arraySize = _lineIn.Split('[',']')[1];
            string name = _lineIn.Trim().Split(' ', '\t','[')[1];

            string ret = dataType + "[]" + " " + name + " = new " + dataType + "[" + arraySize + "];";
            return ret;
        }


        ///
        /// \brief <b>Brief Description:</b> Converts C's atoi() function to C#'s Convert.ToInt32()
        /// \details <b>Detailed Description:</b>
        /// This method translates the string-to-int function atoi() into the C#-equivalent Convert.ToInt32().
        ///
        /// \params[in] _lineIn <b> string </b> The line of C# code to be translated.
        ///
        /// \returns _lineIn <b> string </b> C#-equivalent translation
        ///
        private static string ParseStringToInt(string _lineIn)
        {
            _lineIn = Regex.Replace(_lineIn, @"atoi", "Convert.ToInt32(new string");
            _lineIn = _lineIn.Replace(");", "));");
            return _lineIn;
        }


        ///
        /// \brief <b>Brief Description:</b> Converts C's strlen() function to C#'s str.Length
        /// \details <b>Detailed Description:</b>
        /// This method translates the string length function strlen() to C#-equivalent str.Length
        ///
        /// \params[in] _lineIn <b> string </b> The line of C# code to be translated.
        ///
        /// \returns sb.ToString() <b> string </b> C#-equivalent translation
        ///
        private static string ParseStringLength(string _lineIn)
        {
            Regex rg = new Regex(@"strlen\s*\(\s*([^\s\(\);])*\s*\)");
            string[] parts = rg.Split(_lineIn);
            string[] bits = { parts[0], parts[2] };
            string replace = _lineIn.Split(bits, System.StringSplitOptions.RemoveEmptyEntries)[0];
            replace = replace.Split('(', ')')[1];

            StringBuilder sb = new StringBuilder();
            sb.Append(parts[0] + replace + ".Length" + parts[2]);
            
            return sb.ToString();
        }


        ///
        /// \brief <b>Brief Description:</b> Converts C's main function header to C#'s static void main
        /// \details <b>Detailed Description:</b>
        /// This method translates the main function header "int void main( void )" to C#-equivalent
        /// "static void Main()". It also inserts the Program class and a default namespace.
        ///
        /// \params[in] _lineIn <b> string </b> The line of C# code to be translated.
        ///
        /// \returns sb.ToString() <b> string </b> C#-equivalent translation
        ///
        private static string ParseMain(string _lineIn)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("namespace Something\n{\n\tclass Program\n\t{\n\t\tstatic void Main()");

            return sb.ToString();
        }


        ///
        /// \brief <b>Brief Description:</b> Converts C's #include to C#'s using library declarations
        /// \details <b>Detailed Description:</b>
        /// This method translates the library #include C statements into C#'s using namespace statements.
        ///
        /// \params[in] _lineIn <b> string </b> The line of C# code to be translated.
        ///
        /// \returns sb.ToString() <b> string </b> C#-equivalent translation
        ///
        private static string ParseLibrary(string _lineIn)
        {
            char[] delimiters = { '<', '>' };
            string[] parts = _lineIn.Split(delimiters);

            StringBuilder sb = new StringBuilder();
            sb.Append("using ");

            if (parts[1].Equals("stdio.h"))
            {
                sb.Append("System;\nusing System.IO;\nusing System.Text;");
            }
            else
            {
                sb.Clear();
            }
            
            return sb.ToString();
        }


        ///
        /// \brief <b>Brief Description:</b> Converts C's fprintf() function to C#'s FileStream.Write() method
        /// \details <b>Detailed Description:</b>
        /// This method translates the file print function fprintf() into C#-equivalent FileStream.Write() and
        /// performs the necessary conversions and formatting.
        ///
        /// \params[in] _lineIn <b> string </b> The line of C# code to be translated.
        ///
        /// \returns sb.ToString() <b> string </b> C#-equivalent translation
        ///
        private static string ParseFileWrite(string _lineIn)
        {
            StringBuilder sb = new StringBuilder();

            char[] delimiters = { '(', ')', ',' };
            string[] parts = _lineIn.Split(delimiters);

            sb.Append(parts[1] + ".Write(Encoding.ASCII.GetBytes(" + parts[2] + "), 0, ");

            if (parts[2].Contains("\""))
            {
                int len = parts[2].Trim().Replace("\\","").Length - 2;
                sb.Append(len + ");");
            }
            else
            {
                sb.Append(parts[2].Trim() + ".Length);");
            }
            //}

            return sb.ToString();
        }



		///
		/// \brief <b>Brief Description:</b> Converts fgets() to File.Read(Encoding.ASCII.GetBYtes, ...)
		/// \details <b>Detailed Description:</b>
		/// Parses and converts fgets() to File.Read(Encoding.ASCII.GetBYtes, ...) for reading the contents of a file.
		///
		/// \params[in] _lineIn <b> string </b> The line of C code containing the fgets() call
		///
		/// \returns <b> static string </b> The C# equivalent represented as File.Read(Encoding.ASCII.GetBYtes, ...)
		///
        private static string ParseFileRead(string _lineIn)
        {
            //Regex rg = new Regex(@"fgets\s*\(\s*[^\s][^\)]*\s*,\s*[^\s].*\s*,\s*[^\s].*\s*\)");
            Regex rg = new Regex(@"(fgets\s*\(\s*[^\)]*\))+");
            string[] parts = rg.Split(_lineIn);
            string[] bits = { parts[0], parts[2] };
            string replace = _lineIn.Split(bits, System.StringSplitOptions.RemoveEmptyEntries)[0];
            string buf = replace.Split('(', ')', ',')[1];
            string size = replace.Split('(', ')', ',')[2];
            string fp = replace.Split('(', ')', ',')[3];

            parts[2] = parts[2].Replace("NULL", "0");

            StringBuilder sb = new StringBuilder();
            sb.Append(parts[0] + fp + ".Read(Encoding.ASCII.GetBytes(" + buf + ", 0, " + buf + ".Length), 0, " + size + ")" + parts[2]);

            return sb.ToString();
        }



		///
		/// \brief <b>Brief Description:</b> Converts fopen() to File.Open()
		/// \details <b>Detailed Description:</b>
		/// Parses and converts fopen() to File.Open() for opening a file handle
		///
		/// \params[in] _lineIn <b> string </b> The line of C code containing the fopen() call
		///
		/// \returns  _lineIn <b> static string </b> The C# equivalent represented as File.Open()
		///
        private static string ParseFileOpen(string _lineIn)
        {
            _lineIn = Regex.Replace(_lineIn, @"fopen", "File.Open");
            _lineIn = Regex.Replace(_lineIn, @", ", ", FileMode.OpenOrCreate, FileAccess.");
            _lineIn = Regex.Replace(_lineIn, @"\""r\""", "Read");
            _lineIn = Regex.Replace(_lineIn, @"\""w\""", "Write");

            return _lineIn;
        }



		///
		/// \brief <b>Brief Description:</b> Converts fclose() to File.Close
		/// \details <b>Detailed Description:</b>
		/// Parses and converts fclose() to File.Close for closing a file handle
		///
		/// \params[in] _lineIn <b> string </b> The line of C code containing the fclose() call
		///
		/// \returns  <b> static string </b> The C# equivalent represented as File.Close
		///
        private static string ParseFileClose(string _lineIn)
        {
            char[] delimiters = { '(', ')' };
            string[] parts = _lineIn.Split(delimiters);

            StringBuilder sb = new StringBuilder();
            sb.Append(parts[1]);
            sb.Append(".Close();");

            return sb.ToString();
        }



		///
		/// \brief <b>Brief Description:</b> Converts FILE * to FileStream
		/// \details <b>Detailed Description:</b>
		/// Parses and converts FILE * to FileStream which is saying a file pointer is equivalent to a
		/// FileStream reference
		///
		/// \params[in,out] _lineIn <b> string </b> The line of C code containing the pritf("\n") call.
		///
		/// \returns _lineIn <b> static string </b> The C# equivalent represented as FileStream.
		///
        private static string ParseFileDecl(string _lineIn)
        {
            _lineIn = Regex.Replace(_lineIn, @"FILE\s*\*\s*", "FileStream ");
            //_lineIn = Regex.Replace(_lineIn, @";", " = new FileStream();");
            
            return _lineIn;
        }



		///
		/// \brief <b>Brief Description:</b> Converts printf("\n") to Console.WriteLine("")
		/// \details <b>Detailed Description:</b>
		/// Parses and converts printf("\n") to Console.WriteLine("") for writing a line of text to the console
		/// with a trailing newline.
		///
		/// \params[in,out] _lineIn <b> string </b> The line of C code containing the pritf("\n") call.
		///
		/// \returns _lineIn <b> static string </b> The C# equivalent represented as Console.WriteLine("").
		///
        private static string ParseConsoleWrite(string _lineIn)
        {
            if (_lineIn.Contains("\\n\");") || _lineIn.Contains("\\n\", "))
            {
                _lineIn = Regex.Replace(_lineIn, @"printf", "Console.WriteLine");
                _lineIn = Regex.Replace(_lineIn, @"\\n", "");
            }
            else
            {
                _lineIn = Regex.Replace(_lineIn, @"printf", "Console.Write");
            }

            bool keepGoing = true;
            int paramCtr = 0;

            do
            {
                if (Regex.IsMatch(_lineIn, @"\%\d{0,1}[cdfsx]"))
                {
                    Regex r = new Regex(@"\%\d{0,1}[cdfsx]");
                    _lineIn = r.Replace(_lineIn, "{" + paramCtr++ + "}", 1);
                }
                else
                {
                    keepGoing = false;
                    break;
                }
            } while (keepGoing);

            return _lineIn;
        }



		///
		/// \brief <b>Brief Description:</b> Converts gets() to Console.readline().ToCharArray()
		/// \details <b>Detailed Description:</b>
		/// Parses and converts gets() to Console.readline().ToCharArray() to simulate reading in characters into a char
		/// array until a newline or EOF is found.
		///
		/// \params[in] _lineIn <b> string </b> The line of C code containing the gets() call.
		///
		/// \returns <b> static string </b> The C# equivalent represented as Console.readline().ToCharArray().
		///
        private static string ParseConsoleRead(string _lineIn)
        {
            Regex rg = new Regex(@"(gets\s*\(\s*[^\)]*\))+");
            string[] parts = rg.Split(_lineIn);
            string[] bits = { parts[0], parts[2] };
            string replace = _lineIn.Split(bits, System.StringSplitOptions.RemoveEmptyEntries)[0];
            string buf = replace.Split('(', ')')[1];

            StringBuilder sb = new StringBuilder();
            sb.Append(parts[0] + buf + " = Console.ReadLine().ToCharArray()" + parts[2]);

            return sb.ToString();
        }
    }
}
