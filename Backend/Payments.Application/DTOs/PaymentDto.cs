namespace Payments.Application.DTOs;

public sealed class PaymentDto
{
    public int Id { get; init; }
    public string Reference { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public Guid ClientRequestId { get; init; }
    public DateTime CreatedAt { get; init; }
}
