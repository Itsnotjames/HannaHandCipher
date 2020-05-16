using System.Collections.Generic;
using System.Linq;
using static HannaHandCipher.DigitSeriesOperations;

namespace HannaHandCipher
{
    public abstract class SharedCipherSteps
    {
        protected static string[] CreateSplitComponentK(string componentA, string ComponentD)
        {
            // Create component J.
            string[] aWords = componentA.Split(' ');
            string wordBuffer = "";
            var aCompressedWords = new List<string>();
            // Words must be at-least 4 characters long, else concatenate with the next word.
            foreach (string word in aWords)
                if (word.Length < 4)
                {
                    wordBuffer += word;
                    if (wordBuffer.Length < 4) continue;
                    aCompressedWords.Add(wordBuffer);
                    wordBuffer = "";
                }
                else
                {
                    aCompressedWords.Add(wordBuffer + word);
                    wordBuffer = "";
                }

            string componentJ = aCompressedWords
                .Aggregate("", (current, word) => current + RelativeAlphabeticalIndexEncode(word));

            // Create component K.
            string componentK = IndividualDigitsModulus10Calculation(
                componentJ,
                ResizeUsingLaggedFibonacciGenerator(ComponentD, componentJ.Length),
                DigitSeriesOperations.IndividualDigitsModulus10Operation.Add);

            // Create transposition keys from component K cut to the sizes from component A's compressed word's lengths.
            var splitComponentK = new string[aCompressedWords.Count];
            int aWordStartIdx = 0;
            for (int i = 0; i < aCompressedWords.Count; i++)
            {
                int wordLength = aCompressedWords[i].Length;
                splitComponentK[i] = componentK[aWordStartIdx..(aWordStartIdx + wordLength)];
                aWordStartIdx += wordLength;
            }
            
            return splitComponentK;
        }

        protected static string CreateComponentD (string componentB, string componentC)
        {
            // Using the key matrix, encode a copy of component B with the whitespace removed.
            string componentBNoWhitespace = componentB.Replace(" ", string.Empty);
            string componentBEncoded = StraddlingCheckerboardKeyMatrix.CharValuesToDigitKeys(componentBNoWhitespace);

            // Using the LFG, resize the component C digit series to the length of the encoded component B digit series. 
            string resizedC = ResizeUsingLaggedFibonacciGenerator(componentC, componentBEncoded.Length);

            // Create component D using the two digit series derived from component B and C.
            return IndividualDigitsModulus10Calculation(componentBEncoded, resizedC,
                IndividualDigitsModulus10Operation.Add);
        }

    }
}