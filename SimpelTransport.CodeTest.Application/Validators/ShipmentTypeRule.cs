namespace SimpelTransport.CodeTest.Application.Validators;

public record ShipmentTypeRule(
    ShipmentType Type,
    double MaxWeight,
    double MaxLongSide,
    double MaxShortSide)
{
    public bool Fits(double weight, double longSide, double shortSide)
    {
        return weight <= MaxWeight && longSide <= MaxLongSide && shortSide <= MaxShortSide;
    }
}
