using Payments.Application.DTOs;
using Payments.Application.Interfaces;
using Payments.Domain.Entities;

namespace Payments.Application.Services;

public sealed class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<CreatePaymentResponseDto> CreatePayment(CreatePaymentRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidateCreateRequest(request);

        var (payment, isDuplicate) = await _paymentRepository.AddPaymentAsync(
            request.Amount,
            request.Currency,
            request.ClientRequestId,
            cancellationToken);

        // Service remains idempotent: same clientRequestId always returns the same payment.
        return new CreatePaymentResponseDto
        {
            Payment = MapToDto(payment),
            IsDuplicate = isDuplicate
        };
    }

    public async Task<PagedResultDto<PaymentDto>> GetAllPayments(GetPaymentsRequestDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedPageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var normalizedPageSize = request.PageSize < 1 ? 50 : Math.Min(request.PageSize, 500);

        var currency = NormalizeCurrencyOrNull(request.Currency);

        var (items, totalCount) = await _paymentRepository.GetPaymentsAsync(
            normalizedPageNumber,
            normalizedPageSize,
            currency,
            request.FromCreated,
            request.ToCreated,
            cancellationToken);

        return new PagedResultDto<PaymentDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            PageNumber = normalizedPageNumber,
            PageSize = normalizedPageSize
        };
    }

    public async Task<PaymentDto?> UpdatePayment(int id, UpdatePaymentRequestDto request, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }

        ValidateUpdateRequest(request);

        var payment = await _paymentRepository.UpdatePaymentAsync(id, request.Amount, request.Currency, cancellationToken);
        return payment is null ? null : MapToDto(payment);
    }

    public Task<bool> DeletePayment(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }

        return _paymentRepository.DeletePaymentAsync(id, cancellationToken);
    }

    private static void ValidateCreateRequest(CreatePaymentRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Amount), "Amount must be greater than zero.");
        }

        if (request.ClientRequestId == Guid.Empty)
        {
            throw new ArgumentException("ClientRequestId cannot be empty.", nameof(request.ClientRequestId));
        }

        _ = NormalizeRequiredCurrency(request.Currency);
    }

    private static void ValidateUpdateRequest(UpdatePaymentRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Amount), "Amount must be greater than zero.");
        }

        _ = NormalizeRequiredCurrency(request.Currency);
    }

    private static string? NormalizeCurrencyOrNull(string? currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            return null;
        }

        return NormalizeRequiredCurrency(currency);
    }

    private static string NormalizeRequiredCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency is required.", nameof(currency));
        }

        var normalized = currency.Trim().ToUpperInvariant();
        if (!Payment.AllowedCurrencies.Contains(normalized, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Currency must be one of: USD, EUR, INR, GBP.", nameof(currency));
        }

        return normalized;
    }

    private static PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            Reference = payment.Reference,
            Amount = payment.Amount,
            Currency = payment.Currency,
            ClientRequestId = payment.ClientRequestId,
            CreatedAt = payment.CreatedAt
        };
    }
}
