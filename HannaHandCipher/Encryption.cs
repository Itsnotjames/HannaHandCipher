using System;
using System.Linq;
using static HannaHandCipher.DigitSeriesOperations;
using static HannaHandCipher.UserInput;

namespace HannaHandCipher
{
    public class Encryption : SharedCipherSteps
    {
        public static void Encrypt()
        {
            var encryptionInput = new EncryptionUserInput();
            var intermediateSteps = new IntermediateSteps(encryptionInput);
            
            string ciphertext = EncryptPlaintext(encryptionInput, intermediateSteps);
            Console.WriteLine($"Ciphertext: {ciphertext}");
          
            string encryptedBookIndexPositions = EncryptBookIndexPositions(encryptionInput);
            // The hand cipher calls book index positions "book groups".
            Console.WriteLine($"First and second encrypted book groups: {encryptedBookIndexPositions}"); 
        }
        
        private class IntermediateSteps
        {
            public readonly StraddlingCheckerboardKeyMatrix StraddlingCheckerboardKeyMatrix;
            public readonly string ComponentI;
            public readonly string[] SplitComponentK;

            public IntermediateSteps (EncryptionUserInput encryptionUserInput)
            {
                // Create the key matrix with component A.
                string columnKeys;
                string rowKeys;
                StraddlingCheckerboardKeyMatrix CreateKeyMatrix ()
                {
                    (columnKeys, rowKeys) = GetComponentADigitSeries(encryptionUserInput.ComponentA);
                    return new StraddlingCheckerboardKeyMatrix(columnKeys, rowKeys[..3]);;
                }
                StraddlingCheckerboardKeyMatrix = CreateKeyMatrix();

                string componentD = CreateComponentD(encryptionUserInput.ComponentB, encryptionUserInput.ComponentC);
                
                string CreateComponentI (string compD)
                {
                    // Create component E using the key matrix keys and component D.
                    string kmKeysDigitString = columnKeys + rowKeys;
                    string componentE = IndividualDigitsModulus10Calculation(
                        kmKeysDigitString, 
                        ResizeUsingLaggedFibonacciGenerator(compD, kmKeysDigitString.Length),
                        IndividualDigitsModulus10Operation.Add);

                    // Create component I by using LFG to extend component E to five times it's length, trim off component E,
                    // then dividing the extension into four parts for the F-I components.
                    // The hand cipher defines components F,G,H, but they are unused.
                    int eLength = componentE.Length;
                    string componentsFGHI = ResizeUsingLaggedFibonacciGenerator(componentE, eLength * 5)[eLength..];
                    return (componentsFGHI[(eLength * 3)..]);
                }
                ComponentI = CreateComponentI(componentD);
                
                SplitComponentK = CreateSplitComponentK(encryptionUserInput.ComponentA, componentD);
            }
        }

        private static string EncryptPlaintext(EncryptionUserInput encryptionUserInput, IntermediateSteps intermediateSteps)
        {
            // Encode the plaintext with the key matrix.
            string encodedPlainText = StraddlingCheckerboardKeyMatrix.CharValuesToDigitKeys(encryptionUserInput.PlainText);

            // Modify the encoded plaintext by individually summing it's digits with component I's digits.
            string extendedI = ResizeUsingLaggedFibonacciGenerator(intermediateSteps.ComponentI, encodedPlainText.Length);
            string modifiedEncodedPlainText = IndividualDigitsModulus10Calculation(encodedPlainText, extendedI,
                IndividualDigitsModulus10Operation.Add);

            // Chain the transpositions "beginning to end" or "left to right" for each key within component K.
            string currentTranspositionState = intermediateSteps.SplitComponentK
                .Aggregate(modifiedEncodedPlainText, ColumnarTransposition);

            // Finally insert component C into the plaintext, then return the plaintext.
            int compCInsertionIndex = GetSerialInsertPositionFromComponentA(encryptionUserInput.ComponentA);
            return compCInsertionIndex > currentTranspositionState.Length
                ? currentTranspositionState + encryptionUserInput.ComponentC
                : currentTranspositionState.Insert(compCInsertionIndex, encryptionUserInput.ComponentC);
        }

        private static string EncryptBookIndexPositions(EncryptionUserInput encryptionUserInput)
        {
            // Encrypt component A's book position.
            string bWordLengths = string.Join("",
                encryptionUserInput.ComponentB.Split(" ").Select(word => word.Length).ToArray());
            const int formattedBookIndexPositionLength = 7; // The book index position format: "PPPLLCC" is 7 digits long.
            string bWordLengthsLFG = 
                ResizeUsingLaggedFibonacciGenerator(bWordLengths, formattedBookIndexPositionLength * 2);
            
            string combinedBookIndexPositions = encryptionUserInput.BookIndexPositions.Start.GetFormattedBookIndexPosition()
                                                + encryptionUserInput.BookIndexPositions.Start.GetFormattedBookIndexPosition();
            string bookIndexPositionsPlusB = 
                IndividualDigitsModulus10Calculation(combinedBookIndexPositions, bWordLengthsLFG, 
                    IndividualDigitsModulus10Operation.Add);
            
            return bookIndexPositionsPlusB.Insert(7, "-");
        }
    }
}