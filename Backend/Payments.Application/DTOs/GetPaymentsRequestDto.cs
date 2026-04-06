namespace Payments.Application.DTOs;

public sealed class GetPaymentsRequestDto
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
    public string? Currency { get; init; }
    public DateTime? FromCreated { get; init; }
    public DateTime? ToCreated { get; init; }
}
