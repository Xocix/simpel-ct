## Shipment Validation Context

Our logistics API receives thousands of transport requests daily from various customer systems. Unfortunately, the data quality is often poor. Customers frequently:

- Declare a packageType that contradicts the physical dimensions (e.g., a 500kg "Package").
- Send Unspecified as a type because their system doesn't know our classifications.
- Swap length and width values.
- Your goal is to implement the logic inside the ShipmentValidator.cs class. You must normalize and validate incoming requests based on these constraints:

---

| Type       | Max Weight | Max Dimensions                                     |
| ---------- | ---------: | -------------------------------------------------- |
| Package    |      35 kg | 150 cm (longest side)                              |
| HalfPallet |     400 kg | 60 cm x 80 cm                                      |
| Pallet     |    1000 kg | 120 cm x 80 cm (default) _(up to 600 cm x 300 cm)_ |

---

- Implementation Requirements
  - **Normalization**: If a user sends "Package" but the weight is 50kg, your code should "upgrade" it to the correct type (e.g., HalfPallet).
  - **Detection**: If a user sends "Unspecified" your code must "guess" the correct type based on the smallest fit.
  - **Validation**: Return a failure if the weight is negative, zero, or exceeds the absolute system maximums.
  - **System Max**: Any shipment over 40,000 kg or with any dimension over 600 cm must be rejected.
  - **Orientation**: A 60x80 HalfPallet is the same as an 80x60 HalfPallet. Your logic should handle rotated dimensions.

We have provided a ShipmentTests.cs file using TheoryData. Your solution is considered successful only when all tests pass. Feel free to add your own test cases if you find edge cases we missed.
