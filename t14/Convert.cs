//-----------------------------------------------------------------------
// <copyright file="Convert.cs" company="Gavin Kendall">
//     Copyright (c) Gavin Kendall. All rights reserved.
// </copyright>
// <author>Gavin Kendall</author>
// <summary>A class to convert stuff.</summary>
//-----------------------------------------------------------------------
namespace t14
{
    /// <summary>
    /// A class to convert stuff.
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// Converts a value into its hexadecimal equivalent as a string.
        /// </summary>
        /// <param name="valueToConvert">The value to convert to hexadecimal.</param>
        /// <returns>The converted value in hexadecimal.</returns>
        public static string ToHexadecimal(string valueToConvert)
        {
            string hex = string.Empty;

            if (int.TryParse(valueToConvert, out int number))
            {
                hex = number.ToString("X");
            }

            return hex;
        }

        /// <summary>
        /// Converts a value into its binary equivalent as a string.
        /// </summary>
        /// <param name="valueToConvert"></param>
        /// <returns></returns>
        public static string ToBinary(string valueToConvert)
        {
            string bin = string.Empty;

            if (int.TryParse(valueToConvert, out int number))
            {
                bin = System.Convert.ToString(number, 2);
            }

            return bin;
        }
    }
}
