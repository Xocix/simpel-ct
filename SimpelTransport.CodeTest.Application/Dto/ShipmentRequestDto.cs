namespace SimpelTransport.CodeTest.Application.Dto;

// DeclaredType: "Package", "HalfPallet", "Pallet", or "Unspecified" 
public record ShipmentRequestDto(string RequestId, string DeclaredType, double WeightKg, double LengthCm, double WidthCm, double HeightCm);