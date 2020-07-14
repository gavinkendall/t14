//-----------------------------------------------------------------------
// <copyright file="Parser.cs" company="Gavin Kendall">
//     Copyright (c) Gavin Kendall. All rights reserved.
// </copyright>
// <author>Gavin Kendall</author>
// <summary>A parser responsible for parsing a T14 script.</summary>
//-----------------------------------------------------------------------
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace t14
{
    /// <summary>
    /// A parser responsible for parsing a T14 script.
    /// </summary>
    public class Parser
    {
        private bool _scriptInitialized;

        // Exit commands (including FTS = Fuck This Shit and FI = Fuck It).
        private readonly Regex rgxExit = new Regex("^::exit$|^::quit$|^::fts$|^::fi$");

        // Start and End commands.
        private readonly Regex rgxStart = new Regex("^::start$");
        private readonly Regex rgxEnd = new Regex("^::end$");

        // If command.
        private readonly Regex rgxIf = new Regex("^::if \\((?<LeftValue>) (?<Operator>) (?<RightValue>)\\)$");

        // Variables.
        private readonly VariableCollection variables;
        private readonly Regex rgxVariable = new Regex("^::set (?<VariableName>\\$[a-zA-Z_-]+) = (?<VariableValue>.+)$");

        // The moment where you're looking at something and just go, "What the fuck is this?!".
        // So just pass in the unknown value and let the wtf command do the rest.
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
        private readonly Regex rgxTextToMorse = new Regex("^::morse\\((?<Value>.+)\\)$|^::text->morse\\((?<Value>.+)\\)$");
        private readonly Regex rgxMorseToText = new Regex("^::morse->text\\((?<Value>.+)\\)$");

        /// <summary>
        /// Parser constructor.
        /// </summary>
        public Parser()
        {
            _scriptInitialized = false;
            variables = new VariableCollection();
        }

        /// <summary>
        /// Parses a T14 script given its filename.
        /// </summary>
        /// <param name="filename">The filename of a T14 script.</param>
        public void ParseScript(string filename)
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

            string[] lines = File.ReadAllLines(filename);

            ParseScriptLines(lines, 0, lines.Length);
        }

        /// <summary>
        /// Parses the given lines looking for T14 variables and commands.
        /// </summary>
        /// <param name="lines">The lines to parse.</param>
        /// <param name="start">The starting index.</param>
        /// <param name="end">The ending index.</param>
        public void ParseScriptLines(string[] lines, int start, int end)
        {
            for (int lineIndex = start; lineIndex < end; lineIndex++)
            {
                string line = lines[lineIndex];

                // Ignore any line that's just whitespace or starts with # because it should be interpreted as a comment.
                if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line) || line.StartsWith("#", StringComparison.CurrentCulture))
                {
                    continue;
                }

                // Any line that starts with :: should be interpreted as a command.
                if (line.StartsWith("::", StringComparison.CurrentCulture))
                {
                    if (rgxStart.IsMatch(line))
                    {
                        _scriptInitialized = true;
                        ParseScriptLines(lines, (lineIndex + 1), end);
                        continue;
                    }

                    if (_scriptInitialized)
                    {
                        if (rgxExit.IsMatch(line))
                        {
                            Environment.Exit(0);
                        }

                        if (rgxEnd.IsMatch(line))
                        {
                            _scriptInitialized = false;
                            break;
                        }

                        ParseCommand(line);
                    }
                }
                else
                {
                    if (_scriptInitialized)
                    {
                        // Split each line by space into an array of words.
                        string[] words = line.Split(' ');

                        for (int i = 0; i < words.Length; i++)
                        {
                            string word = words[i];

                            if (string.IsNullOrEmpty(word) || string.IsNullOrWhiteSpace(word))
                            {
                                continue;
                            }

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
                            {
                                Console.Write(" ");
                            }

                            // Output with EOL if we've reached the end of the line.
                            if (i == (words.Length - 1))
                            {
                                Console.WriteLine();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parses a T14 command.
        /// </summary>
        /// <param name="command">The command to parse.</param>
        public void ParseCommand(string command)
        {
            if (rgxVariable.IsMatch(command))
            {
                Variable variable = new Variable()
                {
                    Name = rgxVariable.Match(command).Groups["VariableName"].Value,
                    Value = rgxVariable.Match(command).Groups["VariableValue"].Value.Trim()
                };

                variables.Add(variable);
            }

            if (rgxWTF.IsMatch(command))
            {
                Convert.WhatTheFuckIsThis(rgxWTF.Match(command).Groups["Value"].Value);
            }

            ParseConversionMethods(command, true);
        }

        private void ParseConversionMethods(string value, bool newline)
        {
            bool match = false;

            if (rgxHexToBin.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromHexToBinary(rgxHexToBin.Match(value).Groups["Value"].Value));
            }

            if (rgxHexToDec.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromHexToDecimal(rgxHexToDec.Match(value).Groups["Value"].Value));
            }

            if (rgxHexToASCII.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromHexToASCII(rgxHexToASCII.Match(value).Groups["Value"].Value));
            }

            if(rgxBinToDec.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromBinaryToDecimal(rgxBinToDec.Match(value).Groups["Value"].Value));
            }

            if (rgxBinToHex.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromBinaryToHex(rgxBinToHex.Match(value).Groups["Value"].Value));
            }

            if (rgxBinToASCII.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromBinaryToASCII(rgxBinToASCII.Match(value).Groups["Value"].Value));
            }

            if (rgxDecToHex.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromDecimalToHex(rgxDecToHex.Match(value).Groups["Value"].Value));
            }

            if (rgxDecToBin.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromDecimalToBinary(rgxDecToBin.Match(value).Groups["Value"].Value));
            }

            if(rgxDecToASCII.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromDecimalToASCII(rgxDecToASCII.Match(value).Groups["Value"].Value));
            }

            if(rgxASCIIToBin.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromASCIIToBinary(rgxASCIIToBin.Match(value).Groups["Value"].Value));
            }

            if (rgxASCIIToHex.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromASCIIToHex(rgxASCIIToHex.Match(value).Groups["Value"].Value));
            }

            if (rgxASCIIToDec.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromASCIIToDecimal(rgxASCIIToDec.Match(value).Groups["Value"].Value));
            }

            if (rgxTextToMorse.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromTextToMorse(rgxTextToMorse.Match(value).Groups["Value"].Value));
            }

            if (rgxMorseToText.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromMorseToText(rgxMorseToText.Match(value).Groups["Value"].Value));
            }

            if (match && newline)
            {
                Console.WriteLine();
            }
        }
    }
}
