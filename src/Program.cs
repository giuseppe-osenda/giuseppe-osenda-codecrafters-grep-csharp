static bool OneOrMorePatternMatch(string inputLine, string pattern)
{
    var needles = pattern.Split('+');

    if (needles.Length == 1)
    {
        return inputLine.Contains(needles[0]); //what precede '+' is mandatory 
    }

    foreach (var needle in needles)
    {
        if (!inputLine.Contains(needle)) //all needles are mandatory
            return false;
    }

    return true;
}

//an? Matches an 'a' followed by zero or one 'n' character.
static bool OneOrZeroPatternMatch(string inputLine, string pattern)
{
    var needles = pattern.Split('?');

    if (needles.Length == 1)
    {
        var operatorIndex = pattern.IndexOf('?');

        switch (operatorIndex)
        {
            case -1:
                return false;
            case <= 1: //0 mean there is only '?' and 1 mean it's optional
                return true;
            default:
            {
                var needle = pattern.Substring(0, operatorIndex - 1); //taking only mandatory part

                return inputLine.Contains(needle);
            }
        }
    }

    foreach (var needle in needles) //if i have multiple needles
    {
        if (needle == needles.Last()) //the last is the only one not precedeed by the '?' then it's all mandatory
        {
            return inputLine.Contains(needle);
        }

        if (needle.Length == 1)
        {
            //totally optional   
        }
        else
        {
            var mandatory = pattern.Substring(0, needle.Length - 1); //taking only mandatory part

            if (!inputLine.Contains(mandatory))
                return false;
        }
    }

    return false;
}

static bool AnyCharPattern(string inputLine, string pattern)
{
    var needles = pattern.Split('.');


    foreach (var needle in needles) //if i have multiple needles
    {
        if (!inputLine.Contains(needle))
            return false;
    }

    return true;
}

static bool MultiplePattern(string inputLine, string pattern)
{
    pattern = pattern.Substring(pattern.IndexOf('(')).Trim('(', ')');
    
    Console.WriteLine(pattern);

    var needles = pattern.Split('|');
    
    
    foreach (var needle in needles)
    {
        if (inputLine.Contains(needle))
            return true;
    }

    return false;
}

static bool BackreferencePattern(string inputLine, string pattern)
{
    pattern = pattern.Trim('(', ')');

    return true;
}

static bool MatchPattern(string inputLine, string pattern)
{
    if (pattern.Length == 1)
    {
        return inputLine.Contains(pattern);
    }


    var isPositiveCharactersGroupPattern = pattern.Contains('[') && pattern.Contains(']');
    var isNegativeCharactersGroupPattern = pattern.Contains("[^") && pattern.Contains(']');
    var otherPatterns = pattern.Contains(@"\d") || pattern.Contains(@"\w");
    var isAnchorPattern = pattern.Contains('^') && !pattern.Contains('[') && !pattern.Contains(']');
    var isEndOfStringPattern = pattern.Contains('$');
    var isOneOrMorePattern = pattern.Contains('+');
    var isOneOrZeroPattern = pattern.Contains('?');
    var isAnyCharPattern = pattern.Contains('.');
    var isMultiplePattern = pattern.Contains('|');
    var isBackreferencePattern = System.Text.RegularExpressions.Regex.IsMatch(pattern, @"\\\d");
    
    if (isPositiveCharactersGroupPattern || isNegativeCharactersGroupPattern || isAnchorPattern || isEndOfStringPattern || isBackreferencePattern)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(inputLine, pattern);
    }

    if (isOneOrMorePattern)
        return OneOrMorePatternMatch(inputLine, pattern);

    if (isOneOrZeroPattern)
        return OneOrZeroPatternMatch(inputLine, pattern);
    
    if (isAnyCharPattern)
        return AnyCharPattern(inputLine, pattern);
    
    if (isMultiplePattern)
        return MultiplePattern(inputLine, pattern);

    if (isBackreferencePattern)
    {
        return BackreferencePattern(inputLine, pattern);
    }
    if (!otherPatterns)
        throw new ArgumentException($"Unhandled pattern: {pattern}");
    
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

        bool charIndex;
        if (isDigitPattern)
        {
            //Console.WriteLine($"digit pattern at pos {pos} and {pos + 1}");

            charIndex = false;
            for (var i = ++offSet; i < inputLine.Length; i++)
            {
                //Console.WriteLine($"start searching digit from pos {i}");
                if (!char.IsDigit(inputLine[i])) continue;

                charIndex = true;
                offSet = i;
                pos += 1;
                //Console.WriteLine($"digit found at pos {offSet}");
                break;
            }

            if (charIndex == false)
                return false;

            continue;
        }

        if (isCharPattern)
        {
            //Console.WriteLine($"char pattern at pos {pos} and {pos + 1}");
            charIndex = false;
            for (var i = ++offSet; i < inputLine.Length; i++)
            {
                //Console.WriteLine($"start searching char from pos {i}");
                if (!char.IsLetter(inputLine[i])) continue;

                charIndex = true;
                offSet = i;
                pos += 1;
                //Console.WriteLine($"char found at pos {offSet}");
                break;
            }

            if (charIndex == false)
                return false;

            continue;
        }

        if (isWhiteSpace)
        {
            //Console.WriteLine($"whitespace at pos {pos}");
            charIndex = false;
            for (var i = ++offSet; i < inputLine.Length; i++)
            {
                //Console.WriteLine($"start searching whitespace from pos {i}");
                if (!char.IsWhiteSpace(inputLine[i])) continue;

                charIndex = true;
                offSet = i;
                //Console.WriteLine($"whitespace found at pos {offSet}");
                break;
            }

            if (charIndex == false)
                return false;

            continue;
        }

        if (isDigit || isChar)
        {
            //Console.WriteLine($"explicit digit or char found at pos {pos} of pattern");

            var isLast = inputLine.IndexOf(inputLine.FirstOrDefault(pattern[pos])).Equals(inputLine.Length);

            if (isLast)
            {
                return inputLine.Last().Equals(pattern[pos]);
            }

            charIndex = false;
            for (var i = ++offSet; i < inputLine.Length; i++)
            {
                //Console.WriteLine($"start searching from pos {i}");
                if (inputLine[i] != pattern[pos]) continue;

                charIndex = true;
                offSet = i;
                //Console.WriteLine($"found explicit digit or char at pos {offSet}");
                break;
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