//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Gavin Kendall">
//     Copyright (c) 2020-2021 Gavin Kendall
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
                Console.WriteLine("T14 Interpreted Scripting Language (v1.0.0.2)");
                Console.WriteLine("Copyright (C) 2021 Gavin Kendall");
                Console.WriteLine("\nThis program is a language interpreter which parses a T14 script and performs various functions based on the T14 scripting language written in a T14 script. The source code is available at https://github.com/gavinkendall/t14 for your review.");
                Console.WriteLine("\nThis program comes with ABSOLUTELY NO WARRANTY. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. THE PROGRAM IS PROVIDED \"AS IS\" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.");
                Console.WriteLine("\nPlease read the readme.txt file for help with using the T14 language interpreter.");

                return;
            }
            else if (args.Length == 1 && !string.IsNullOrEmpty(args[0]))
            {
                Parser parser = new Parser();

                if (args[0].StartsWith("\"", StringComparison.CurrentCulture) && args[0].EndsWith("\"", StringComparison.CurrentCulture))
                {
                    args[0] = args[0].TrimStart('"');
                    args[0] = args[0].TrimEnd('"');
                }

                if (args[0].EndsWith(".t14", StringComparison.CurrentCulture))
                {
                    parser.ParseScript(args[0]);
                }
                else if (args[0].StartsWith("::", StringComparison.CurrentCulture))
                {
                    parser.ParseConversionMethod(args[0]);
                }
            }
        }
    }
}
