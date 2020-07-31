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
        private readonly Regex _rgxExit = new Regex("^::exit$|^::quit$|^::fts$|^::fi$");

        // Start and End commands for blocks of code.
        private readonly BlockCollection _blocks;
        private readonly Regex _rgxStart = new Regex("^::start\\((?<BlockName>[0-9a-zA-Z_-]+)\\)$");
        private readonly Regex _rgxEnd = new Regex("^::end$");

        // Run command.
        private readonly Regex _rgxRun = new Regex("^::run\\((?<BlockName>[0-9a-zA-Z_-]+)\\)$");

        // If command.
        private readonly Regex _rgxIf = new Regex("^::if \\((?<LeftValue>) (?<Operator>) (?<RightValue>)\\)->\\((?<BlockName>[0-9a-zA-Z_-]+)\\)$");

        // Variables.
        private readonly VariableCollection variables;
        private readonly Regex _rgxVariable = new Regex("^::set (?<VariableName>\\$[0-9a-zA-Z_-]+) = (?<VariableValue>.+)$");

        // The moment where you're looking at something and just go, "What the fuck is this?!".
        // So just pass in the unknown value and let the wtf command do the rest.
        private readonly Regex _rgxWTF = new Regex("^::wtf\\((?<Value>.+)\\)$");

        // Conversion methods.
        private readonly Regex _rgxHexToBin = new Regex("^::hex->bin\\((?<Value>.+)\\)$");
        private readonly Regex _rgxHexToDec = new Regex("^::hex->dec\\((?<Value>.+)\\)$");
        private readonly Regex _rgxHexToASCII = new Regex("^::hex->ascii\\((?<Value>.+)\\)$");
        private readonly Regex _rgxBinToDec = new Regex("^::bin->dec\\((?<Value>.+)\\)$");
        private readonly Regex _rgxBinToHex = new Regex("^::bin->hex\\((?<Value>.+)\\)$");
        private readonly Regex _rgxBinToASCII = new Regex("^::bin->ascii\\((?<Value>.+)\\)$");
        private readonly Regex _rgxDecToHex = new Regex("^::dec->hex\\((?<Value>.+)\\)$");
        private readonly Regex _rgxDecToBin = new Regex("^::dec->bin\\((?<Value>.+)\\)$");
        private readonly Regex _rgxDecToASCII = new Regex("^::dec->ascii\\((?<Value>.+)\\)$");
        private readonly Regex _rgxASCIIToBin = new Regex("^::ascii->bin\\((?<Value>.+)\\)$");
        private readonly Regex _rgxASCIIToHex = new Regex("^::ascii->hex\\((?<Value>.+)\\)$");
        private readonly Regex _rgxASCIIToDec = new Regex("^::ascii->dec\\((?<Value>.+)\\)$");
        private readonly Regex _rgxTextToMorse = new Regex("^::morse\\((?<Value>.+)\\)$|^::text->morse\\((?<Value>.+)\\)$");
        private readonly Regex _rgxMorseToText = new Regex("^::morse->text\\((?<Value>.+)\\)$");

        /// <summary>
        /// Parser constructor.
        /// </summary>
        public Parser()
        {
            _scriptInitialized = false;

            _blocks = new BlockCollection();
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
                    if (!_scriptInitialized && _rgxStart.IsMatch(line))
                    {
                        Block block = new Block()
                        {
                            Name = _rgxStart.Match(line).Groups["BlockName"].Value.Trim(),
                            LineIndex = lineIndex
                        };

                        _blocks.Add(block);

                        continue;
                    }

                    if (_scriptInitialized)
                    {
                        if (_rgxExit.IsMatch(line))
                        {
                            Environment.Exit(0);
                        }

                        if (_rgxEnd.IsMatch(line))
                        {
                            break;
                        }

                        if (_rgxRun.IsMatch(line))
                        {
                            string blockName = _rgxRun.Match(line).Groups["BlockName"].Value.Trim();

                            if (!string.IsNullOrEmpty(blockName))
                            {
                                foreach (Block block in _blocks)
                                {
                                    if (block.Name.Equals(blockName))
                                    {
                                        ParseScriptLines(lines, (block.LineIndex + 1), end);
                                        break;
                                    }
                                }
                            }
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

            if (!_scriptInitialized)
            {
                foreach (Block block in _blocks)
                {
                    if (block.Name.Equals("main"))
                    {
                        _scriptInitialized = true;
                        ParseScriptLines(lines, (block.LineIndex + 1), end);
                        break;
                    }
                }

                if (!_scriptInitialized)
                {
                    Console.WriteLine("main block not found");
                }
            }
        }

        /// <summary>
        /// Parses a T14 command.
        /// </summary>
        /// <param name="command">The command to parse.</param>
        public void ParseCommand(string command)
        {
            if (_rgxVariable.IsMatch(command))
            {
                Variable variable = new Variable()
                {
                    Name = _rgxVariable.Match(command).Groups["VariableName"].Value,
                    Value = _rgxVariable.Match(command).Groups["VariableValue"].Value.Trim()
                };

                variables.Add(variable);
            }

            if (_rgxWTF.IsMatch(command))
            {
                Convert.WhatTheFuckIsThis(_rgxWTF.Match(command).Groups["Value"].Value);
            }

            ParseConversionMethods(command, true);
        }

        private void ParseConversionMethods(string value, bool newline)
        {
            bool match = false;

            if (_rgxHexToBin.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromHexToBinary(_rgxHexToBin.Match(value).Groups["Value"].Value));
            }

            if (_rgxHexToDec.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromHexToDecimal(_rgxHexToDec.Match(value).Groups["Value"].Value));
            }

            if (_rgxHexToASCII.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromHexToASCII(_rgxHexToASCII.Match(value).Groups["Value"].Value));
            }

            if(_rgxBinToDec.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromBinaryToDecimal(_rgxBinToDec.Match(value).Groups["Value"].Value));
            }

            if (_rgxBinToHex.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromBinaryToHex(_rgxBinToHex.Match(value).Groups["Value"].Value));
            }

            if (_rgxBinToASCII.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromBinaryToASCII(_rgxBinToASCII.Match(value).Groups["Value"].Value));
            }

            if (_rgxDecToHex.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromDecimalToHex(_rgxDecToHex.Match(value).Groups["Value"].Value));
            }

            if (_rgxDecToBin.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromDecimalToBinary(_rgxDecToBin.Match(value).Groups["Value"].Value));
            }

            if(_rgxDecToASCII.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromDecimalToASCII(_rgxDecToASCII.Match(value).Groups["Value"].Value));
            }

            if(_rgxASCIIToBin.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromASCIIToBinary(_rgxASCIIToBin.Match(value).Groups["Value"].Value));
            }

            if (_rgxASCIIToHex.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromASCIIToHex(_rgxASCIIToHex.Match(value).Groups["Value"].Value));
            }

            if (_rgxASCIIToDec.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromASCIIToDecimal(_rgxASCIIToDec.Match(value).Groups["Value"].Value));
            }

            if (_rgxTextToMorse.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromTextToMorse(_rgxTextToMorse.Match(value).Groups["Value"].Value));
            }

            if (_rgxMorseToText.IsMatch(value))
            {
                match = true;
                Console.Write(Convert.FromMorseToText(_rgxMorseToText.Match(value).Groups["Value"].Value));
            }

            if (match && newline)
            {
                Console.WriteLine();
            }
        }
    }
}
