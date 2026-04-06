-- =============================================
-- 1. sp_AddPayment
-- 3.1 sp_AddPayment
-- - Prevent duplicate by ClientRequestId
-- - Generate Reference: PAY-YYYYMMDD-####
-- - Sequence resets daily via PaymentReferenceCounter (RefDate key)
-- - Return existing record if duplicate
-- =============================================
CREATE PROCEDURE dbo.sp_AddPayment
@Amount           DECIMAL(19,4),
@Currency         VARCHAR(10),
@ClientRequestId  UNIQUEIDENTIFIER
AS
BEGIN
SET NOCOUNT ON;
SET XACT_ABORT ON;

 
DECLARE @Now       DATETIME2(0) = SYSUTCDATETIME();
DECLARE @RefDate   DATE = CAST(@Now AS DATE);
DECLARE @NextNo    INT;
DECLARE @Reference VARCHAR(20);
DECLARE @NewId     INT;

BEGIN TRY
    BEGIN TRAN;

    -- Duplicate check
    IF EXISTS (
        SELECT 1
        FROM dbo.Payments WITH (UPDLOCK, HOLDLOCK)
        WHERE ClientRequestId = @ClientRequestId
    )
    BEGIN
        SELECT Id, Reference, Amount, Currency, ClientRequestId, CreatedAt
        FROM dbo.Payments
        WHERE ClientRequestId = @ClientRequestId;

        COMMIT TRAN;
        RETURN;
    END

    -- Sequence handling
    DECLARE @Counter TABLE (NextNumber INT);

    UPDATE c WITH (UPDLOCK, HOLDLOCK)
    SET c.LastNumber = c.LastNumber + 1
    OUTPUT inserted.LastNumber INTO @Counter(NextNumber)
    FROM dbo.PaymentReferenceCounter c
    WHERE c.RefDate = @RefDate;

    IF NOT EXISTS (SELECT 1 FROM @Counter)
    BEGIN
        INSERT INTO dbo.PaymentReferenceCounter (RefDate, LastNumber)
        VALUES (@RefDate, 1);

        INSERT INTO @Counter (NextNumber) VALUES (1);
    END

    SELECT TOP (1) @NextNo = NextNumber FROM @Counter;

    IF @NextNo > 9999
    BEGIN
        RAISERROR ('Daily payment reference limit exceeded (max 9999).', 16, 1);
        ROLLBACK TRAN;
        RETURN;
    END

    SET @Reference =
        CONCAT(
            'PAY-',
            CONVERT(CHAR(8), @RefDate, 112),
            '-',
            RIGHT(CONCAT('0000', CONVERT(VARCHAR(10), @NextNo)), 4)
        );

    INSERT INTO dbo.Payments (Reference, Amount, Currency, ClientRequestId, CreatedAt)
    VALUES (@Reference, @Amount, @Currency, @ClientRequestId, @Now);

    SET @NewId = SCOPE_IDENTITY();

    SELECT Id, Reference, Amount, Currency, ClientRequestId, CreatedAt
    FROM dbo.Payments
    WHERE Id = @NewId;

    COMMIT TRAN;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRAN;

    -- Handle duplicate safely
    IF EXISTS (SELECT 1 FROM dbo.Payments WHERE ClientRequestId = @ClientRequestId)
    BEGIN
        SELECT Id, Reference, Amount, Currency, ClientRequestId, CreatedAt
        FROM dbo.Payments
        WHERE ClientRequestId = @ClientRequestId;
        RETURN;
    END

    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT 
        @ErrorMessage = ERROR_MESSAGE(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE();

    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH
 

END
GO

-- =============================================
-- 2. sp_GetPayments
-- Supports optional filters + pagination for optimized reads
-- =============================================
CREATE PROCEDURE dbo.sp_GetPayments
@PageNumber   INT = 1,
@PageSize     INT = 50,
@Currency     VARCHAR(10) = NULL,
@FromCreated  DATETIME2(0) = NULL,
@ToCreated    DATETIME2(0) = NULL
AS
BEGIN
SET NOCOUNT ON;

IF @PageNumber < 1 SET @PageNumber = 1;
IF @PageSize < 1 SET @PageSize = 50;
IF @PageSize > 500 SET @PageSize = 500;

;WITH Filtered AS
(
    SELECT Id, Reference, Amount, Currency, ClientRequestId, CreatedAt
    FROM dbo.Payments
    WHERE (@Currency IS NULL OR Currency = @Currency)
      AND (@FromCreated IS NULL OR CreatedAt >= @FromCreated)
      AND (@ToCreated   IS NULL OR CreatedAt <  @ToCreated)
)
SELECT *
FROM Filtered
ORDER BY CreatedAt DESC, Id DESC
OFFSET (@PageNumber - 1) * @PageSize ROWS
FETCH NEXT @PageSize ROWS ONLY;

SELECT COUNT_BIG(1) AS TotalCount
FROM dbo.Payments
WHERE (@Currency IS NULL OR Currency = @Currency)
  AND (@FromCreated IS NULL OR CreatedAt >= @FromCreated)
  AND (@ToCreated   IS NULL OR CreatedAt <  @ToCreated);


END
GO

-- =============================================
-- 3. sp_UpdatePayment
-- =============================================
CREATE PROCEDURE dbo.sp_UpdatePayment
@Id        INT,
@Amount    DECIMAL(19,4),
@Currency  VARCHAR(10)
AS
BEGIN
SET NOCOUNT ON;


UPDATE dbo.Payments
SET Amount = @Amount,
    Currency = @Currency
WHERE Id = @Id;

IF @@ROWCOUNT = 0
BEGIN
    RAISERROR ('Payment not found for update.', 16, 1);
    RETURN;
END

SELECT Id, Reference, Amount, Currency, ClientRequestId, CreatedAt
FROM dbo.Payments
WHERE Id = @Id;


END
GO

-- =============================================
-- 4. sp_DeletePayment
-- =============================================
CREATE PROCEDURE dbo.sp_DeletePayment
@Id INT
AS
BEGIN
SET NOCOUNT ON;


DELETE FROM dbo.Payments
OUTPUT deleted.Id,
       deleted.Reference,
       deleted.Amount,
       deleted.Currency,
       deleted.ClientRequestId,
       deleted.CreatedAt
WHERE Id = @Id;

IF @@ROWCOUNT = 0
BEGIN
    RAISERROR ('Payment not found for delete.', 16, 1);
    RETURN;
END


END
GO 
 