//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Gavin Kendall">
//     Copyright (c) Gavin Kendall. All rights reserved.
// </copyright>
// <author>Gavin Kendall</author>
// <summary>The main entry point for the T14 language interpreter.</summary>
//-----------------------------------------------------------------------
using System;

namespace t14
{
    /// <summary>
    /// The main class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">You need to specify the first argument as the filename of a T14 script.</param>
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("T14 Interpreted Scripting Language (v1.0)");
                Console.WriteLine("Copyright (C) 2020 Gavin Kendall");
                Console.WriteLine("\nThis program is a language interpreter which parses a T14 script and performs various functions based on the T14 scripting language written in a T14 script. The source code is available at https://github.com/gavinkendall/t14 for your review.");
                Console.WriteLine("\nThis program comes with ABSOLUTELY NO WARRANTY. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. THE PROGRAM IS PROVIDED \"AS IS\" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.");
                Console.WriteLine("\nRun \"t14.exe help\" (or \"dotnet t14.dll help\" if you're using a Mac or running on Linux with the Microsoft .NET Core framework) for help in using the T14 language interpreter.");

                return;
            }

            if (args.Length == 1 && !string.IsNullOrEmpty(args[0]) && args[0].Equals("help"))
            {
                Console.WriteLine(":: Help ::");
                Console.WriteLine("You would normally run the T14 interpreter with a given filename that ends with a \".t14\" file extension as the first command line argument for the interpreter. For example, on Windows, you would run \"t14.exe hello.t14\" and on a Mac or Linux system with Microsoft .NET Core you would run \"dotnet t14.dll hello.t14\" to interpret, and parse, the T14 scripting language written in the provided T14 script.");
                Console.WriteLine();
                Console.WriteLine(":: Conversion Methods ::");
                Console.WriteLine("Here is a list of methods you can use in your T14 script to convert stuff.");
                Console.WriteLine("::dec->bin(65)");
                Console.WriteLine("::dec->hex(65)");
                Console.WriteLine("::dec->ascii(65)");
                Console.WriteLine("::bin->dec(01000001)");
                Console.WriteLine("::bin->hex(01000001)");
                Console.WriteLine("::bin->ascii(01000001)");
                Console.WriteLine("::hex->bin(41)");
                Console.WriteLine("::hex->dec(41)");
                Console.WriteLine("::hex->ascii(41)");
                Console.WriteLine("::ascii->bin(A)");
                Console.WriteLine("::ascii->hex(A)");
                Console.WriteLine("::ascii->dec(A)");
                Console.WriteLine("::text->morse(hello world) or ::morse(hello world)");
                Console.WriteLine("::morse->text(.... . .-.. .-..---/.---- - .-. .- ..-..)");
            }
            else if (args.Length == 1 && !string.IsNullOrEmpty(args[0]))
            {
                Parser parser = new Parser();
                parser.Parse(args[0]);
            }
        }
    }
}
