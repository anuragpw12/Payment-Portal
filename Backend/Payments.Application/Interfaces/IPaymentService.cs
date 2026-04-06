using Payments.Application.DTOs;

namespace Payments.Application.Interfaces;

public interface IPaymentService
{
    Task<CreatePaymentResponseDto> CreatePayment(CreatePaymentRequestDto request, CancellationToken cancellationToken = default);
    Task<PagedResultDto<PaymentDto>> GetAllPayments(GetPaymentsRequestDto request, CancellationToken cancellationToken = default);
    Task<PaymentDto?> UpdatePayment(int id, UpdatePaymentRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> DeletePayment(int id, CancellationToken cancellationToken = default);
}
