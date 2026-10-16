namespace Cyfrinair.Core;

public record PassphraseOptions
{
    public int Words { get; init; } = 4;
    public char Separator { get; init; } = '-';
    public DigitPlacementOption DigitPlacement { get; init; } = DigitPlacementOption.Once;
    public CasingOption Casing { get; init; } = CasingOption.Upper;
    
}
public enum CasingOption
{
    Upper,
    Lower,
    Random,
}

public enum DigitPlacementOption
{
    Once,
    None,
    EveryWord,
}
