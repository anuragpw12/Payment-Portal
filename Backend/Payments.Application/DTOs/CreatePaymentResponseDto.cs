namespace Payments.Application.DTOs;

public sealed class CreatePaymentResponseDto
{
    public required PaymentDto Payment { get; init; }
    public bool IsDuplicate { get; init; }
}
