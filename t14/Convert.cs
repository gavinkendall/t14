//-----------------------------------------------------------------------
// <copyright file="Convert.cs" company="Gavin Kendall">
//     Copyright (c) Gavin Kendall. All rights reserved.
// </copyright>
// <author>Gavin Kendall</author>
// <summary>A class to convert stuff.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace t14
{
    /// <summary>
    /// A class to convert stuff.
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// Attempts to figure out what a given unknown value actually is by trying out different conversion methods.
        /// </summary>
        /// <param name="unknown">Any unknown value.</param>
        public static void WhatTheFuckIsThis(string unknown)
        {
            if (string.IsNullOrEmpty(unknown) || string.IsNullOrWhiteSpace(unknown))
                return;

            Console.WriteLine("I'm trying to figure out what this is ...");
            Console.WriteLine(unknown);

            Regex rgxBin = new Regex("^[0|1]+$");
            Regex rgxHex = new Regex("^[0-9a-fA-F]+$");

            if (!string.IsNullOrEmpty(unknown) && rgxBin.IsMatch(unknown))
            {
                Console.WriteLine("I think it might be something in binary.");

                Console.WriteLine("The decimal representation of this binary value is ...");
                Console.WriteLine(FromBinaryToDecimal(unknown));

                Console.WriteLine("The hex representation of this binary value is ...");
                Console.WriteLine(FromBinaryToHex(unknown));

                Console.WriteLine("The ASCII representation of this binary value is ...");
                Console.WriteLine(FromBinaryToASCII(unknown));
            }
            else if (!string.IsNullOrEmpty(unknown) && rgxHex.IsMatch(unknown))
            {
                Console.WriteLine("I think it might be something in hexadecimal.");

                int integer = FromHexToInteger(unknown);
                string strInteger = integer == 0 ? "0 (but this might not actually be 0)" : integer.ToString();
                Console.WriteLine("The decimal representation of this hex value is ...");
                Console.WriteLine(strInteger);

                string ascii = FromHexToASCII(unknown);
                if (!string.IsNullOrEmpty(ascii) && ascii.Length > 0)
                {
                    Console.WriteLine("The ASCII representation of this hex value is ...");
                    Console.WriteLine(FromHexToASCII(unknown));
                }
            }

            Console.WriteLine("That's as much as I could figure out.");
            Console.WriteLine("I'm sorry if it wasn't helpful or sufficient.");
        }

        /// <summary>
        /// Converts a hexadecimal value into an integer.
        /// </summary>
        /// <param name="hex">The hexadecimal value to convert as an integer.</param>
        /// <returns>An integer from the given hexadecimal value.</returns>
        public static int FromHexToInteger(string hex)
        {
            if (int.TryParse(hex, NumberStyles.HexNumber,
                CultureInfo.CurrentCulture, out int integer))
            {
                return integer;
            }

            return integer;
        }

        /// <summary>
        /// Converts a hexadecimal value into ASCII.
        /// </summary>
        /// <param name="hex">The hexadecimal value to convert into ASCII.</param>
        /// <returns>The converted value in ASCII.</returns>
        public static string FromHexToASCII(string hex)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hex.Length; i += 2)
            {
                if ((i + 2) <= hex.Length)
                {
                    string hs = hex.Substring(i, 2);
                    sb.Append(System.Convert.ToChar(System.Convert.ToUInt32(hs, 16)));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a binary value into its decimal representation.
        /// </summary>
        /// <param name="bin">The binary value to convert to decimal.</param>
        /// <returns>The converted value in decimal.</returns>
        public static string FromBinaryToDecimal(string bin)
        {
            return System.Convert.ToInt32(bin, 2).ToString();
        }

        /// <summary>
        /// Converts a binary value into its hexadecimal representation.
        /// </summary>
        /// <param name="bin">The binary value to convert to hexadecimal.</param>
        /// <returns>The converted value in hexadecimal.</returns>
        public static string FromBinaryToHex(string bin)
        {
            return System.Convert.ToInt32(bin, 2).ToString("X");
        }

        /// <summary>
        /// Converts a binary value into ASCII.
        /// </summary>
        /// <param name="bin">The binary value to convert into ASCII.</param>
        /// <returns>The converted value in ASCII.</returns>
        public static string FromBinaryToASCII(string bin)
        {
            var list = new List<Byte>();

            for (int i = 0; i < bin.Length; i += 8)
            {
                if ((i + 8) <= bin.Length)
                {
                    string t = bin.Substring(i, 8);
                    list.Add(System.Convert.ToByte(t, 2));
                }
            }

            var byteArray = list.ToArray();
            return Encoding.ASCII.GetString(byteArray);
        }

        /// <summary>
        /// Converts a decimal value into its hexadecimal representation.
        /// </summary>
        /// <param name="decimal">The decimal value to convert to hexadecimal.</param>
        /// <returns>The converted value in hexadecimal.</returns>
        public static string FromDecimalToHex(string @decimal)
        {
            string hex = string.Empty;

            if (int.TryParse(@decimal, out int number))
            {
                hex = number.ToString("X");
            }

            return hex;
        }

        /// <summary>
        /// Converts a decimal value into its binary representation.
        /// </summary>
        /// <param name="decimal">The decimal value to convert to binary.</param>
        /// <returns>The converted value in binary.</returns>
        public static string FromDecimalToBinary(string @decimal)
        {
            string bin = string.Empty;

            if (int.TryParse(@decimal, out int number))
            {
                bin = System.Convert.ToString(number, 2);
            }

            return bin;
        }
    }
}
