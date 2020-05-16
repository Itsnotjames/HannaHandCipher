using System;
using HannaHandCipher;

namespace HannaHandCipher
{
    /// <summary>
    ///     A character's index position in a book in the 7 digit format:
    ///     page 000-999 (P), line 00-99 (L), character 00-99 (C)
    ///     "PPPLLCC"
    /// </summary>
    public class BookIndexPosition
    {
        public int Character;
        public int Line;
        public int Page;

        public string GetFormattedBookIndexPosition()
        {
            // Page position leading zeroes.
            string pageFormatted;
            if (Page > 99)
                pageFormatted = Page.ToString();
            else if (Page > 9)
                pageFormatted = "0" + Page;
            else
                pageFormatted = "00" + Page;

            // Line position leading zeroes.
            string lineFormatted;
            if (Line > 9)
                lineFormatted = Line.ToString();
            else
                lineFormatted = "0" + Line;

            // Character position leading zeroes.
            string characterFormatted;
            if (Line > 9)
                characterFormatted = Character.ToString();
            else
                characterFormatted = "0" + Character;

            return pageFormatted + lineFormatted + characterFormatted;
        }
    }
}

public class BookIndexPositions
{
    public readonly BookIndexPosition End;
    public readonly BookIndexPosition Start;

    public BookIndexPositions()
    {
        while (true)
        {
            Console.WriteLine("Enter the page, line, and character index position of the first character in the selected text.");
            Start = new BookIndexPosition
            {
                Page = InputBookPageIndex(),
                Line = InputBookLineIndex(),
                Character = InputBookCharacterIndex()
            };

            Console.WriteLine("Enter the page, line, and character index position of the last character in the selected text.");
            End = new BookIndexPosition
            {
                Page = InputBookPageIndex(),
                Line = InputBookLineIndex(),
                Character = InputBookCharacterIndex()
            };
            if (Start.Page > End.Page)
            {
                Console.WriteLine("The first character position must be before the last character position.");
                continue;
            }

            if (Start.Line > End.Line && Start.Page == End.Page)
            {
                Console.WriteLine("The first character position must be before the last character position.");
                continue;
            }

            if (Start.Character > End.Character && Start.Line == End.Line && Start.Page == End.Page)
            {
                Console.WriteLine("The first character position must be before the last character position.");
                continue;
            }

            break;
        }
    }

    private static int InputBookPageIndex()
    {
        Console.WriteLine("Enter the page index between 1-999:");
        while (true)
        {
            string inputStr = Console.ReadLine();
            short inputNum;
            if (short.TryParse(inputStr, out short parsedInput))
            {
                inputNum = parsedInput;
            }
            else
            {
                Console.WriteLine("The page index must be a number.");
                continue;
            }

            if (inputNum > 999 || inputNum < 1)
            {
                Console.WriteLine("The page index must be between 1 and 999");
                continue;
            }

            return inputNum;
        }
    }

    private static int InputBookLineIndex()
    {
        Console.WriteLine("Enter the line index between 1-99:");
        while (true)
        {
            string inputStr = Console.ReadLine();
            short inputNum;
            if (short.TryParse(inputStr, out short parsedInput))
            {
                inputNum = parsedInput;
            }
            else
            {
                Console.WriteLine("The line index must be a number.");
                continue;
            }

            if (inputNum > 99 || inputNum < 1)
            {
                Console.WriteLine("The line index must be between 1 and 99");
                continue;
            }

            return inputNum;
        }
    }

    private static int InputBookCharacterIndex()
    {
        Console.WriteLine("Enter the character index between 1-99:");
        while (true)
        {
            string inputStr = Console.ReadLine();
            short inputNum;
            if (short.TryParse(inputStr, out short parsedInput))
            {
                inputNum = parsedInput;
            }
            else
            {
                Console.WriteLine("The character index must be a number.");
                continue;
            }

            if (inputNum > 99 || inputNum < 1)
            {
                Console.WriteLine("The character index must be between 1 and 99");
                continue;
            }

            return inputNum;
        }
    }
}