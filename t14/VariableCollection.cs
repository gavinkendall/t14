//-----------------------------------------------------------------------
// <copyright file="VariableCollection.cs" company="Gavin Kendall">
//     Copyright (c) Gavin Kendall. All rights reserved.
// </copyright>
// <author>Gavin Kendall</author>
// <summary>A class representing a collection of variables.</summary>
//-----------------------------------------------------------------------
using System.Collections.Generic;

namespace t14
{
    /// <summary>
    /// A class representing a collection of variables.
    /// </summary>
    public class VariableCollection
    {
        private readonly List<Variable> variables = new List<Variable>();

        /// <summary>
        /// Variable Collection constructor.
        /// </summary>
        public VariableCollection()
        {

        }

        /// <summary>
        /// Adds a variable to the variable collection.
        /// </summary>
        /// <param name="variable">A variable to add to the variable collection.</param>
        public void Add(Variable variable)
        {
            if (!variables.Contains(variable))
            {
                variables.Add(variable);
            }
        }

        /// <summary>
        /// Gets a variable from the variable collection by the variable name.
        /// </summary>
        /// <param name="name">The name of a variable to get from the variable collection.</param>
        /// <returns>The variable found in the variable collection.</returns>
        public Variable GetByName(string name)
        {
            foreach(Variable variable in variables)
            {
                if (variable.Name.Equals(name))
                {
                    return variable;
                }
            }

            return null;
        }
    }
}
