using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace HannaHandCipher
{
    /// <summary>
    ///     A german variant of the CT-37 straddling checkerboard.
    ///     This key matrix encodes a digit string by interpreting the string's digits as coordinates.
    ///     The 7 X and 3 Y coordinates represented by the 0-9 digits are assigned on it's initialization.
    ///     Seven single X coordinates are mapped to the 7 most common characters in the german alphabet: "ESTLAND".
    ///     The 3/10 digits left represent the X in an XY coordinate which is mapped to the remaining alphabet characters,
    ///     whitespace, and digit characters.
    ///     Following after one of the 3/10 X digits is it's corresponding Y digit.
    /// </summary>
    /*
     Example:
               0 1 2 3 4 5 6 7 8 9
                      (X)
               6 4 8 7 1 5 9 3 0 2
               E   S   T   L A N D
      0      4 B C F G H I J K M O
      1  (Y) 7 P Q R U V W X Y Z 
      2      5 1 2 3 4 5 6 7 8 9 0
     */
    public class StraddlingCheckerboardKeyMatrix
    {
        private static Dictionary<string, char> _keyMatrix;

        // A permutation of the 10 digits in the "0-9" range followed by 3 digits within the "0-9" range.  
        private readonly string _tenColumnDigitKeys;
        private readonly string _threeRowDigitKeys;

        public StraddlingCheckerboardKeyMatrix(string tenColumnDigitKeys, string threeRowDigitKeys)
        {
            _tenColumnDigitKeys = tenColumnDigitKeys;
            _threeRowDigitKeys = threeRowDigitKeys;

            if (tenColumnDigitKeys.Length != 10)
                throw new ArgumentException(
                    $"\"digitKeys\" contains {tenColumnDigitKeys.Length} digits. 10 digits are required.");
            if (threeRowDigitKeys.Length != 3)
                throw new ArgumentException(
                    $"\"digitKeys\" contains {tenColumnDigitKeys.Length} digits. 3 digits are required.");

            _keyMatrix = new Dictionary<string, char>();

            // If row key is unique, assign a character from "ESTLAND" to it.
            const string estandCharacters = "ESTLAND";
            int estlandIdx = 0;
            foreach (char uniqueColumnKey in 
                from columnKey 
                    in tenColumnDigitKeys 
                let isUnique = threeRowDigitKeys.All(rowKey => columnKey != rowKey) 
                where isUnique 
                select columnKey)
            {
                _keyMatrix.Add($"{uniqueColumnKey}", estandCharacters[estlandIdx]);
                estlandIdx++;
            }

            // Remaining alphabet B-Z
            int column = 0;
            int row = 0;
            for (int alphabetASCII = 65; alphabetASCII <= 90; alphabetASCII++)
            {
                // Next line when row is filled.
                if (column >= tenColumnDigitKeys.Length)
                {
                    column = 0;
                    row++;
                }

                if (_keyMatrix.ContainsValue((char) alphabetASCII)) continue;
                _keyMatrix.Add($"{threeRowDigitKeys[row]}{tenColumnDigitKeys[column]}", (char) alphabetASCII);
                column++;
            }

            // Next line
            row++;
            column = 0;

            // 1-9
            for (int digitASCII = 49; digitASCII <= 57; digitASCII++)
            {
                _keyMatrix.Add($"{threeRowDigitKeys[row]}{tenColumnDigitKeys[column]}", (char) digitASCII);
                column++;
            }

            // 0
            _keyMatrix.Add($"{threeRowDigitKeys[row]}{tenColumnDigitKeys[column]}", '0');
        }

        public static string CharValuesToDigitKeys(string stringToEncode)
        {
            stringToEncode = stringToEncode.ToUpper();

            // Remove unsupported characters.
            stringToEncode = Regex.Replace(stringToEncode, @"[^A-Z0-9]", "");
            stringToEncode = stringToEncode.Replace(" ", "");

            return stringToEncode.Aggregate("",
                (current, c) => current + _keyMatrix.FirstOrDefault(x => x.Value == c).Key);
        }

        public string DigitKeysToCharValues(string digitKeySeriesToDecode)
        {
            string output = "";
            for (int index = 0; index < digitKeySeriesToDecode.Length; index++)
            {
                char rowDigit = digitKeySeriesToDecode[index];
                if (Array.Exists(_threeRowDigitKeys.ToCharArray(), rowKey => rowKey.Equals(rowDigit)))
                {
                    // Is not an ESTLAND coordinate.
                    index++;
                    int columnDigit = CharUnicodeInfo.GetDecimalDigitValue(digitKeySeriesToDecode[index]);
                    output += _keyMatrix.FirstOrDefault(x => x.Key == $"{rowDigit}{columnDigit}").Value;
                }
                else
                {
                    // Is a ESTLAND coordinate.
                    output += _keyMatrix.FirstOrDefault(
                        x => x.Key == rowDigit.ToString()).Value;
                }
            }

            return output;
        }
    }
}