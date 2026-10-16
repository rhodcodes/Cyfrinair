using System.Security.Cryptography;
using System.Text;

namespace Cyfrinair.Core;

public class Password(PasswordOptions options)
{
    public const string BaseCharacterSet = "abcdefghjkmnpqrtuvwxyzACDEFGHIJKMNPQRTUVWXYZ";
    public const string DigitSet = "234679";
    public const string SymbolSet = "!@#$%^&*()+[]{}<>?";
    public const string AmbiguousChars = "ilLoOSsB";
    public const string AmbiguousSymbols = "-_.,|";
    public const string AmbiguousDigits = "0158";
    private readonly PasswordOptions _options = options ?? throw new ArgumentNullException(nameof(options));

    public string Generate()
    {
        return CreatePassword();
    }

    private string CreatePassword()
    {
        string availableCharacters = BaseCharacterSet;
        StringBuilder result = new(_options.Length);
        byte[] bytes = new byte[_options.Length];

        if (_options.Length < 1)
        {
            throw new ArgumentException("Password length must be at least 1 character.", nameof(_options.Length));
        }

        if (_options.IncludeSymbols)
        {
            availableCharacters += SymbolSet;
        }

        if (_options.IncludeDigits)
        {
            availableCharacters += DigitSet;
        }

        if (_options.IncludeAmbiguousChars)
        {
            availableCharacters += AmbiguousChars;
        }

        if (_options is { IncludeSymbols: true, IncludeAmbiguousChars: true })
        {
            availableCharacters += AmbiguousSymbols;
        }

        if (_options is { IncludeDigits: true, IncludeAmbiguousChars: true })
        {
            availableCharacters += AmbiguousDigits;
        }

        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        for (int i = 0; i < _options.Length; i++)
        {
            int index = bytes[i] % availableCharacters.Length;
            result.Append(availableCharacters[index]);
        }

        return result.ToString();
    }
}