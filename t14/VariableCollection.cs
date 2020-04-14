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
    /// 
    /// </summary>
    public class VariableCollection
    {
        private List<Variable> variables = new List<Variable>();

        /// <summary>
        /// 
        /// </summary>
        public VariableCollection()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        public void Add(Variable variable)
        {
            if (!variables.Contains(variable))
            {
                variables.Add(variable);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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
