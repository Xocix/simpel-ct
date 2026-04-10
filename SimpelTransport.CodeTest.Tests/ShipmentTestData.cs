namespace SimpelTransport.CodeTest.Tests;

public class ShipmentTestData : TheoryData<string, double, double, double, string, bool>
{
    public ShipmentTestData()
    {
        // Corrections (Wrong dimensions for Declared Type)
        Add("Package", 35.1, 40, 40, "HalfPallet", true);
        Add("Package", 10, 151, 40, "Pallet", true);
        Add("HalfPallet", 200, 120, 80, "Pallet", true);
        Add("Pallet", 2, 20, 20, "Package", true);

        // Auto-Detection (Wrong or No Declared Type)
        Add("Unspecified", 5, 30, 30, "Package", true);
        Add("Unspecified", 50, 30, 30, "HalfPallet", true);
        Add("Unspecified", 200, 60, 80, "HalfPallet", true);
        Add("Unspecified", 200, 80, 60, "HalfPallet", true);
        Add("Unspecified", 400, 120, 80, "Pallet", true);

        // Boundaries
        Add("Package", 35.0, 40, 40, "Package", true);
        Add("Pallet", 1000, 600, 300, "Pallet", true);

        // Failures
        Add("Pallet", 40001, 120, 80, "", false);
        Add("Package", 10, 0, 40, "", false);
        Add("Package", -5, 40, 40, "", false);
        Add("Unspecified", 500, 1000, 80, "", false);
    }
}