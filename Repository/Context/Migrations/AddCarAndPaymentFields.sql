-- Migration: Add Car and Payment Fields
-- Date: 2025-11-25

-- Add Status column to Cars table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cars]') AND name = 'Status')
BEGIN
    ALTER TABLE [dbo].[Cars]
    ADD [Status] int NOT NULL DEFAULT 1;
END
GO

-- Add DepositPercent column to Cars table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cars]') AND name = 'DepositPercent')
BEGIN
    ALTER TABLE [dbo].[Cars]
    ADD [DepositPercent] int NOT NULL DEFAULT 0;
END
GO

-- Add RentalLocationId column to Cars table (nullable)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cars]') AND name = 'RentalLocationId')
BEGIN
    ALTER TABLE [dbo].[Cars]
    ADD [RentalLocationId] int NULL;
END
GO

-- Add TxnRef column to Payments table (nullable)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'TxnRef')
BEGIN
    ALTER TABLE [dbo].[Payments]
    ADD [TxnRef] nvarchar(max) NULL;
END
GO

-- Add TransactionNo column to Payments table (nullable)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'TransactionNo')
BEGIN
    ALTER TABLE [dbo].[Payments]
    ADD [TransactionNo] nvarchar(max) NULL;
END
GO

-- Update existing Cars records with default values if needed
UPDATE [dbo].[Cars]
SET [Status] = 1, [DepositPercent] = 0
WHERE [Status] IS NULL OR [DepositPercent] IS NULL;
GO

