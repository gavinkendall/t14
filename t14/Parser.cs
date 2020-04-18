//-----------------------------------------------------------------------
// <copyright file="Parser.cs" company="Gavin Kendall">
//     Copyright (c) Gavin Kendall. All rights reserved.
// </copyright>
// <author>Gavin Kendall</author>
// <summary>A parser is responsible for parsing a T14 script.</summary>
//-----------------------------------------------------------------------
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace t14
{
    /// <summary>
    /// A parser is responsible for parsing a T14 script.
    /// </summary>
    public class Parser
    {
        private readonly Regex rgxVariable = new Regex("^::set (?<VariableName>\\$[a-zA-Z_-]+) = (?<VariableValue>.+)$");
        private readonly Regex rgxWTF = new Regex("^::wtf\\((?<Value>.+)\\)$");

        // Conversion methods.
        private readonly Regex rgxHexToBin = new Regex("^::hex->bin\\((?<Value>.+)\\)$");
        private readonly Regex rgxHexToDec = new Regex("^::hex->dec\\((?<Value>.+)\\)$");
        private readonly Regex rgxHexToASCII = new Regex("^::hex->ascii\\((?<Value>.+)\\)$");
        private readonly Regex rgxBinToDec = new Regex("^::bin->dec\\((?<Value>.+)\\)$");
        private readonly Regex rgxBinToHex = new Regex("^::bin->hex\\((?<Value>.+)\\)$");
        private readonly Regex rgxBinToASCII = new Regex("^::bin->ascii\\((?<Value>.+)\\)$");
        private readonly Regex rgxDecToHex = new Regex("^::dec->hex\\((?<Value>.+)\\)$");
        private readonly Regex rgxDecToBin = new Regex("^::dec->bin\\((?<Value>.+)\\)$");
        private readonly Regex rgxDecToASCII = new Regex("^::dec->ascii\\((?<Value>.+)\\)$");
        private readonly Regex rgxASCIIToBin = new Regex("^::ascii->bin\\((?<Value>.+)\\)$");
        private readonly Regex rgxASCIIToHex = new Regex("^::ascii->hex\\((?<Value>.+)\\)$");
        private readonly Regex rgxASCIIToDec = new Regex("^::ascii->dec\\((?<Value>.+)\\)$");

        /// <summary>
        /// Parser constructor.
        /// </summary>
        public Parser()
        {
            
        }

        /// <summary>
        /// Parser constructor accepting the filename of a T14 script.
        /// </summary>
        /// <param name="filename">The filename of a T14 script.</param>
        public void Parse(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"Error: File named {filename} could not be found.");
                return;
            }

            if (!filename.EndsWith(".t14", StringComparison.CurrentCulture))
            {
                Console.WriteLine($"Error: File named {filename} does not end with extension \".t14\".");
                return;
            }

            VariableCollection variables = new VariableCollection();

            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                // Ignore any line that starts with # because it should be interpreted as a comment.
                if (line.StartsWith("#", StringComparison.CurrentCulture))
                    continue;

                // Any line that starts with :: should be interpreted as a command.
                if (line.StartsWith("::", StringComparison.CurrentCulture))
                {
                    if (rgxVariable.IsMatch(line))
                    {
                        Variable variable = new Variable()
                        {
                            Name = rgxVariable.Match(line).Groups["VariableName"].Value,
                            Value = rgxVariable.Match(line).Groups["VariableValue"].Value
                        };

                        variables.Add(variable);
                    }

                    if (rgxWTF.IsMatch(line))
                    {
                        Convert.WhatTheFuckIsThis(rgxWTF.Match(line).Groups["Value"].Value);
                    }

                    ParseConversionMethods(line, true);

                    continue;
                }

                // Split each line by space into an array of words.
                string[] words = line.Split(' ');

                for (int i = 0; i < words.Length; i++)
                {
                    string word = words[i];

                    if (string.IsNullOrEmpty(word) || string.IsNullOrWhiteSpace(word))
                        continue;

                    // This word is a variable.
                    if (word.StartsWith("$", StringComparison.CurrentCulture))
                    {
                        // Output the value of the variable if we find it in the variale collection using the variable name.
                        Variable variable = variables.GetByName(word);

                        if (variable != null)
                        {
                            Console.Write(variable.Value);
                        }
                    }
                    else if (word.StartsWith("::", StringComparison.CurrentCulture))
                    {
                        ParseConversionMethods(word, false);
                    }
                    else
                    {
                        // Output any word in a line that appears in the file as standard output if it's not a comment, command, or variable.
                        Console.Write(word);
                    }

                    // Assume there's a space between each word and output words until the end of the line without a trailing space.
                    if (i < (words.Length - 1))
                        Console.Write(" ");

                    // Output with EOL if we've reached the end of the line.
                    if (i == (words.Length - 1))
                        Console.WriteLine();
                }
            }
        }

        private void ParseConversionMethods(string value, bool newline)
        {
            if (rgxHexToBin.IsMatch(value))
            {
                Console.Write(Convert.FromHexToBinary(rgxHexToBin.Match(value).Groups["Value"].Value));
            }

            if (rgxHexToDec.IsMatch(value))
            {
                Console.Write(Convert.FromHexToDecimal(rgxHexToDec.Match(value).Groups["Value"].Value));
            }

            if (rgxHexToASCII.IsMatch(value))
            {
                Console.Write(Convert.FromHexToASCII(rgxHexToASCII.Match(value).Groups["Value"].Value));
            }

            if(rgxBinToDec.IsMatch(value))
            {
                Console.Write(Convert.FromBinaryToDecimal(rgxBinToDec.Match(value).Groups["Value"].Value));
            }

            if (rgxBinToHex.IsMatch(value))
            {
                Console.Write(Convert.FromBinaryToHex(rgxBinToHex.Match(value).Groups["Value"].Value));
            }

            if (rgxBinToASCII.IsMatch(value))
            {
                Console.Write(Convert.FromBinaryToASCII(rgxBinToASCII.Match(value).Groups["Value"].Value));
            }

            if (rgxDecToHex.IsMatch(value))
            {
                Console.Write(Convert.FromDecimalToHex(rgxDecToHex.Match(value).Groups["Value"].Value));
            }

            if (rgxDecToBin.IsMatch(value))
            {
                Console.Write(Convert.FromDecimalToBinary(rgxDecToBin.Match(value).Groups["Value"].Value));
            }

            if(rgxDecToASCII.IsMatch(value))
            {
                Console.Write(Convert.FromDecimalToASCII(rgxDecToASCII.Match(value).Groups["Value"].Value));
            }

            if(rgxASCIIToBin.IsMatch(value))
            {
                Console.Write(Convert.FromASCIIToBinary(rgxASCIIToBin.Match(value).Groups["Value"].Value));
            }

            if (rgxASCIIToHex.IsMatch(value))
            {
                Console.Write(Convert.FromASCIIToHex(rgxASCIIToHex.Match(value).Groups["Value"].Value));
            }

            if (rgxASCIIToDec.IsMatch(value))
            {
                Console.Write(Convert.FromASCIIToDecimal(rgxASCIIToDec.Match(value).Groups["Value"].Value));
            }

            if (newline)
                Console.WriteLine();
        }
    }
}
