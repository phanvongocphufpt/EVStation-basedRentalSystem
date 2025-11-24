# Script to fix database by adding missing columns
$connectionString = "Server=localhost;Database=EVSDB;User Id=sa;Password=12345;TrustServerCertificate=True;"

$sqlCommands = @(
    "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cars]') AND name = 'Status') BEGIN ALTER TABLE [dbo].[Cars] ADD [Status] int NOT NULL DEFAULT 1; END",
    "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cars]') AND name = 'DepositPercent') BEGIN ALTER TABLE [dbo].[Cars] ADD [DepositPercent] int NOT NULL DEFAULT 0; END",
    "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cars]') AND name = 'RentalLocationId') BEGIN ALTER TABLE [dbo].[Cars] ADD [RentalLocationId] int NULL; END",
    "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'TxnRef') BEGIN ALTER TABLE [dbo].[Payments] ADD [TxnRef] nvarchar(max) NULL; END",
    "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'TransactionNo') BEGIN ALTER TABLE [dbo].[Payments] ADD [TransactionNo] nvarchar(max) NULL; END",
    "UPDATE [dbo].[Cars] SET [Status] = 1, [DepositPercent] = 0 WHERE [Status] IS NULL OR [DepositPercent] IS NULL;"
)

$commandNames = @(
    "Adding Status column to Cars",
    "Adding DepositPercent column to Cars",
    "Adding RentalLocationId column to Cars",
    "Adding TxnRef column to Payments",
    "Adding TransactionNo column to Payments",
    "Updating existing Cars records"
)

try {
    Write-Host "Connecting to database..." -ForegroundColor Yellow
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "Connection successful!" -ForegroundColor Green
    Write-Host "Executing SQL commands..." -ForegroundColor Yellow
    
    for ($i = 0; $i -lt $sqlCommands.Length; $i++) {
        $command = $sqlCommands[$i]
        $commandName = $commandNames[$i]
        
        Write-Host ""
        Write-Host "$commandName..." -ForegroundColor Cyan
        $cmd = $connection.CreateCommand()
        $cmd.CommandText = $command
        try {
            $result = $cmd.ExecuteNonQuery()
            Write-Host "Success" -ForegroundColor Green
        }
        catch {
            if ($_.Exception.Message -match "already exists" -or $_.Exception.Message -match "duplicate") {
                Write-Host "Already exists, skipping" -ForegroundColor Yellow
            }
            else {
                Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
            }
        }
        finally {
            if ($cmd) { $cmd.Dispose() }
        }
    }
    
    $connection.Close()
    Write-Host ""
    Write-Host "Database update completed successfully!" -ForegroundColor Green
    Write-Host "Please restart your application." -ForegroundColor Yellow
}
catch {
    Write-Host "Error connecting to database: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please check:" -ForegroundColor Yellow
    Write-Host "1. SQL Server is running" -ForegroundColor Yellow
    Write-Host "2. Connection string is correct" -ForegroundColor Yellow
    Write-Host "3. Database EVSDB exists" -ForegroundColor Yellow
}
