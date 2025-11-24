# Hướng dẫn sửa lỗi Database

## Lỗi: 500 Internal Server Error khi gọi API /api/Car

Lỗi này xảy ra vì database chưa có các cột mới được thêm vào entities:
- **Bảng Cars**: `Status`, `DepositPercent`, `RentalLocationId`
- **Bảng Payments**: `TxnRef`, `TransactionNo`

## Cách sửa:

### Cách 1: Chạy SQL Script (Khuyến nghị)

1. Mở **SQL Server Management Studio** hoặc công cụ SQL khác
2. Kết nối đến database `EVSDB`
3. Mở và chạy file: `Repository/Context/Migrations/AddCarAndPaymentFields.sql`

Script này sẽ tự động kiểm tra và thêm các cột nếu chưa tồn tại.

### Cách 2: Chạy Migration bằng Entity Framework

1. **Tắt Visual Studio** (để giải phóng file lock)
2. Mở **Command Prompt** hoặc **PowerShell**
3. Chạy lệnh:

```bash
cd C:\SWP_PRJ\EVStation-basedRentalSystem
dotnet ef database update --project Repository --startup-project EVStation-basedRentalSystem --context EVSDbContext
```

### Cách 3: Chạy SQL trực tiếp

Nếu không có file script, chạy các lệnh SQL sau:

```sql
-- Thêm cột vào bảng Cars
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cars]') AND name = 'Status')
    ALTER TABLE [dbo].[Cars] ADD [Status] int NOT NULL DEFAULT 1;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cars]') AND name = 'DepositPercent')
    ALTER TABLE [dbo].[Cars] ADD [DepositPercent] int NOT NULL DEFAULT 0;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cars]') AND name = 'RentalLocationId')
    ALTER TABLE [dbo].[Cars] ADD [RentalLocationId] int NULL;

-- Thêm cột vào bảng Payments
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'TxnRef')
    ALTER TABLE [dbo].[Payments] ADD [TxnRef] nvarchar(max) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'TransactionNo')
    ALTER TABLE [dbo].[Payments] ADD [TransactionNo] nvarchar(max) NULL;

-- Cập nhật giá trị mặc định cho các record hiện có
UPDATE [dbo].[Cars]
SET [Status] = 1, [DepositPercent] = 0
WHERE [Status] IS NULL OR [DepositPercent] IS NULL;
```

## Sau khi chạy script:

1. **Restart ứng dụng** (nếu đang chạy)
2. **Test lại API** `/api/Car` - lỗi 500 sẽ biến mất

## Kiểm tra:

Chạy query sau để kiểm tra các cột đã được thêm:

```sql
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Cars'
AND COLUMN_NAME IN ('Status', 'DepositPercent', 'RentalLocationId');

SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Payments'
AND COLUMN_NAME IN ('TxnRef', 'TransactionNo');
```

