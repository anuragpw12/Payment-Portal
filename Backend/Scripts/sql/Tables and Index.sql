
--TABLE

--PaymentReferenceCounter is used to generate Reference values safely under concurrency and to reset numbering daily.

--Without it, if sp_AddPayment uses MAX(...) + 1 from the Payments table, two parallel transactions can read the same 
--maximum value and generate duplicate references.

--With PaymentReferenceCounter:
--There is one row per date (RefDate), e.g. 2026-04-06
--LastNumber is incremented inside a transaction with proper locking
--The first insert of a new day starts at 1, so the reset happens automatically
--It avoids scanning the entire Payments table for each insert, improving performance

--This approach ensures correctness, provides automatic daily reset, and improves insert performance.

IF OBJECT_ID('dbo.PaymentReferenceCounter', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PaymentReferenceCounter
    (
        RefDate     DATE        NOT NULL CONSTRAINT PK_PaymentReferenceCounter PRIMARY KEY,
        LastNumber  INT         NOT NULL
    );
END

--Main Table - Payments
IF OBJECT_ID('dbo.Payments', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Payments
    (
        Id               INT               IDENTITY(1,1) NOT NULL CONSTRAINT PK_Payments PRIMARY KEY CLUSTERED,
        Reference        VARCHAR(20)       NOT NULL, -- PAY-YYYYMMDD-####
        Amount           DECIMAL(19,4)     NOT NULL,
        Currency         VARCHAR(10)       NOT NULL,
        ClientRequestId  UNIQUEIDENTIFIER  NOT NULL,
        CreatedAt        DATETIME2(0)      NOT NULL CONSTRAINT DF_Payments_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END
GO


-- INDEXES / CONSTRAINTS

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Payments_ClientRequestId' AND object_id = OBJECT_ID('dbo.Payments'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UQ_Payments_ClientRequestId
        ON dbo.Payments (ClientRequestId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Payments_Reference' AND object_id = OBJECT_ID('dbo.Payments'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UQ_Payments_Reference
        ON dbo.Payments (Reference);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Payments_CreatedAt_Id' AND object_id = OBJECT_ID('dbo.Payments'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Payments_CreatedAt_Id
        ON dbo.Payments (CreatedAt DESC, Id DESC)
        INCLUDE (Reference, Amount, Currency, ClientRequestId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Payments_Currency_CreatedAt' AND object_id = OBJECT_ID('dbo.Payments'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Payments_Currency_CreatedAt
        ON dbo.Payments (Currency, CreatedAt DESC)
        INCLUDE (Id, Reference, Amount, ClientRequestId);
END
GO
