namespace Cyfrinair.Core.Tests;

public class PasswordGenerator
{
    [Test]
    public async Task ThrowsArgumentNullException_GivenNullOption()
    {
        // Arrange
        PasswordOptions options = null!;

        // Act & Assert
        await Assert.That(() =>
            {
                new Password(options);
            })
            .Throws<ArgumentNullException>();
    }
    
    [Test]
    [Arguments(10)]
    [Arguments(15)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task ReturnsPasswordWithCorrectLength_GivenValidLength(int length)
    {
        // Arrange
        PasswordOptions options = new() { Length = length };
        Password generator = new(options);

        // Act
        string result = generator.Generate();

        // Assert
        await Assert.That(result).Length().IsEqualTo(length);
    }

    [Test]
    public async Task ContainsNoAmbiguousCharacters_GivenAmbiguousCharsDisabled()
    {
        // Arrange
        PasswordOptions options = new() { IncludeAmbiguousChars = false };
        Password generator = new(options);
        var results = new List<string>(100);

        // Act
        
        // Generate 100 passwords to ensure coverage
        for (var i = 0; i < 100; i++)
        {
            results.Add(generator.Generate());
        }

        // Assert
        foreach (var result in results)
        {
            foreach (var ambiguousChar in Password.AmbiguousChars)
            {
                await Assert.That(result).DoesNotContain(ambiguousChar);
            }
        }
    }

    [Test]
    public async Task ContainsNoAmbiguousDigits_GivenAmbiguousCharsDisabledAndDigitsEnabled()
    {
        // Arrange
        PasswordOptions options = new() { IncludeAmbiguousChars = false, IncludeDigits = true };
        Password generator = new(options);
        var results = new List<string>(100);

        // Act
        // Generate 100 passwords to ensure coverage
        for (var i = 0; i < 100; i++)
        {
            results.Add(generator.Generate());
        }

        // Assert
        foreach (string result in results)
        {
            foreach (char ambiguousDigit in Password.AmbiguousDigits)
            {
                await Assert.That(result).DoesNotContain(ambiguousDigit);
            }
        }
    }

    [Test]
    public async Task ContainsNoAmbiguousSymbols_GivenSymbolsEnabled()
    {
        // Arrange
        PasswordOptions options = new() { IncludeSymbols = true, IncludeAmbiguousChars = false };
        Password generator = new(options);
        var results = new List<string>(100);

        // Act
        // Generate 100 passwords to ensure coverage
        for (var i = 0; i < 100; i++)
        {
            results.Add(generator.Generate());
        }

        // Assert

        foreach (var result in results)
        {
            foreach (var ambiguousSymbol in Password.AmbiguousSymbols)
            {
                await Assert.That(result).DoesNotContain(ambiguousSymbol);
            }
        }
    }

    [Test]
    public async Task ContainsAllAmbiguousChars_WhenAllOptionsEnabled()
    {
        PasswordOptions options = new() { IncludeDigits = true, IncludeSymbols = true, IncludeAmbiguousChars = true };
        Password generator = new(options);
        var results = new List<string>(100);

        // Generate 100 passwords to ensure coverage
        for (var i = 0; i < 100; i++)
        {
            results.Add(generator.Generate());
        }

        bool foundAmbiguousChar = results.Any(pw => Password.AmbiguousChars.Any(pw.Contains));
        bool foundAmbiguousSymbol = results.Any(pw => Password.AmbiguousSymbols.Any(pw.Contains));
        bool foundAmbiguousDigit = results.Any(pw => Password.AmbiguousDigits.Any(pw.Contains));

        await Assert.That(foundAmbiguousChar).IsTrue();
        await Assert.That(foundAmbiguousSymbol).IsTrue();
        await Assert.That(foundAmbiguousDigit).IsTrue();
    }

    [Test]
    public async Task ContainsOnlyAmbiguousChars_WhenOnlyAmbiguousCharsEnabled()
    {
        PasswordOptions options = new() { IncludeDigits = false, IncludeSymbols = false, IncludeAmbiguousChars = true };
        Password generator = new(options);
        var results = new List<string>(100);

        // Generate 100 passwords to ensure coverage
        for (var i = 0; i < 100; i++)
        {
            results.Add(generator.Generate());
        }

        foreach (var c in results.SelectMany(result => result))
        {
            await Assert.That(
                    Password.BaseCharacterSet.Contains(c) ||
                    Password.AmbiguousChars.Contains(c)
                )
                .IsTrue();
        }
    }

    [Test]
    public async Task ContainsOnlySymbols_WhenOnlySymbolsEnabled()
    {
        PasswordOptions options = new() { IncludeDigits = false, IncludeSymbols = true, IncludeAmbiguousChars = false };
        Password generator = new(options);
        var results = new List<string>(100);
        
        // Generate 100 passwords to ensure coverage
        for (var i = 0; i < 100; i++)
        {
            results.Add(generator.Generate());
        }

        foreach (var c in results.SelectMany(result => result))
        {
            await Assert.That(
                    Password.BaseCharacterSet.Contains(c) ||
                    Password.SymbolSet.Contains(c)
                )
                .IsTrue();
        }
    }

    [Test]
    public async Task ContainsOnlyDigits_WhenOnlyDigitsEnabled()
    {
        PasswordOptions options = new() { IncludeDigits = true, IncludeSymbols = false, IncludeAmbiguousChars = false };
        Password generator = new(options);
        var results = new List<string>(100);

        // Generate 100 passwords to ensure coverage
        for (var i = 0; i < 100; i++)
        {
            results.Add(generator.Generate());
        }

        foreach (var c in results.SelectMany(result => result))
        {
            await Assert.That(
                    Password.BaseCharacterSet.Contains(c) ||
                    Password.DigitSet.Contains(c)
                )
                .IsTrue();
        }
    }

    [Test]
    public async Task ContainsAmbiguousSymbolsButNoDigits_WhenSymbolsAndAmbiguousCharsEnabled()
    {
        PasswordOptions options = new() { IncludeDigits = false, IncludeSymbols = true, IncludeAmbiguousChars = true };
        Password generator = new(options);
        var results = new List<string>(100);
        
        // Generate 100 passwords to ensure coverage
        for (var i = 0; i < 100; i++)
        {
            results.Add(generator.Generate());
        }

        bool foundAmbiguousSymbol = results.Any(pw => Password.AmbiguousSymbols.Any(pw.Contains));
        bool foundAmbiguousDigit = results.Any(pw => Password.AmbiguousDigits.Any(pw.Contains));

        await Assert.That(foundAmbiguousSymbol).IsTrue();
        await Assert.That(foundAmbiguousDigit).IsFalse();
    }

    [Test]
    public async Task ContainsAmbiguousDigitsButNoSymbols_WhenDigitsAndAmbiguousCharsEnabled()
    {
        PasswordOptions options = new() { IncludeDigits = true, IncludeSymbols = false, IncludeAmbiguousChars = true };
        Password generator = new(options);
        var results = new List<string>(100);
        
        // Generate 100 passwords to ensure coverage
        for (var i = 0; i < 100; i++)
        {
            results.Add(generator.Generate());
        }

        bool foundAmbiguousDigit = results.Any(pw => Password.AmbiguousDigits.Any(pw.Contains));
        bool foundAmbiguousSymbol = results.Any(pw => Password.AmbiguousSymbols.Any(pw.Contains));

        await Assert.That(foundAmbiguousDigit).IsTrue();
        await Assert.That(foundAmbiguousSymbol).IsFalse();
    }

    [Test]
    public async Task ContainsNoDigitsOrSymbolsOrAmbiguous_WhenAllOptionsDisabled()
    {
        PasswordOptions options =
            new() { IncludeDigits = false, IncludeSymbols = false, IncludeAmbiguousChars = false };
        Password generator = new(options);
        
        var results = new List<string>(100);
        
        // Generate 100 passwords to ensure coverage
        for (var i = 0; i < 100; i++)
        {
            results.Add(generator.Generate());
        }

        foreach (var c in results.SelectMany(result => result))
        {
            await Assert.That(Password.BaseCharacterSet.Contains(c)).IsTrue();
        }
    }

    [Test]
    public async Task ThrowsArgumentException_GivenZeroLength()
    {
        PasswordOptions options = new() { Length = 0 };
        await Assert.That(() => new Password(options).Generate())
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ThrowsArgumentException_GivenNegativeLength()
    {
        PasswordOptions options = new() { Length = -5 };
        await Assert.That(() => new Password(options).Generate())
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ReturnsPasswordWithMaxLength_GivenLargeLength()
    {
        // Adjust max length as appropriate for your application
        int maxLength = 1000;
        PasswordOptions options = new() { Length = maxLength };
        Password generator = new(options);

        string result = generator.Generate();

        await Assert.That(result).Length().IsEqualTo(maxLength);
    }

    [Test]
    public async Task ReturnsAllPasswords_GivenLargeQuantity()
    {
        ushort quantity = 1000; // Use a large but reasonable value for test speed
        PasswordOptions options = new();
        Password generator = new(options);
        
        var results = new List<string>(quantity);
        
        // Generate 100 passwords to ensure coverage
        for (var i = 0; i < quantity; i++)
        {
            results.Add(generator.Generate());
        }

        await Assert.That(results).Count().IsEqualTo(quantity).And.HasDistinctItems();
    }

    [Test]
    public async Task ReturnsPasswordWithBaseSet_GivenAllOptionsDisabledAndLengthGreaterThanZero()
    {
        PasswordOptions options = new()
        {
            IncludeDigits = false, IncludeSymbols = false, IncludeAmbiguousChars = false, Length = 10
        };
        Password generator = new(options);

        var result = generator.Generate();

        foreach (var c in result)
        {
            await Assert.That(Password.BaseCharacterSet.Contains(c)).IsTrue();
        }
    }
}