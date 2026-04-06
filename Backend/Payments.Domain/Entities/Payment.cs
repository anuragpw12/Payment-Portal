namespace Payments.Domain.Entities;

public sealed class Payment
{
    private static readonly HashSet<string> AllowedCurrencySet = new(StringComparer.OrdinalIgnoreCase)
    {
        "USD",
        "EUR",
        "INR",
        "GBP"
    };

    public Payment(
        int id,
        string reference,
        decimal amount,
        string currency,
        Guid clientRequestId,
        DateTime createdAt)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(reference))
        {
            throw new ArgumentException("Reference is required.", nameof(reference));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        if (clientRequestId == Guid.Empty)
        {
            throw new ArgumentException("ClientRequestId cannot be empty.", nameof(clientRequestId));
        }

        if (createdAt == default)
        {
            throw new ArgumentException("CreatedAt must be a valid date.", nameof(createdAt));
        }

        Id = id;
        Reference = reference.Trim();
        Amount = amount;
        Currency = NormalizeAndValidateCurrency(currency);
        ClientRequestId = clientRequestId;
        CreatedAt = createdAt;
    }

    public int Id { get; }
    public string Reference { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public Guid ClientRequestId { get; }
    public DateTime CreatedAt { get; }

    public static IReadOnlyCollection<string> AllowedCurrencies => AllowedCurrencySet;

    public static Payment Create(
        string reference,
        decimal amount,
        string currency,
        Guid clientRequestId,
        DateTime createdAt)
    {
        return new Payment(
            id: 0,
            reference: reference,
            amount: amount,
            currency: currency,
            clientRequestId: clientRequestId,
            createdAt: createdAt);
    }

    private static string NormalizeAndValidateCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency is required.", nameof(currency));
        }

        var normalized = currency.Trim().ToUpperInvariant();
        if (!AllowedCurrencySet.Contains(normalized))
        {
            throw new ArgumentException(
                $"Currency '{currency}' is not supported. Allowed values: USD, EUR, INR, GBP.",
                nameof(currency));
        }

        return normalized;
    }
}
