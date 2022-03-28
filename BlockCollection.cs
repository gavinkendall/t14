//-----------------------------------------------------------------------
// <copyright file="BlockCollection.cs" company="Gavin Kendall">
//     Copyright (c) 2020-2021 Gavin Kendall
// </copyright>
// <author>Gavin Kendall</author>
// <summary>A class representing a collection of blocks.</summary>
//-----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;

namespace t14
{
    /// <summary>
    /// A class representing a collection of blocks.
    /// </summary>
    public class BlockCollection : IEnumerable<Block>
    {
        private readonly List<Block> blocks = new List<Block>();

        /// <summary>
        /// Block Collection constructor.
        /// </summary>
        public BlockCollection()
        {

        }

        public List<Block>.Enumerator GetEnumerator()
        {
            return blocks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Block>)blocks).GetEnumerator();
        }

        IEnumerator<Block> IEnumerable<Block>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return blocks.Count; }
        }

        /// <summary>
        /// Adds a block to the block collection.
        /// </summary>
        /// <param name="block">A block to add to the block collection.</param>
        public void Add(Block block)
        {
            if (!blocks.Contains(block))
            {
                blocks.Add(block);
            }
        }

        /// <summary>
        /// Gets a block from the block collection by the block name.
        /// </summary>
        /// <param name="name">The name of a block to get from the block collection.</param>
        /// <returns>The block found in the block collection.</returns>
        public Block GetByName(string name)
        {
            foreach (Block block in blocks)
            {
                if (block.Name.Equals(name))
                {
                    return block;
                }
            }

            return null;
        }
    }
}
