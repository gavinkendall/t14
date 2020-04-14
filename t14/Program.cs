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
                Console.WriteLine("Error: No filename specified. You should be running t14.exe with a filename as the first argument. Try again.");
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
