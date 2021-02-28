//-----------------------------------------------------------------------
// <copyright file="Variable.cs" company="Gavin Kendall">
//     Copyright (c) 2020-2021 Gavin Kendall
// </copyright>
// <author>Gavin Kendall</author>
// <summary>A class representing a block.</summary>
//-----------------------------------------------------------------------
namespace t14
{
    /// <summary>
    /// A class representing a block.
    /// </summary>
    public class Block
    {
        /// <summary>
        /// The name of the block.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The line index where the block starts.
        /// </summary>
        public int LineIndex { get; set; }

        /// <summary>
        /// Variable constructor.
        /// </summary>
        public Block()
        {

        }
    }
}