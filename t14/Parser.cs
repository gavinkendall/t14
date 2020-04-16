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
        private readonly Regex rgxHex = new Regex("^::hex\\((?<ValueToConvert>.+)\\)$");
        private readonly Regex rgxBin = new Regex("^::bin\\((?<ValueToConvert>.+)\\)$");

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

            if (!filename.EndsWith(".t14"))
            {
                Console.WriteLine($"Error: File named {filename} does not end with extension \".t14\".");
                return;
            }

            VariableCollection variables = new VariableCollection();

            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                // Ignore any line that starts with # because it should be interpreted as a comment.
                if (line.StartsWith("#"))
                    continue;

                // Any line that starts with :: should be interpreted as a command.
                if (line.StartsWith("::"))
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

                    if (rgxHex.IsMatch(line))
                    {
                        Console.WriteLine(Convert.FromDecimalToHex(rgxHex.Match(line).Groups["ValueToConvert"].Value));
                    }

                    if (rgxBin.IsMatch(line))
                    {
                        Console.WriteLine(Convert.FromDecimalToBinary(rgxBin.Match(line).Groups["ValueToConvert"].Value));
                    }

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
                    if (word.StartsWith("$"))
                    {
                        // Output the value of the variable if we find it in the variale collection using the variable name.
                        Variable variable = variables.GetByName(word);

                        if (variable != null)
                        {
                            Console.Write(variable.Value);
                        }
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
    }
}
