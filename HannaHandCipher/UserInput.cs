using System;
using System.Text.RegularExpressions;

namespace HannaHandCipher
{
    public static class UserInput
    {
        private static string InputPlainText()
        {
            Console.WriteLine("Enter the plaintext to encrypt.");
            string plainTextInput = RemoveNonWhitespaceAndAlphanumericCharacters(Console.ReadLine());
            while (plainTextInput.Length < 2)
            {
                Console.WriteLine("Plaintext must be larger than two characters.");
                plainTextInput = RemoveNonWhitespaceAndAlphanumericCharacters(Console.ReadLine());
            }

            return plainTextInput;
        }

        public class EncryptionUserInput
        {
            public readonly string PlainText;
            public readonly string ComponentA;
            public readonly BookIndexPositions BookIndexPositions;
            public readonly string ComponentB;
            public readonly string ComponentC;

            public EncryptionUserInput()
            {
                Console.WriteLine("This cipher uses three keys and a book to encrypt a single plaintext.");
                PlainText = InputPlainText();

                ComponentA = InputEncryptComponentA();

                BookIndexPositions = new BookIndexPositions();
                
                Console.WriteLine("For key B, enter at-least two words.");
                ComponentB = InputComponentB();

                Console.WriteLine("Enter key C. This key must be only made up of 5 digits.");
                Console.WriteLine("For key C, enter 5 digits.");
                ComponentC = InputComponentC();
            }
        }

        public class DecryptionUserInput
        {
            public readonly string EncryptedPlainText;
            public readonly string ComponentB;
            public readonly string EncryptedBookGroupStart;
            public readonly string EncryptedBookGroupEnd;

            public DecryptionUserInput()
            {
                Console.WriteLine("This cipher uses one key, two encrypted book indexes called \"book groups\", " +
                                  "and the corresponding book to decrypt a single plaintext.");

                EncryptedPlainText = InputEncryptedPlainText();

                Console.WriteLine("Enter key B.");
                ComponentB = InputComponentB();

                Console.WriteLine("Enter the first encrypted book group.");
                EncryptedBookGroupStart = InputEncryptedBookGroup();

                Console.WriteLine("Enter the second encrypted book group.");
                EncryptedBookGroupEnd = InputEncryptedBookGroup();
            }
        }

        private static string InputEncryptedPlainText()
        {
            Console.WriteLine("Enter the encrypted plaintext to decrypt.");
            string encryptedPlainTextInput;
            while (true)
            {
                encryptedPlainTextInput =
                    RemoveNonWhitespaceAndAlphanumericCharacters(Console.ReadLine());
                
                var nonDigit = new Regex(@"[^0-9]");
                if (nonDigit.IsMatch(encryptedPlainTextInput))
                {
                    Console.WriteLine("Encrypted plaintext must contain only digits.");
                    continue;
                }
                
                if (encryptedPlainTextInput.Length < 6)
                {
                    Console.WriteLine("Encrypted plaintext must contain at-least six characters.");
                    continue;
                }
                
                break;
            }

            return encryptedPlainTextInput;
        }

        private static string InputEncryptComponentA()
        {
            Console.WriteLine("For key A, enter a selection of text in the book that contains at-least 13 " +
                              "alphabetic characters, and at-least two words.");
            
            return InputComponentA();
        }
        
        public static string InputDecryptComponentA(string startBookIndexPosition, string endBookIndexPosition)
        {
            Console.WriteLine("Enter component A, the text found in the book using the following book groups:");
            Console.WriteLine("The first character of component A is found on " +
                              $"the page index:{startBookIndexPosition[..3]}, " +
                              $"the row index:{startBookIndexPosition[3..5]}, and " +
                              $"the character index:{startBookIndexPosition[5..7]}");
            Console.WriteLine("The last character of component A is found on " +
                              $"the page index:{endBookIndexPosition[..3]}, " +
                              $"the row index:{endBookIndexPosition[3..5]}, and " +
                              $"the character index:{endBookIndexPosition[5..7]}");
            
            return InputComponentA();
        }
        
        private static string InputComponentA()
        {
            while (true)
            {
                string componentAInput =
                    RemoveNonWhitespaceAndAlphanumericCharacters(Console.ReadLine());
                string inputWithoutDigits = Regex.Replace(componentAInput, @"[\d-]", string.Empty);

                string inputWithoutWhitespace = inputWithoutDigits.Replace(" ", "");
                if (inputWithoutWhitespace.Length < 13)
                {
                    Console.WriteLine(
                        "The text selection for component A must contain at-least 13 alphabetic characters");
                    continue;
                }

                var wordSeparation = new Regex("\\b\\s\\b");
                if (!wordSeparation.IsMatch(inputWithoutDigits))
                {
                    Console.WriteLine("The text selection for component A must contain at-least two words.");
                    continue;
                }

                return inputWithoutDigits;
            }
        }

        private static string InputComponentB()
        {
            while (true)
            {
                string componentBInput =
                    RemoveNonWhitespaceAndAlphanumericCharacters(Console.ReadLine());

                var wordSeparation = new Regex("\\b\\s\\b");
                if (!wordSeparation.IsMatch(componentBInput))
                {
                    Console.WriteLine("Component B must contain at-least two words.");
                    continue;
                }

                return componentBInput;
            }
        }

        private static string InputComponentC()
        {
            while (true)
            {
                string componentCInput = Console.ReadLine();

                if (componentCInput.Length != 5)
                {
                    Console.WriteLine("Component C must be 5 digit characters long.");
                    continue;
                }

                bool componentCIsDigits = int.TryParse(componentCInput, out _);
                if (!componentCIsDigits)
                {
                    Console.WriteLine("Component C must be only made up of digits.");
                    continue;
                }


                return componentCInput;
            }
        }

        private static string InputEncryptedBookGroup()
        {
            while (true)
            {
                string componentBInput = Console.ReadLine();

                if (!int.TryParse(componentBInput, out _))
                {
                    Console.WriteLine("The encrypted book group must contain only digits.");
                    continue;
                }

                if (componentBInput.Length != 7)
                {
                    Console.WriteLine("The encrypted book group must be 7 digit characters long.");
                    continue;
                }

                return componentBInput;
            }
        }

        private static string RemoveNonWhitespaceAndAlphanumericCharacters(string str)
        {
            var nonWhitespaceAndAlphanumericRegex = new Regex("[^a-zA-Z0-9( )]");
            str = nonWhitespaceAndAlphanumericRegex.Replace(str, "");
            return str;
        }
    }
}