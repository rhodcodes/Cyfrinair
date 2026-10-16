using System.Globalization;
using System.Security.Cryptography;

namespace Cyfrinair.Core;

public partial class Passphrase(PassphraseOptions options)
{
    private readonly PassphraseOptions _options = options ?? throw new ArgumentNullException(nameof(options));

    private readonly TextInfo _textInfo = new CultureInfo("cy-GB").TextInfo;

    public string Generate()
    {
        return CreatePassphrase();
    }

    private string CreatePassphrase()
    {
        if (_options.Words < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(_options.Words), "The number of words must be at least 1.");
        }

        if (char.IsAscii(_options.Separator) == false || _options.Separator == '\0')
        {
            throw new ArgumentException("The separator must be an ASCII character.", nameof(_options.Separator));
        }

        string[] candidates = new string[_options.Words];

        for (int i = 0; i < _options.Words; i++)
        {
            int r = RandomNumberGenerator.GetInt32(_wordList.Length);
            candidates[i] = _wordList[r];
        }

        if (_options.Casing == CasingOption.Upper)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i] = _textInfo.ToTitleCase(candidates[i]);
            }
        }

        if (_options.Casing == CasingOption.Lower)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i] = _textInfo.ToLower(candidates[i]);
            }
        }

        if (_options.Casing == CasingOption.Random)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                int r = RandomNumberGenerator.GetInt32(2);
                if (r == 0)
                {
                    candidates[i] = _textInfo.ToTitleCase(candidates[i]);
                }
                else
                {
                    candidates[i] = _textInfo.ToLower(candidates[i]);
                }
            }
        }

        // Add digits

        if (_options.DigitPlacement == DigitPlacementOption.Once)
        {
            int index = RandomNumberGenerator.GetInt32(candidates.Length);
            candidates[index] = AppendRandomNumber(candidates[index]);
        }

        if (_options.DigitPlacement == DigitPlacementOption.EveryWord)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i] = AppendRandomNumber(candidates[i]);
            }
        }

        // Add separator
        string passphrase = string.Join(_options.Separator, candidates);

        return passphrase;
    }

    private string AppendRandomNumber(string candidate)
    {
        return $"{candidate}{RandomNumberGenerator.GetInt32(10)}";
    }
}