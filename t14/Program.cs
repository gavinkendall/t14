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
                Console.WriteLine(":: Help ::");
                Console.WriteLine("You would normally run t14.exe (or dotnet t14.dll) with a filename that ends with a \".t14\" file extension. For example, on Windows, you would run ...");
                Console.WriteLine("t14.exe hello.t14");
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

                return;
            }

            if (args.Length == 1 && !string.IsNullOrEmpty(args[0]))
            {
                Parser parser = new Parser();
                parser.Parse(args[0]);
            }
        }
    }
}
