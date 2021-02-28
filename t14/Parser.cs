//-----------------------------------------------------------------------
// <copyright file="Parser.cs" company="Gavin Kendall">
//     Copyright (c) 2020-2021 Gavin Kendall
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
        private readonly Regex _rgxStart = new Regex("^::start\\[(?<BlockName>[0-9a-zA-Z_-]+)\\]$");
        private readonly Regex _rgxEnd = new Regex("^::end$");

        // Run command.
        private readonly Regex _rgxRun = new Regex("^(?<Command>::run\\[(?<BlockName>[0-9a-zA-Z_-]+)\\])$");

        // If command.
        private readonly Regex _rgxIf = new Regex("^(?<Command>::if \\[(?<LeftValue>) (?<Operator>) (?<RightValue>)\\]->\\[(?<BlockName>[0-9a-zA-Z_-]+)\\])$");

        // Variables.
        private readonly VariableCollection _variables;
        private readonly Regex _rgxVariableName = new Regex("^\\[(?<VariableName>[0-9a-zA-Z_-]+)\\]$");
        private readonly Regex _rgxVariablesInLine = new Regex("(?<Variables>\\[(?<VariableName>[0-9a-zA-Z_-]+)\\]+)");
        private readonly Regex _rgxSetVariableWithValue = new Regex("^(?<Command>::set \\[(?<VariableName>[0-9a-zA-Z_-]+)\\] = (?<VariableValue>.+))$");
        private readonly Regex _rgxSetVariableWithVariable = new Regex("^(?<Command>::set \\[(?<VariableWithOldValue>[0-9a-zA-Z_-]+)\\] = \\[(?<VariableWithNewValue>[0-9a-zA-Z_-]+)\\])$");

        // The moment where you're looking at something and just go, "What the fuck is this?!".
        // So just pass in the unknown value and let the wtf command do the rest.
        private readonly Regex _rgxWTF = new Regex("::wtf\\[(?<Value>.+)\\]");

        // Conversion methods.
        private Convert _convert;
        private readonly Regex _rgxHexToBin = new Regex("(?<Method>::hex->bin\\[(?<Value>.+)\\])");
        private readonly Regex _rgxHexToDec = new Regex("(?<Method>::hex->dec\\[(?<Value>.+)\\])");
        private readonly Regex _rgxHexToASCII = new Regex("(?<Method>::hex->ascii\\[(?<Value>.+)\\])");
        private readonly Regex _rgxBinToDec = new Regex("(?<Method>::bin->dec\\[(?<Value>.+)\\])");
        private readonly Regex _rgxBinToHex = new Regex("(?<Method>::bin->hex\\[(?<Value>.+)\\])");
        private readonly Regex _rgxBinToASCII = new Regex("(?<Method>::bin->ascii\\[(?<Value>.+)\\])");
        private readonly Regex _rgxDecToHex = new Regex("(?<Method>::dec->hex\\[(?<Value>.+)\\])");
        private readonly Regex _rgxDecToBin = new Regex("(?<Method>::dec->bin\\[(?<Value>.+)\\])");
        private readonly Regex _rgxDecToASCII = new Regex("(?<Method>::dec->ascii\\[(?<Value>.+)\\])");
        private readonly Regex _rgxASCIIToBin = new Regex("(?<Method>::ascii->bin\\[(?<Value>.+)\\])");
        private readonly Regex _rgxASCIIToHex = new Regex("(?<Method>::ascii->hex\\[(?<Value>.+)\\])");
        private readonly Regex _rgxASCIIToDec = new Regex("(?<Method>::ascii->dec\\[(?<Value>.+)\\])");
        private readonly Regex _rgxTextToMorse = new Regex("(?<Method>::morse\\[(?<Value>.+)\\]$|^::text->morse\\[(?<Value>.+)\\])");
        private readonly Regex _rgxMorseToText = new Regex("(?<Method>::morse->text\\[(?<Value>.+)\\])");
        private readonly Regex _rgxDecToRoman = new Regex("(?<Method>::dec->roman\\[(?<Value>.+)\\])");

        /// <summary>
        /// Parser constructor.
        /// </summary>
        public Parser()
        {
            _scriptInitialized = false;

            _convert = new Convert();
            _blocks = new BlockCollection();
            _variables = new VariableCollection();
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

            // Parse the lines in the script from the very first line (index 0) to the last line.
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

                // If we haven't initializaed the script yet and we find a starting block ("::start") then add the block to the block collection.
                if (!_scriptInitialized && _rgxStart.IsMatch(line))
                {
                    Block block = new Block()
                    {
                        Name = _rgxStart.Match(line).Groups["BlockName"].Value.Trim(),
                        LineIndex = lineIndex // The index of where we found the ::start command for the start of the block.
                    };

                    _blocks.Add(block);

                    continue;
                }

                // Parse the blocks in the script when the script has been initialized.
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

                        line = line.Replace(_rgxRun.Match(line).Groups["Command"].Value, string.Empty);

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

                    line = ParseCommand(line);
                    line = ParseConversionMethod(line);

                    if (!string.IsNullOrEmpty(line) && !string.IsNullOrWhiteSpace(line))
                    {
                        //Console.WriteLine(line.Replace(line, GetVariableNameFromValue(line)));

                        if (_rgxVariablesInLine.IsMatch(line))
                        {
                            foreach (Match variableName in _rgxVariablesInLine.Matches(line))
                            {
                                //Console.Write(_variables.GetByName(variableName.Groups[1].Value));
                            }
                        }
                        //Console.WriteLine(line);
                    }
                }
            }

            if (!_scriptInitialized)
            {
                // Go through the blocks in the block collection looking for a block named "main".
                // If we find a block named "main" then we consider the script initialized and parse the lines again.
                foreach (Block block in _blocks)
                {
                    if (block.Name.Equals("main"))
                    {
                        _scriptInitialized = true;
                        ParseScriptLines(lines, (block.LineIndex + 1), end);
                        break;
                    }
                }

                // The "main" block could not be found.
                if (!_scriptInitialized)
                {
                    //Console.WriteLine("main block not found");
                }
            }
        }

        /// <summary>
        /// Parses a T14 command that's on its own line in a T14 script.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        private string ParseCommand(string line)
        {
            if (_rgxSetVariableWithValue.IsMatch(line))
            {
                Variable variable = new Variable()
                {
                    Name = _rgxSetVariableWithValue.Match(line).Groups["VariableName"].Value,
                    Value = _rgxSetVariableWithValue.Match(line).Groups["VariableValue"].Value.Trim()
                };

                _variables.Add(variable);

                line = line.Replace(_rgxSetVariableWithValue.Match(line).Groups["Command"].Value, string.Empty);
            }
            else if (_rgxSetVariableWithVariable.IsMatch(line))
            {
                string variableWithOldValue = _rgxSetVariableWithVariable.Match(line).Groups["VariableWithOldValue"].Value;
                string variableWithNewValue = _rgxSetVariableWithVariable.Match(line).Groups["VariableWithNewValue"].Value;

                Variable varOld = _variables.GetByName(variableWithOldValue);
                Variable varNew = _variables.GetByName(variableWithNewValue);

                if (varOld != null && varNew != null)
                {
                    varOld.Value = varNew.Value;
                }

                line = line.Replace(_rgxSetVariableWithVariable.Match(line).Groups["Command"].Value, string.Empty);
            }
            else if (_rgxIf.IsMatch(line))
            {
                string leftValue = _rgxIf.Match(line).Groups["LeftValue"].Value;
                string @operator = _rgxIf.Match(line).Groups["Operator"].Value;
                string rightValue = _rgxIf.Match(line).Groups["RightValue"].Value;
                string blockName = _rgxIf.Match(line).Groups["BlockName"].Value;

                ParseIf(leftValue, @operator, rightValue, blockName);

                line = line.Replace(_rgxIf.Match(line).Groups["Command"].Value, string.Empty);
            }

            return line;
        }

        /// <summary>
        /// Parses the methods used for converting a type of data into another type of data (such as decimal to binary).
        /// A conversion method could be called from either a command line terminal, on its own line in the script, or within a line in the script.
        /// For example, executing this command in a command line terimal ...
        /// t14.exe ::dec->bin[1234567]
        /// ... will output ...
        /// 100101101011010000111
        /// ... or writing this line in a T14 script ...
        /// ::dec->bin[1234567]
        /// ... will output ...
        /// 100101101011010000111
        /// ... and writing this line in a T14 script ...
        /// The binary of 1234567 is ::dec->bin[1234567].
        /// ... will output ...
        /// The binary of 1234567 is 100101101011010000111.
        /// </summary>
        /// <param name="line">The line with the conversion method (such as "::dec->bin[1234567]").</param>
        public string ParseConversionMethod(string line)
        {
            if (_rgxHexToBin.IsMatch(line))
            {
                string method = _rgxHexToBin.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxHexToBin.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromHexToBinary(value));
            }
            else if (_rgxHexToDec.IsMatch(line))
            {
                string method = _rgxHexToDec.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxHexToDec.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromHexToDecimal(value));
            }
            else if (_rgxHexToASCII.IsMatch(line))
            {
                string method = _rgxHexToASCII.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxHexToASCII.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromHexToASCII(value));
            }
            else if (_rgxBinToDec.IsMatch(line))
            {
                string method = _rgxBinToDec.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxBinToDec.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromBinaryToDecimal(value));
            }
            else if (_rgxBinToHex.IsMatch(line))
            {
                string method = _rgxBinToHex.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxBinToHex.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromBinaryToHex(value));
            }
            else if (_rgxBinToASCII.IsMatch(line))
            {
                string method = _rgxBinToASCII.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxBinToASCII.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromBinaryToASCII(value));
            }
            else if (_rgxDecToHex.IsMatch(line))
            {
                string method = _rgxDecToHex.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxDecToHex.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromDecimalToHex(value));
            }
            else if (_rgxDecToBin.IsMatch(line))
            {
                string method = _rgxDecToBin.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxDecToBin.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromDecimalToBinary(value));
            }
            else if (_rgxDecToASCII.IsMatch(line))
            {
                string method = _rgxDecToASCII.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxDecToASCII.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromDecimalToASCII(value));
            }
            else if (_rgxASCIIToBin.IsMatch(line))
            {
                string method = _rgxASCIIToBin.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxASCIIToBin.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromASCIIToBinary(value));
            }
            else if (_rgxASCIIToHex.IsMatch(line))
            {
                string method = _rgxASCIIToHex.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxASCIIToHex.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromASCIIToHex(value));
            }
            else if (_rgxASCIIToDec.IsMatch(line))
            {
                string method = _rgxASCIIToDec.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxASCIIToDec.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromASCIIToDecimal(value));
            }
            else if (_rgxTextToMorse.IsMatch(line))
            {
                string method = _rgxTextToMorse.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxTextToMorse.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromTextToMorse(value));
            }
            else if (_rgxMorseToText.IsMatch(line))
            {
                string method = _rgxMorseToText.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxMorseToText.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromMorseToText(value));
            }
            else if (_rgxDecToRoman.IsMatch(line))
            {
                string method = _rgxDecToRoman.Match(line).Groups["Method"].Value;
                string value = GetVariableNameFromValue(_rgxDecToRoman.Match(line).Groups["Value"].Value);

                line = line.Replace(method, _convert.FromDecimalToRoman(value));
            }

            return line;
        }

        private string GetVariableNameFromValue(string value)
        {
            if (_rgxVariableName.IsMatch(value))
            {
                string variableName = _rgxVariableName.Match(value).Groups["VariableName"].Value;
                value = _variables.GetByName(variableName).Value;
            }

            return value;
        }

        private void ParseIf(string leftValue, string @operator, string rightValue, string blockName)
        {

        }
    }
}
