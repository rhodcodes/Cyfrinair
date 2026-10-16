using System.Security.Cryptography;
using System.Text;

namespace Cyfrinair.Core;

public class Pin
{
    public const string DigitSet = "0123456789";

    public string Generate(int length)
    {
        return CreatePin(length);
    }

    private string CreatePin(int length)
    {
        if (length < 1) throw new ArgumentException("Length cannot be less than 1.", nameof(length));
        
        string availableCharacters = DigitSet;
        StringBuilder result = new(length);
        byte[] bytes = new byte[length];

        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        for (int i = 0; i < length; i++)
        {
            int index = bytes[i] % availableCharacters.Length;
            result.Append(availableCharacters[index]);
        }
        
        return result.ToString();
    }
}