using System.Text.RegularExpressions;

namespace Cyfrinair.Core.Tests;

public partial class PinGenerator
{
    [Test]
    public async Task ThrowsArgumentException_GivenZeroLength()
    {
        Pin pinGenerator = new();
        await Assert.That(() => pinGenerator.Generate(0))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ThrowsArgumentException_GivenNegativeLength()
    {
        Pin pinGenerator = new();
        await Assert.That(() => pinGenerator.Generate(-1))
            .Throws<ArgumentException>();
    }
    
    [Test]
    [Arguments(1)]
    [Arguments(5)]
    [Arguments(10)]
    [Arguments(25)]
    [Arguments(50)]
    public async Task ReturnsPinOfCorrectLength_GivenValidLength(int length)
    {
      Pin pinGenerator = new();
      
      await Assert.That(pinGenerator.Generate(length)).Length().IsEqualTo(length);
    }

    [Test]
    public async Task ReturnsPinThatContainsOnlyDigits_GivenValidLength()
    {
        Regex regex = OnlyDigitCharactersRegex();
        Pin pinGenerator = new();
        var result = pinGenerator.Generate(100);
        
        await Assert.That(regex.IsMatch(result)).IsTrue();
    }

    [GeneratedRegex("^[0-9]+$")]
    private static partial Regex OnlyDigitCharactersRegex();
    
    [Test]
    public async Task ReturnsSingleDigit_GivenLengthOne()
    {
        Pin pinGenerator = new();
        var result = pinGenerator.Generate(1);
        
        await Assert.That(result).Length().IsEqualTo(1);
        await Assert.That(char.IsDigit(result[0])).IsTrue();
    }

    [Test]
    public async Task ReturnsDifferentValuesAcrossMultipleGenerations_GivenValidLength()
    {
        Pin pinGenerator = new();
        var results = Enumerable.Range(0, 100)
            .Select(_ => pinGenerator.Generate(10))
            .ToList();

        await Assert.That(results).HasDistinctItems();
    }

    [Test]
    public async Task ReturnsPinContainingOnlyCharactersFromDigitSet_GivenValidLength()
    {
        Pin pinGenerator = new();
        string result = pinGenerator.Generate(100);
        
        await Assert.That(result.All(Pin.DigitSet.Contains)).IsTrue();
        
    }
}