//-----------------------------------------------------------------------
// <copyright file="ScriptParser.cs" company="Gavin Kendall">
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
    public class ScriptParser
    {
        // Blocks.
        private readonly BlockCollection blocks;
        private readonly Regex rgxStart = new Regex("^::start\\((?<BlockName>[0-9a-zA-Z_-]+)\\)$");
        private readonly Regex rgxEnd = new Regex("^::end$");

        // Variables.
        private readonly VariableCollection variables;
        private readonly Regex rgxVariable = new Regex("^::set (?<VariableName>\\$[0-9a-zA-Z_-]+) = (?<VariableValue>.+)$");

        public ScriptParser()
        {
            blocks = new BlockCollection();
            variables = new VariableCollection();
        }
    }
}
