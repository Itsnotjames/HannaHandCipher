using System;
using System.Linq;
using static HannaHandCipher.DigitSeriesOperations;
using static HannaHandCipher.UserInput;

namespace HannaHandCipher
{
    public class Decryption : SharedCipherSteps
    {
        public static void Decrypt()
        {
            var userInput = new DecryptionUserInput();
            var intermediateSteps = new DecryptIntermediateSteps(userInput);
            string decryptedPlainText = DecryptPlainText(userInput, intermediateSteps);
            
            Console.WriteLine("Decrypted plaintext:");
            Console.WriteLine(decryptedPlainText);
        }
        
        private class DecryptIntermediateSteps
        {
            public readonly string ColumnKeys;
            public readonly string RowKeys;
            public readonly string ComponentD;
            public readonly string[] SplitComponentK;
            public readonly StraddlingCheckerboardKeyMatrix StraddlingCheckerboardKeyMatrix;
            public readonly int ComponentCInsertionIndex;

            public DecryptIntermediateSteps(DecryptionUserInput userInput)
            {
                (string, string) DecryptBookIndexPositions()
                {
                    string combinedEncryptedBookIndexPositions =
                        userInput.EncryptedBookGroupStart + userInput.EncryptedBookGroupEnd;

                    // Get a digit series of the word lengths in B.
                    string componentBWordLengths = userInput.ComponentB
                        .Split(' ')
                        .Aggregate("", (current, word) => current + word.Length);

                    // Resize component B's word lengths to the combined encrypted book index positions using the LFG.
                    string resizedComponentBWordLengths = 
                        ResizeUsingLaggedFibonacciGenerator(componentBWordLengths, combinedEncryptedBookIndexPositions.Length);

                    // Decrypt the book index positions by subtracting component B's resized word lengths.
                    string decryptedCombinedBookIndexPositions = IndividualDigitsModulus10Calculation(
                        combinedEncryptedBookIndexPositions,
                        resizedComponentBWordLengths,
                        IndividualDigitsModulus10Operation.Subtract);

                    return (decryptedCombinedBookIndexPositions[..7], decryptedCombinedBookIndexPositions[7..14]);
                }
                (string startBookIndexPosition, string endBookIndexPosition) = DecryptBookIndexPositions();
                
                // User finds and inputs component A found the book with the decrypted book index positions.
                string componentA = InputDecryptComponentA(startBookIndexPosition, endBookIndexPosition);

                // Use component A to find the position of the 5 digit serial: component C.
                ComponentCInsertionIndex = GetSerialInsertPositionFromComponentA(componentA);

                // Use component C's position to find it inside the encrypted plaintext.
                string componentC = userInput.EncryptedPlainText[ComponentCInsertionIndex..(ComponentCInsertionIndex + 5)];

                // Create the key matrix with the key matrix keys.
                (ColumnKeys, RowKeys) = GetComponentADigitSeries(componentA);
                StraddlingCheckerboardKeyMatrix = new StraddlingCheckerboardKeyMatrix(ColumnKeys[..10], RowKeys[..3]);

                ComponentD = CreateComponentD(userInput.ComponentB, componentC);

                SplitComponentK = CreateSplitComponentK(componentA, ComponentD);
            }
        }

        private static string DecryptPlainText(DecryptionUserInput userInput, DecryptIntermediateSteps intermediateSteps)
        {
            // Remove component C from the ciphertext.
            string encryptedPlainTextWithoutComponentC = 
                userInput.EncryptedPlainText.Remove(intermediateSteps.ComponentCInsertionIndex, 5);

            // Chain the transpositions in reverse: "end to beginning" or "right to left" for each key within component K.
            string currentTranspositionState = encryptedPlainTextWithoutComponentC;
            for (int index = intermediateSteps.SplitComponentK.Length - 1; index >= 0; index--)
            {
                string tKey = intermediateSteps.SplitComponentK[index];
                currentTranspositionState =
                    ReverseColumnarTransposition(currentTranspositionState, tKey);
            }

            // Create component E using the key matrix keys and component D.
            string kmKeysDigitString = intermediateSteps.ColumnKeys + intermediateSteps.RowKeys;
            string componentE = IndividualDigitsModulus10Calculation(
                kmKeysDigitString,
                ResizeUsingLaggedFibonacciGenerator(intermediateSteps.ComponentD, kmKeysDigitString.Length),
                IndividualDigitsModulus10Operation.Add
            );

            // Create component I by using LFG to extend component E to five times it's length, trim off component E,
            // then dividing the extension into four parts for the F-I components.
            // The hand cipher defines components F,G,H, but they are unused.
            int eLength = componentE.Length;
            string componentsFGHI = ResizeUsingLaggedFibonacciGenerator(componentE, eLength * 5)[eLength..];
            string componentI = componentsFGHI[(eLength * 3)..];

            // Modify the encoded plaintext by individually subtracting it's digits with component I's digits.
            string extendedI = ResizeUsingLaggedFibonacciGenerator(componentI, currentTranspositionState.Length);
            currentTranspositionState = IndividualDigitsModulus10Calculation(
                currentTranspositionState,
                extendedI,
                IndividualDigitsModulus10Operation.Subtract);

            // Reverse encode the plaintext with the key matrix.
            return intermediateSteps.StraddlingCheckerboardKeyMatrix.DigitKeysToCharValues(currentTranspositionState);
        }
    }
}