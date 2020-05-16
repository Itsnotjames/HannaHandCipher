using System;

namespace HannaHandCipher
{
    public static class HannaHandCipher
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter \"encrypt\" for encryption.");
                Console.WriteLine("Enter \"decrypt\" for decryption.");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "encrypt":
                        Encryption.Encrypt();
                        break;

                    case "decrypt":
                        Decryption.Decrypt();
                        break;

                    default:
                        Console.WriteLine("Invalid input.");
                        continue;
                }

                Console.ReadLine();
            }
        }
    }
}