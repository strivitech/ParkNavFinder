using System.Text;
using DataManager.Api.Common;

namespace DataManager.Api.Services;

public class GmailSubEmailsGenerator : IEmailGenerator
{
    private const string LowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
    private const string NumberChars = "1234567890";
    private const string GmailDomain = "@gmail.com";
    
    private const string AllChars = LowerCaseChars + NumberChars;
    
    public string Generate(string startsWith)
    {
        ArgumentException.ThrowIfNullOrEmpty(startsWith);
        
        var stringBuilder = new StringBuilder(startsWith);
        stringBuilder.Append('+');
        stringBuilder.Append(Constants.GeneratedEmailSharedKey);
        const int randomPartLength = 8;

        for (int i = 0; i < randomPartLength; i++)
        {
            int index = Random.Shared.Next(AllChars.Length);
            stringBuilder.Append(AllChars[index]);
        }

        stringBuilder.Append(GmailDomain);

        return stringBuilder.ToString();
    }
}