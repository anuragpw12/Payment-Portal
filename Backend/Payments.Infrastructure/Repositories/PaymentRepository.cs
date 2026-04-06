using Dapper;
using Microsoft.Data.SqlClient;
using Payments.Application.Interfaces;
using Payments.Domain.Entities;
using Payments.Infrastructure.DBContext;
using System.Data;

namespace Payments.Infrastructure.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private const int UniqueConstraintViolation = 2627;
    private const int DuplicateKeyViolation = 2601;
    private const string GetByClientRequestIdSql = """
                                                   SELECT TOP (1)
                                                       Id,
                                                       Reference,
                                                       Amount,
                                                       Currency,
                                                       ClientRequestId,
                                                       CreatedAt
                                                   FROM dbo.Payments
                                                   WHERE ClientRequestId = @ClientRequestId;
                                                   """;

    private readonly IDbConnectionFactory _dbConnectionFactory;

    public PaymentRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<(Payment Payment, bool IsDuplicate)> AddPaymentAsync(
        decimal amount,
        string currency,
        Guid clientRequestId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        // Fast idempotency path for retries: if clientRequestId already exists, return it.
        var existing = await GetByClientRequestIdAsync(connection, clientRequestId, cancellationToken);
        if (existing is not null)
        {
            return (existing, true);
        }

        try
        {
            var created = await connection.QuerySingleAsync<Payment>(BuildAddPaymentCommand(amount, currency, clientRequestId, cancellationToken));
            return (created, false);
        }
        catch (SqlException ex) when (ex.Number is UniqueConstraintViolation or DuplicateKeyViolation)
        {
            // Concurrency fallback: another request inserted with same clientRequestId.
            var existingAfterRace = await GetByClientRequestIdAsync(connection, clientRequestId, cancellationToken);
            if (existingAfterRace is not null)
            {
                return (existingAfterRace, true);
            }

            throw;
        }
    }

    public async Task<(IReadOnlyList<Payment> Items, long TotalCount)> GetPaymentsAsync(
        int pageNumber,
        int pageSize,
        string? currency,
        DateTime? fromCreated,
        DateTime? toCreated,
        CancellationToken cancellationToken = default)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "dbo.sp_GetPayments",
            parameters: new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Currency = currency,
                FromCreated = fromCreated,
                ToCreated = toCreated
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi = await connection.QueryMultipleAsync(command);
        var items = (await multi.ReadAsync<Payment>()).ToList();
        var totalCount = await multi.ReadSingleAsync<long>();

        return (items, totalCount);
    }

    public async Task<Payment?> UpdatePaymentAsync(int id, decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "dbo.sp_UpdatePayment",
            parameters: new
            {
                Id = id,
                Amount = amount,
                Currency = currency
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        try
        {
            return await connection.QuerySingleAsync<Payment>(command);
        }
        catch (SqlException ex) when (ex.Number == 50002)
        {
            return null;
        }
    }

    public async Task<bool> DeletePaymentAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "dbo.sp_DeletePayment",
            parameters: new { Id = id },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        try
        {
            _ = await connection.QuerySingleAsync<Payment>(command);
            return true;
        }
        catch (SqlException ex) when (ex.Number == 50003)
        {
            return false;
        }
    }

    private static CommandDefinition BuildAddPaymentCommand(decimal amount, string currency, Guid clientRequestId, CancellationToken cancellationToken)
    {
        return new CommandDefinition(
            commandText: "dbo.sp_AddPayment",
            parameters: new
            {
                Amount = amount,
                Currency = currency,
                ClientRequestId = clientRequestId
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
    }

    private static Task<Payment?> GetByClientRequestIdAsync(IDbConnection connection, Guid clientRequestId, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(
            commandText: GetByClientRequestIdSql,
            parameters: new { ClientRequestId = clientRequestId },
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        return connection.QuerySingleOrDefaultAsync<Payment>(command);
    }
}
