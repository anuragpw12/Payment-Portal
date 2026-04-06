using System.ComponentModel.DataAnnotations;

namespace Payments.Application.DTOs;

public sealed class CreatePaymentRequestDto
{
    [Range(typeof(decimal), "0.01", "999999999999999.9999")]
    public decimal Amount { get; init; }

    [Required]
    [StringLength(10, MinimumLength = 3)]
    public string Currency { get; init; } = string.Empty;

    [Required]
    public Guid ClientRequestId { get; init; }
}
