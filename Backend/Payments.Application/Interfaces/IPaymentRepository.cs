using Payments.Domain.Entities;

namespace Payments.Application.Interfaces;

public interface IPaymentRepository
{
    Task<(Payment Payment, bool IsDuplicate)> AddPaymentAsync(
        decimal amount,
        string currency,
        Guid clientRequestId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Payment> Items, long TotalCount)> GetPaymentsAsync(
        int pageNumber,
        int pageSize,
        string? currency,
        DateTime? fromCreated,
        DateTime? toCreated,
        CancellationToken cancellationToken = default);

    Task<Payment?> UpdatePaymentAsync(int id, decimal amount, string currency, CancellationToken cancellationToken = default);

    Task<bool> DeletePaymentAsync(int id, CancellationToken cancellationToken = default);
}
