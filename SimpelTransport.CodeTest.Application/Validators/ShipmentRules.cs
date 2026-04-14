namespace SimpelTransport.CodeTest.Application.Validators;

public static class ShipmentRules
{
    public const double SystemMaxWeight = 40000;
    public const double SystemMaxDimension = 600;

    // Ordered from smallest to largest, validator picks first match
    public static IReadOnlyList<ShipmentTypeRule> TypeRules { get; } =
    [
        new(ShipmentType.Package,    MaxWeight: 35,   MaxLongSide: 150, MaxShortSide: 150),
        new(ShipmentType.HalfPallet, MaxWeight: 400,  MaxLongSide: 80,  MaxShortSide: 60),
        new(ShipmentType.Pallet,     MaxWeight: 1000, MaxLongSide: 600, MaxShortSide: 300),
    ];
}
