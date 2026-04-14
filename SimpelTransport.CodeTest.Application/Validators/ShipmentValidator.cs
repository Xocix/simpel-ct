using SimpelTransport.CodeTest.Application.Dto;

namespace SimpelTransport.CodeTest.Application.Validators;

public class ShipmentValidator
{
    public ValidationResultDto ValidateAndNormalize(ShipmentRequestDto request)
    {
        // TODO: Implement the logic to normalize the DeclaredType and validate against the business rules.
        // Currently, it just returns exactly what the customer sent (WHICH IS WRONG)
        if (request.WeightKg <= 0 || request.LengthCm <= 0 || request.WidthCm <= 0)
        {
            return Fail("Weight and dimensions must be positive.");
        }

        if (request.WeightKg > ShipmentRules.SystemMaxWeight ||
            request.LengthCm > ShipmentRules.SystemMaxDimension ||
            request.WidthCm > ShipmentRules.SystemMaxDimension)
        {
            return Fail("Exceeds system maximum limits.");
        }

        var longSide = Math.Max(request.LengthCm, request.WidthCm);
        var shortSide = Math.Min(request.LengthCm, request.WidthCm);

        foreach (var rule in ShipmentRules.TypeRules)
        {
            if (rule.Fits(request.WeightKg, longSide, shortSide))
            {
                return Success(rule.Type);
            }
        }

        return Fail("Shipment doesn't fit any supported type.");
    }

    private static ValidationResultDto Success(ShipmentType type)
    {
        return new(true, type.ToString(), $"Classified as {type}.");
    }

    private static ValidationResultDto Fail(string reason)
    {
        return new(false, string.Empty, reason);
    }
}