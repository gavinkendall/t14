//-----------------------------------------------------------------------
// <copyright file="Convert.cs" company="Gavin Kendall">
//     Copyright (c) Gavin Kendall. All rights reserved.
// </copyright>
// <author>Gavin Kendall</author>
// <summary>A class for converting stuff.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace t14
{
    /// <summary>
    /// A class for converting stuff.
    /// </summary>
    public static class Convert
    {
        // International Morse Code
        // https://en.wikipedia.org/wiki/Morse_code
        private static Dictionary<char, string> morseCodes = new Dictionary<char, string>()
        {
            { 'A', ".-" },
            { 'B', "-..." },
            { 'C', "-.-." },
            { 'D', "-.." },
            { 'E', "." },
            { 'F', "..-." },
            { 'G', "--." },
            { 'H', "...." },
            { 'I', ".." },
            { 'J', ".---" },
            { 'K', "-.-" },
            { 'L', ".-.." },
            { 'M', "--" },
            { 'N', "-." },
            { 'O', "---" },
            { 'P', ".--." },
            { 'Q', "--.-" },
            { 'R', ".-." },
            { 'S', "..." },
            { 'T', "-" },
            { 'U', "..-" },
            { 'V', "...-" },
            { 'W', ".--" },
            { 'X', "-..-" },
            { 'Y', "-.--" },
            { 'Z', "--.." },
            { '1', ".----" },
            { '2', "..---" },
            { '3', "...--" },
            { '4', "....-" },
            { '5', "....." },
            { '6', "-...." },
            { '7', "--..." },
            { '8', "---.." },
            { '9', "----." },
            { '0', "-----" }
        };

        /// <summary>
        /// Attempts to figure out what a given unknown value actually is by trying out different conversion methods.
        /// </summary>
        /// <param name="unknown">Any unknown value.</param>
        public static void WhatTheFuckIsThis(string unknown)
        {
            if (string.IsNullOrEmpty(unknown) || string.IsNullOrWhiteSpace(unknown))
                return;

            Console.WriteLine("::wtf(" + unknown + ")");
            Console.WriteLine("I'm trying to figure out what this is ...");
            Console.WriteLine(unknown);

            Regex rgxBin = new Regex("^[0|1]+$");
            Regex rgxHex = new Regex("^[0-9a-fA-F]+$");

            if (!string.IsNullOrEmpty(unknown) && rgxBin.IsMatch(unknown))
            {
                Console.WriteLine("\nI think it might be something in binary.");

                Console.WriteLine("\n::bin->dec(" + unknown + ")");
                Console.WriteLine("The decimal representation of this binary value is ...");
                Console.WriteLine(FromBinaryToDecimal(unknown));

                Console.WriteLine("\n::bin->hex(" + unknown + ")");
                Console.WriteLine("The hex representation of this binary value is ...");
                Console.WriteLine(FromBinaryToHex(unknown));

                Console.WriteLine("\n::bin->ascii(" + unknown + ")");
                Console.WriteLine("The ASCII representation of this binary value is ...");
                Console.WriteLine(FromBinaryToASCII(unknown));
            }
            else if (!string.IsNullOrEmpty(unknown) && rgxHex.IsMatch(unknown))
            {
                Console.WriteLine("\nI think it might be something in hexadecimal.");

                Console.WriteLine("\n::hex->dec(" + unknown + ")");
                Console.WriteLine("The decimal representation of this hex value is ...");
                Console.WriteLine(FromHexToDecimal(unknown));

                Console.WriteLine("\n::hex->ascii(" + unknown + ")");
                string ascii = FromHexToASCII(unknown);
                if (!string.IsNullOrEmpty(ascii) && ascii.Length > 0)
                {
                    Console.WriteLine("The ASCII representation of this hex value is ...");
                    Console.WriteLine(FromHexToASCII(unknown));
                }
            }

            Console.WriteLine("\nThat's as much as I could figure out.");
            Console.WriteLine("I'm sorry if it wasn't helpful or sufficient.");
        }

        /// <summary>
        /// ::hex->bin(value)
        /// Converts a hexadecimal value into a binary value.
        /// </summary>
        /// <param name="value">The hexadecimal value to convert into a binary value.</param>
        /// <returns>The converted value in binary.</returns>
        public static string FromHexToBinary(string value)
        {
            return System.Convert.ToString(System.Convert.ToInt64(value, 16), 2);
        }

        /// <summary>
        /// ::hex->dec(value)
        /// Converts a hexadecimal value into a decimal value.
        /// </summary>
        /// <param name="value">The hexadecimal value to convert as a decimal value.</param>
        /// <returns>The converted value in decimal.</returns>
        public static string FromHexToDecimal(string value)
        {
            if (int.TryParse(value, NumberStyles.HexNumber,
                CultureInfo.CurrentCulture, out int integer))
            {
                return integer.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// ::hex->ascii(value)
        /// Converts a hexadecimal value into ASCII.
        /// </summary>
        /// <param name="value">The hexadecimal value to convert into ASCII.</param>
        /// <returns>The converted value in ASCII.</returns>
        public static string FromHexToASCII(string value)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < value.Length; i += 2)
            {
                if ((i + 2) <= value.Length)
                {
                    string hs = value.Substring(i, 2);
                    sb.Append(System.Convert.ToChar(System.Convert.ToUInt64(hs, 16)));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// ::bin->dec(value)
        /// Converts a binary value into its decimal representation.
        /// </summary>
        /// <param name="value">The binary value to convert to decimal.</param>
        /// <returns>The converted value in decimal.</returns>
        public static string FromBinaryToDecimal(string value)
        {
            return System.Convert.ToInt64(value, 2).ToString();
        }

        /// <summary>
        /// ::bin->hex(value)
        /// Converts a binary value into its hexadecimal representation.
        /// </summary>
        /// <param name="value">The binary value to convert to hexadecimal.</param>
        /// <returns>The converted value in hexadecimal.</returns>
        public static string FromBinaryToHex(string value)
        {
            return System.Convert.ToInt64(value, 2).ToString("X");
        }

        /// <summary>
        /// ::hex->ascii(value)
        /// Converts a binary value into ASCII.
        /// </summary>
        /// <param name="value">The binary value to convert into ASCII.</param>
        /// <returns>The converted value in ASCII.</returns>
        public static string FromBinaryToASCII(string value)
        {
            var list = new List<Byte>();

            for (int i = 0; i < value.Length; i += 8)
            {
                if ((i + 8) <= value.Length)
                {
                    string t = value.Substring(i, 8);
                    list.Add(System.Convert.ToByte(t, 2));
                }
            }

            var byteArray = list.ToArray();
            return Encoding.ASCII.GetString(byteArray);
        }

        /// <summary>
        /// ::dec->hex(value)
        /// Converts a decimal value into its hexadecimal representation.
        /// </summary>
        /// <param name="value">The decimal value to convert to hexadecimal.</param>
        /// <returns>The converted value in hexadecimal.</returns>
        public static string FromDecimalToHex(string value)
        {
            if (int.TryParse(value, out int number))
            {
                return number.ToString("X");
            }

            return string.Empty;
        }

        /// <summary>
        /// ::dec->bin(value)
        /// Converts a decimal value into its binary representation.
        /// </summary>
        /// <param name="value">The decimal value to convert to binary.</param>
        /// <returns>The converted value in binary.</returns>
        public static string FromDecimalToBinary(string value)
        {
            if (int.TryParse(value, out int number))
            {
                return System.Convert.ToString(number, 2);
            }

            return string.Empty;
        }

        /// <summary>
        /// ::dec->ascii(value)
        /// Converts a decimal value into its ASCII representation.
        /// </summary>
        /// <param name="value">The decimal value to convert to ASCII.</param>
        /// <returns>The converted value in ASCII.</returns>
        public static string FromDecimalToASCII(string value)
        {
            return System.Convert.ToChar(System.Convert.ToInt64(value)).ToString();
        }

        /// <summary>
        /// ::ascii->bin(value)
        /// Converts ASCII to binary.
        /// </summary>
        /// <param name="value">The ASCII value to convert to binary.</param>
        /// <returns>The converted value as binary.</returns>
        public static string FromASCIIToBinary(string value)
        {
            StringBuilder sb = new StringBuilder();
            byte[] bytes = Encoding.ASCII.GetBytes(value);

            foreach (byte b in bytes)
            {
                sb.Append(System.Convert.ToString(b, 2));
            }

            return sb.ToString();
        }

        /// <summary>
        /// ::ascii->hex(value)
        /// Converts ASCII to hexadecimal.
        /// </summary>
        /// <param name="value">The ASCII value to convert to hex.</param>
        /// <returns>The converted value as hex.</returns>
        public static string FromASCIIToHex(string value)
        {
            StringBuilder sb = new StringBuilder();
            byte[] bytes = Encoding.ASCII.GetBytes(value);

            foreach (byte b in bytes)
            {
                sb.Append(System.Convert.ToString(b, 16));
            }

            return sb.ToString();
        }

        /// <summary>
        /// ::ascii->dec(value)
        /// Converts ASCII to decimal.
        /// </summary>
        /// <param name="value">The ASCII value to convert to decimal.</param>
        /// <returns>The converted value as ASCII.</returns>
        public static string FromASCIIToDecimal(string value)
        {
            return System.Convert.ToInt64(System.Convert.ToChar(value)).ToString();
        }

        /// <summary>
        /// ::morse(value)
        /// ::text->morse(value)
        /// Converts text into Morse code.
        /// Based on code from http://www.blackwasp.co.uk/MorseCode.aspx
        /// </summary>
        /// <param name="value">The text to convert into Morse code.</param>
        /// <returns>The Morse code representation of the given text.</returns>
        public static string FromTextToMorse(string value)
        {
            string[] words = value.ToUpper().Split(' ');

            StringBuilder sb = new StringBuilder();

            foreach (string word in words)
            {
                if (sb.Length != 0)
                    sb.Append("/");

                sb.Append(MorseWord(word));
            }

            return sb.ToString();
        }

        /// <summary>
        /// ::morse->text(value)
        /// Converts morse code into normal text.
        /// </summary>
        /// <param name="value">The morse code to convert back to normal text.</param>
        /// <returns>The normal text of the given morse code.</returns>
        public static string FromMorseToText(string value)
        {
            string[] words = value.Split('/');

            //Console.WriteLine(words[0]);

            StringBuilder sb = new StringBuilder();

            foreach(string word in words)
            {
                string[] codes = word.Split(' ');

                foreach (string ch in codes)
                {
                    sb.Append(morseCodes.FirstOrDefault(x => x.Value == ch).Key);
                }

                sb.Append(" ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Based on code from http://www.blackwasp.co.uk/MorseCode.aspx
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static string MorseWord(string word)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char ch in word)
            {
                if (sb.Length != 0 && sb[sb.Length - 1] != ' ')
                    sb.Append(" ");

                sb.Append(MorseCharacter(ch));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Based on code from http://www.blackwasp.co.uk/MorseCode.aspx
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private static string MorseCharacter(char ch)
        {
            foreach(char c in morseCodes.Keys)
            {
                if (c == ch)
                {
                    return morseCodes[c];
                }
            }

            return string.Empty;
        }
    }
}