namespace Cyfrinair.Core;

public record PasswordOptions
{
    public int Length { get; init; } = 25;
    public bool IncludeDigits { get; init; } = true;
    public bool IncludeSymbols { get; init; } = false;
    public bool IncludeAmbiguousChars { get; init; } = false;
    
}