using SimpelTransport.CodeTest.Application.Dto;

namespace SimpelTransport.CodeTest.Application.Validators;

public class ShipmentValidator
{
    public ValidationResultDto ValidateAndNormalize(ShipmentRequestDto request)
    {
        // TODO: Implement the logic to normalize the DeclaredType and validate against the business rules.
        // Currently, it just returns exactly what the customer sent (WHICH IS WRONG)
        return new ValidationResultDto(true, request.DeclaredType, "Not yet implemented");
    }
}