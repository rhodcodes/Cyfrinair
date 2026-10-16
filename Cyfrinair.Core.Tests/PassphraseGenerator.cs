using System.Text.RegularExpressions;

namespace Cyfrinair.Core.Tests;

public partial class PassphraseGenerator
{
    [Test]
    public async Task ThrowsArgumentNullException_GivenNullOptions()
    {
        // Arrange
        PassphraseOptions options = null!;

        // Act & Assert
        await Assert.That(() => { new Passphrase(options); })
            .Throws<ArgumentNullException>();
    }

    [Test]
    [Arguments(3)]
    [Arguments(4)]
    [Arguments(5)]
    [Arguments(6)]
    [Arguments(7)]
    public async Task ReturnsMultipleDistinctPassphrases_GivenMultipleCount(ushort n)
    {
        // Arrange
        PassphraseOptions options = new();
        Passphrase generator = new(options);
        List<string> results = new(n);

        // Act
        for (var i = 0; i < n; i++)
        {
            results.Add(generator.Generate());
        }

        // Assert
        await Assert.That(results).Count().IsEqualTo(n).And.HasDistinctItems();
    }

    [Test]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(6)]
    [Arguments(8)]
    [Arguments(10)]
    [Arguments(12)]
    [Arguments(14)]
    public async Task ReturnsNWords_GivenNWordsOption(int n)
    {
        // Arrange
        PassphraseOptions options = new() { Words = n };
        Passphrase generator = new(options);

        // Act
        var result = generator.Generate();

        // Assert
        await Assert.That(result.Split('-')).Count().IsEqualTo(n);
    }

    [Test]
    public async Task ReturnsCorrectSeparator_GivenSeparatorOption()
    {
        // Arrange
        PassphraseOptions options = new() { Separator = '>' };
        Passphrase generator = new(options);

        // Act
        var result = generator.Generate();

        // Assert
        await Assert.That(result).Contains('>').And.DoesNotContain('-');
    }

    [Test]
    public async Task ContainsSingleDigit_WhenDigitPlacementIsOnce()
    {
        // Arrange
        PassphraseOptions options = new() { DigitPlacement = DigitPlacementOption.Once };
        Passphrase generator = new(options);

        // Act
        string result = generator.Generate();
        int digitCount = result.Count(char.IsDigit);

        // Assert
        await Assert.That(digitCount).IsEqualTo(1);
    }

    [Test]
    public async Task DigitQuantityIsEqualToWords_WhenDigitPlacementIsEveryWord()
    {
        // Arrange
        PassphraseOptions options = new() { DigitPlacement = DigitPlacementOption.EveryWord, Words = 4 };
        Passphrase generator = new(options);

        // Act
        string result = generator.Generate();
        int digitCount = result.Count(char.IsDigit);

        // Assert
        await Assert.That(digitCount).IsEqualTo(4);
    }

    [Test]
    public async Task ContainsNoDigits_WhenDigitPlacementIsNone()
    {
        // Arrange
        PassphraseOptions options = new() { DigitPlacement = DigitPlacementOption.None };
        Passphrase generator = new(options);

        // Act
        string result = generator.Generate();
        int digitCount = result.Count(char.IsDigit);

        // Assert
        await Assert.That(digitCount).IsEqualTo(0);
    }

    [Test]
    public async Task ContainsOnlyUppercaseWords_WhenCasingIsUpper()
    {
        // Arrange
        PassphraseOptions options = new() { Casing = CasingOption.Upper };
        Passphrase generator = new(options);
        var regex = OnlyUpperCaseCharactersRegex();

        // Act
        var words = generator.Generate().Split('-');

        // Assert
        foreach (string word in words)
        {
            await Assert.That(regex.IsMatch(word)).IsTrue();
        }
    }

    [Test]
    public async Task ContainsOnlyLowercaseWords_WhenCasingIsLower()
    {
        // Arrange
        PassphraseOptions options = new() { Casing = CasingOption.Lower };
        Passphrase generator = new(options);
        Regex regex = OnlyLowerCaseCharactersRegex();

        // Act
        var words = generator.Generate().Split('-');

        // Assert
        foreach (string word in words)
        {
            await Assert.That(regex.IsMatch(word)).IsTrue();
        }
    }

    [Test]
    public async Task ContainsMixedCaseWords_WhenCasingIsRandom()
    {
        // Arrange
        PassphraseOptions options = new() { Casing = CasingOption.Random };
        Passphrase generator = new(options);

        // Act
        var words = generator.Generate().Split('-');

        // Assert
        bool hasUpper = words.Any(word => word.Any(char.IsUpper));
        bool hasLower = words.Any(word => word.Any(char.IsLower));

        await Assert.That(hasUpper).IsTrue();
        await Assert.That(hasLower).IsTrue();
    }

    [Test]
    public async Task ReturnsCorrectSeparator_GivenWhitespaceSeparator()
    {
        // Arrange
        PassphraseOptions options = new() { Separator = ' ' };
        Passphrase generator = new(options);

        // Act
        var result = generator.Generate();

        // Assert
        await Assert.That(result).Contains(' ').And.DoesNotContain('-');
    }

    [Test]
    public async Task ReturnsCorrectSeparator_GivenSpecialCharacterSeparator()
    {
        // Arrange
        PassphraseOptions options = new() { Separator = '_' };
        Passphrase generator = new(options);

        // Act
        var result = generator.Generate();

        // Assert
        await Assert.That(result).Contains('_').And.DoesNotContain('-');
    }

    [Test]
    public async Task DigitQuantityIsEqualToWords_WhenDigitPlacementIsEveryWordAndCasingIsLower()
    {
        // Arrange
        PassphraseOptions options = new()
        {
            DigitPlacement = DigitPlacementOption.EveryWord, Words = 3, Casing = CasingOption.Lower
        };
        Passphrase generator = new(options);

        // Act
        var result = generator.Generate();
        int digitCount = result.Count(char.IsDigit);

        // Assert
        await Assert.That(digitCount).IsEqualTo(3);
    }

    [Test]
    public async Task ContainsSingleDigit_WhenDigitPlacementIsOnceAndCasingIsRandom()
    {
        // Arrange
        PassphraseOptions options = new() { DigitPlacement = DigitPlacementOption.Once, Casing = CasingOption.Random };
        Passphrase generator = new(options);

        // Act
        var result = generator.Generate();
        int digitCount = result.Count(char.IsDigit);

        // Assert
        await Assert.That(digitCount).IsEqualTo(1);
    }

    [Test]
    public async Task ThrowsArgumentException_GivenZeroWords()
    {
        // Arrange
        PassphraseOptions options = new() { Words = 0 };
        Passphrase generator = new(options);

        // Act & Assert
        await Assert.That(generator.Generate)
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ThrowsArgumentException_GivenNegativeWords()
    {
        // Arrange
        PassphraseOptions options = new() { Words = -1 };
        Passphrase generator = new(options);

        // Act & Assert
        await Assert.That(generator.Generate)
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ThrowsArgumentException_GivenInvalidSeparator()
    {
        // Arrange
        PassphraseOptions options = new() { Separator = '\0' };
        Passphrase generator = new(options);

        // Act & Assert
        await Assert.That(generator.Generate)
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ReturnsPassphrase_GivenLargeWordCount()
    {
        // Arrange
        PassphraseOptions options = new() { Words = 1000 };
        Passphrase generator = new(options);

        // Act
        var result = generator.Generate();

        // Assert
        await Assert.That(result.Split('-')).Count().IsEqualTo(1000);
    }

    [GeneratedRegex("^[A-Z]")]
    private static partial Regex OnlyUpperCaseCharactersRegex();
    [GeneratedRegex("^[a-z]")]
    private static partial Regex OnlyLowerCaseCharactersRegex();
}