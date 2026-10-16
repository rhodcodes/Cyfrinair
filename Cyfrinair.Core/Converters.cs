namespace Cyfrinair.Core;

public static class Converters
{
    public static CasingOption ToCasingOption(this string casing)
    {
        return casing.ToLower() switch
        {
            "random" => CasingOption.Random,
            "upper" => CasingOption.Upper,
            "lower" => CasingOption.Lower,
            _ => CasingOption.Random
        };
    }
    
    public static DigitPlacementOption ToDigitPlacementOption(this string digitPlacement)
    {
        return digitPlacement.ToLower() switch
        {
            "once" => DigitPlacementOption.Once,
            "none" => DigitPlacementOption.None,
            "every" => DigitPlacementOption.EveryWord,
            _ => DigitPlacementOption.Once
        };
    }
}