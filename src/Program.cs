using System;
using System.IO;

static bool MatchPattern(string inputLine, string pattern)
{
    var isPositiveCharactersGroupPattern = pattern.Contains('[') && pattern.Contains(']');
    var isNegativeCharactersGroupPattern = pattern.Contains("[^") && pattern.Contains(']');
    
    if (pattern is @"\d" or @"\w" || isPositiveCharactersGroupPattern || isNegativeCharactersGroupPattern)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(inputLine, pattern);
    }

    if (pattern.Length == 1)
    {
        return inputLine.Contains(pattern);
    }

    throw new ArgumentException($"Unhandled pattern: {pattern}");
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

