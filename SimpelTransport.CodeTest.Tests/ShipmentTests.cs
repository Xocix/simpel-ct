using SimpelTransport.CodeTest.Application.Dto;
using SimpelTransport.CodeTest.Application.Validators;

namespace SimpelTransport.CodeTest.Tests;

public class ShipmentTests
{
    private readonly ShipmentValidator _validator = new();

    [Theory]
    [ClassData(typeof(ShipmentTestData))]
    public void ValidateAndNormalize_ShouldReturnExpectedResult(
        string inputType,
        double weight,
        double length,
        double width,
        string expectedType,
        bool expectedValid
    )
    {
        var request = new ShipmentRequestDto("REQ-ID", inputType, weight, length, width, 100);

        var result = _validator.ValidateAndNormalize(request);

        Assert.Equal(expectedValid, result.IsValid);

        if (expectedValid)
        {
            Assert.Equal(expectedType, result.FinalType);
        }
    }
}