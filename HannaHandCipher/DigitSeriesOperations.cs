using System;
using System.Globalization;
using System.Linq;

namespace HannaHandCipher
{
    public static class DigitSeriesOperations
    {
        /// <summary>
        ///     Relative Alphabetical Index Encode.
        ///     Encodes an alphabetic string by replacing each of the string's characters with it's index position (starting at
        ///     1 instead of 0) when sorted alphabetically.
        ///     For example, the word "example" is sorted alphabetically to: "aeelmpx". "example"'s characters are replaced with
        ///     their index position in "aeelmpx", with left-to-right preference: "2715643".
        /// </summary>
        public static string RelativeAlphabeticalIndexEncode(string stringToEncode)
        {
            // Remove whitespace.
            stringToEncode = stringToEncode.Replace(" ", string.Empty);

            // Change string to uppercase for case-insensitivity.
            stringToEncode = stringToEncode.ToUpper();

            // Get the characters sorted alphabetically.
            string sortedStringToEncode = string.Concat(stringToEncode.Split().OrderBy(c => c));
            char[] encodedStringChars = sortedStringToEncode.ToCharArray();
            Array.Sort(encodedStringChars);
            sortedStringToEncode = new string(encodedStringChars);

            // Replace each character with it's index in the sorted string.
            for (int sortedIndex = 0; sortedIndex < sortedStringToEncode.Length; sortedIndex++)
            for (int inputIndex = 0; inputIndex < stringToEncode.Length; inputIndex++)
            {
                if (sortedStringToEncode[sortedIndex] != stringToEncode[inputIndex]) continue;
                stringToEncode = stringToEncode
                    .Remove(inputIndex, 1)
                    .Insert(inputIndex, Convert.ToString((sortedIndex + 1) % 10));
                break;
            }

            return stringToEncode;
        }

        /// <summary>
        ///     Resize a digit series to a specific length.
        ///     Extending the series is done using a Lagged Fibonacci Generator.
        /// </summary>
        /// <exception cref="ArgumentException">LFG requires 2 or more digits to extend a digit series.</exception>
        public static string ResizeUsingLaggedFibonacciGenerator(string digitSeries, int intendedLength)
        {
            int iterLFG = 1;
            // Clip
            if (digitSeries.Length > intendedLength) return digitSeries[..intendedLength];

            if (digitSeries.Length < 2)
                throw new ArgumentException("LFG requires 2 or more digits to extend a digit series.");

            // Extend
            while (digitSeries.Length < intendedLength)
            {
                digitSeries += (CharUnicodeInfo.GetDecimalDigitValue(digitSeries[iterLFG - 1])
                                + CharUnicodeInfo.GetDecimalDigitValue(digitSeries[iterLFG]))
                               % 10;
                iterLFG++;
            }

            return digitSeries;
        }

        public enum IndividualDigitsModulus10Operation
        {
            Add,
            Subtract
        }
        
        /// <summary>
        ///     Sum the individual digits with matching indexes, then modulus 10 the result, preventing any elements outside
        ///     the 0-9 range.
        /// </summary>
        /// <returns></returns>
        public static string IndividualDigitsModulus10Calculation(string digitSeriesOne, string digitSeriesTwo,
            IndividualDigitsModulus10Operation operation)
        {
            string digitStringOne = digitSeriesOne;
            string digitStringTwo = digitSeriesTwo;
            if (digitStringOne.Length != digitStringTwo.Length)
                throw new ArgumentException("Both digit series must be the same length.");

            string resultDigitString = "";
            for (int i = 0; i < digitStringOne.Length; i++)
            {
                resultDigitString += operation switch
                {
                    IndividualDigitsModulus10Operation.Add => (CharUnicodeInfo.GetDecimalDigitValue(digitStringOne[i]) +
                                           CharUnicodeInfo.GetDecimalDigitValue(digitStringTwo[i])) % 10,

                    IndividualDigitsModulus10Operation.Subtract => ((CharUnicodeInfo.GetDecimalDigitValue(digitStringOne[i]) -
                                                 CharUnicodeInfo.GetDecimalDigitValue(digitStringTwo[i])) % 10 + 10) % 10,
                    _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
                };
            }

            return resultDigitString;
        }

        public static string ColumnarTransposition(string toTranspose, string keysDigitSeries)
        {
            // Attach the columns to the digit elements of the key.
            int[] keys = keysDigitSeries.Select(CharUnicodeInfo.GetDecimalDigitValue).ToArray();
            var matrix = new string[keys.Length];

            // In this loop, toTranspose string is viewed as a matrix by dividing it into rows with the length of the key.
            // The loop starts at the first element of each tKey's column, then iterates down each row.
            for (int keyIdx = 0; keyIdx < keys.Length; keyIdx++)
            for (int textDigitIdx = keyIdx; textDigitIdx < toTranspose.Length; textDigitIdx += keys.Length)
                matrix[keyIdx] += toTranspose[textDigitIdx];
            
            // Transpose matrix by sorting the columns by the transposition key's digits.
            for (int i = 0; i < keys.Length - 1; i++)
            for (int j = i + 1; j > 0; j--)
            {
                if (keys[j - 1] <= keys[j]) continue;
                
                // Sort the keys representing the matrix's column indexes.
                int buffer = keys[j - 1];
                keys[j - 1] = keys[j];
                keys[j] = buffer;
                
                // Sort the matrix's columns.
                string transpositionColumnBuffer = matrix[j - 1];
                matrix[j - 1] = matrix[j];
                matrix[j] = transpositionColumnBuffer;
            }
            
            return string.Join("", matrix);
        }
        
        private class TranspositionColumn
        {
            public int Key;
            public int ColumnLength;
            public string Text;
        }
        
        public static string ReverseColumnarTransposition(string toTranspose, string keyDigitSeries)
        {
            int[] key = keyDigitSeries.Select(CharUnicodeInfo.GetDigitValue).ToArray();

            // Get the initial transposition column with the column lengths determined by the length of toTranspose.
            TranspositionColumn[] GetColumnLengths(int[] tKey, string toTransposeBuffer)
            {
                var tMatrix = new TranspositionColumn[keyDigitSeries.Length];
                for (int i = 0; i < tMatrix.Length; i++)
                {
                    int currentColumnLength = (int)Math.Ceiling((double)toTransposeBuffer.Length / (keyDigitSeries.Length - i));
                    tMatrix[i] = new TranspositionColumn {Key = tKey[i], ColumnLength = currentColumnLength};
                    toTransposeBuffer = toTransposeBuffer.Remove(0, currentColumnLength);
                }

                return tMatrix;
            }
            TranspositionColumn[] matrix = GetColumnLengths(key, toTranspose);
            
            // Sort the columns by key digit sizes.
            TranspositionColumn[] SortColumns(TranspositionColumn[] tMatrix)
            {
                for (int i = 0; i < tMatrix.Length - 1; i++)
                for (int j = i + 1; j > 0; j--)
                {
                    if (tMatrix[j - 1].Key <= tMatrix[j].Key) continue;
                    TranspositionColumn columnBuffer = tMatrix[j - 1];
                    tMatrix[j - 1] = tMatrix[j];
                    tMatrix[j] = columnBuffer;
                }

                return tMatrix;
            }
            matrix = SortColumns(matrix);

            // Populate each key's columns.
            TranspositionColumn[] PopulateColumns(TranspositionColumn[] tMatrix, string toTransposeBuffer)
            {
                foreach (TranspositionColumn column in tMatrix)
                {
                    column.Text = toTransposeBuffer[..column.ColumnLength];
                    toTransposeBuffer = toTransposeBuffer.Remove(0, column.ColumnLength);
                }

                return tMatrix;
            }
            matrix = PopulateColumns(matrix, toTranspose);

            // Reverse the key sort on the key's columns.
            TranspositionColumn[] ReverseKeySort(TranspositionColumn[] tMatrix, int[] keyIter)
            {
                // Set the columns to the index positions determined by the keys.
                var reversedTranspositionMatrix = new TranspositionColumn[keyDigitSeries.Length];
                foreach (TranspositionColumn column in tMatrix)
                    for (int j = 0; j < keyIter.Length; j++)
                    {
                        if (column.Key != keyIter[j]) continue;
                        reversedTranspositionMatrix[j] = column;
                        keyIter[j] = -1;
                        break;
                    }

                return reversedTranspositionMatrix;
            }
            matrix = ReverseKeySort(matrix, key);

            // Transpose the matrix according to it's length.
            string TranposeMatrix(TranspositionColumn[] tMatrix)
            {
                string transposedMatrix = "";
                for (int y = 0; y < tMatrix[0].Text.Length; y++)
                for (int x = 0; x < tMatrix.Length; x++)
                {
                    if (y >= tMatrix[x].Text.Length)
                        break;
                    transposedMatrix += tMatrix[x].Text[y];
                }

                return transposedMatrix;
            }
            return TranposeMatrix(matrix);
        }
        public static int GetSerialInsertPositionFromComponentA(string componentA)
        {
            string[] aWords = componentA.Split(' ');

            // Subtract the first two word lengths, then absolute the result.
            int compCInsertionIndex = Math.Abs(aWords[0].Length - aWords[1].Length);

            // Sum the remaining word lengths.
            for (int idx = 2; idx < aWords.Length; idx++) compCInsertionIndex += aWords[idx].Length;

            return compCInsertionIndex;
        }
        
        /// <summary>
        ///     Converts a string of 13 characters or more into XY coordinates for a 10 by 3 key matrix.
        /// </summary>
        /// <param name="inputString">String of 13 characters or more.</param>
        public static (string, string) GetComponentADigitSeries(string inputString)
        {
            // Remove whitespace.
            inputString = inputString.Replace(" ", string.Empty);

            // Error checking: Requires 13 characters.
            if (inputString.Replace(" ", string.Empty).Length < 13 || inputString.Contains(' '))
                throw new ArgumentException("\"inputString\" requires 13 characters.");

            // RAI encode then convert to int array.
            string firstTenDigits = RelativeAlphabeticalIndexEncode(inputString[..10]);
            string remainingDigits = RelativeAlphabeticalIndexEncode(inputString[10..]);

            return (firstTenDigits, remainingDigits);
        }
    }
}