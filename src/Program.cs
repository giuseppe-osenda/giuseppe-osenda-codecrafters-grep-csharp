using System;
using System.IO;

static bool MatchPattern(string inputLine, string pattern)
{
    if (pattern.Length == 1)
    {
        return inputLine.Contains(pattern);
    }
 
  
    var isPositiveCharactersGroupPattern = pattern.Contains('[') && pattern.Contains(']');
    var isNegativeCharactersGroupPattern = pattern.Contains("[^") && pattern.Contains(']');
    var otherPatterns = pattern.Contains(@"\d") || pattern.Contains(@"\w");
    
    if (isPositiveCharactersGroupPattern || isNegativeCharactersGroupPattern)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(inputLine, pattern);
    }
    
    if(!otherPatterns)
        throw new ArgumentException($"Unhandled pattern: {pattern}");

    var charIndex = false;
    var offSet = 0;
    
    for (var pos = 0; pos < pattern.Length; pos++)
    {
        //search for \d or \w or a number or a digit if found search for at least a digit or a number in inputLine
        //if found continue to parse the pattern, if next i have a \d or \w or ' ' or a digit or a char search for a digit or char or whitespace starting from the previous found index
        var isDigitPattern = pattern[pos] == '\\' && pattern[pos + 1] == 'd';
        var isCharPattern = pattern[pos] == '\\' && pattern[pos + 1] == 'w';
        var isWhiteSpace = pattern[pos] == ' ';
        var isChar = false;
        var isDigit = false;
        
        if (!isDigitPattern && !isCharPattern && !isWhiteSpace)
        {
             isDigit = char.IsDigit(pattern[pos]);
             isChar = char.IsLetter(pattern[pos]);
        }

        if (isDigitPattern)
        {
            Console.WriteLine($"digit pattern at pos {pos} and {pos + 1}");
            
            charIndex = false;
            for (int i = ++offSet; i < inputLine.Length; i++)
            {
                Console.WriteLine($"start searching digit from pos {i}");
                if (char.IsDigit(inputLine[i]))
                {
                    charIndex = true;
                    offSet = i;
                    pos += 1;
                    Console.WriteLine($"digit found at pos {offSet}");
                    break;
                }
            }
            
            if (charIndex == false)
                return false;
            
            continue;

        }

        if (isCharPattern)
        {
            Console.WriteLine($"char pattern at pos {pos} and {pos + 1}");
            charIndex = false;
            for (int i = ++offSet; i < inputLine.Length; i++)
            {
                Console.WriteLine($"start searching char from pos {i}");
                if (char.IsLetter(inputLine[i]))
                {
                    charIndex = true;
                    offSet = i;
                    pos += 1;
                    Console.WriteLine($"char found at pos {offSet}");
                    break;
                }
            }
            
            if (charIndex == false)
                return false;
            
            continue;

        }

        if (isWhiteSpace)
        {
            Console.WriteLine($"whitespace at pos {pos}");
            charIndex = false;
            for (int i = ++offSet; i < inputLine.Length; i++)
            {
                Console.WriteLine($"start searching whitespace from pos {i}");
                if (char.IsWhiteSpace(inputLine[i]))
                {
                    charIndex = true;
                    offSet = i;
                    Console.WriteLine($"whitespace found at pos {offSet}");
                    break;
                }
            }
            
            if (charIndex == false)
                return false;
            
            continue;

        }

        if (isDigit || isChar)
        {
            
            Console.WriteLine($"explicit digit or char found at pos {pos} of pattern");

            var isLast = inputLine.IndexOf(inputLine.FirstOrDefault(pattern[pos])).Equals(inputLine.Length);

            if (isLast)
            {
                return inputLine.Last().Equals(pattern[pos]);
            }
            
            charIndex = false;
            for (int i = ++offSet; i < inputLine.Length; i++)
            {
                Console.WriteLine($"start searching from pos {i}");
                if (inputLine[i] == pattern[pos])
                {
                    charIndex = true;
                    offSet = i;
                    Console.WriteLine($"found explicit digit or char at pos {offSet}");
                    break;
                }
            }
            
            if (charIndex == false)
                return false;
            
        }
    }

    return true;
}

if (args[0] != "-E")
{
    Console.WriteLine("Expected first argument to be '-E'");
    Environment.Exit(2);
}

string pattern = args[1];
string inputLine = Console.In.ReadToEnd();

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");


if (MatchPattern(inputLine, pattern))
{
    Environment.Exit(0);
}
else
{
    Environment.Exit(1);
}