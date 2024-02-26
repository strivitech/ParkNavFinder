namespace DataManager.Api.Services;

public class PasswordGenerator : IPasswordGenerator
{
    private const string LowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
    private const string UpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string NumberChars = "1234567890";
    private const string SymbolChars = "!@#$%^&*";
    private const int MinLength = 8; // Can't be less than 4

    private const string AllChars = LowerCaseChars + UpperCaseChars + NumberChars + SymbolChars;

    public string Generate(int length = MinLength)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(length, MinLength);
            
        char[] passwordChars = new char[length];
        passwordChars[0] = GetRandomChar(LowerCaseChars);
        passwordChars[1] = GetRandomChar(UpperCaseChars);
        passwordChars[2] = GetRandomChar(NumberChars);
        passwordChars[3] = GetRandomChar(SymbolChars);
        
        for (int i = 4; i < length; i++)
        {
            passwordChars[i] = GetRandomChar(AllChars);
        }
        
        Random.Shared.Shuffle(passwordChars);

        return new string(passwordChars);
    }

    private static char GetRandomChar(string chars)
    {
        int index = Random.Shared.Next(chars.Length);
        return chars[index];
    }
}