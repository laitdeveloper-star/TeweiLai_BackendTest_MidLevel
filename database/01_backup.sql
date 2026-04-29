USE master;
GO

DECLARE @DatabaseName sysname = N'BackendExamHub';
DECLARE @BackupPath nvarchar(4000) = N'C:\SQLBackups\BackendExamHub.bak';

BACKUP DATABASE [BackendExamHub]
TO DISK = @BackupPath
WITH
    INIT,
    FORMAT,
    COMPRESSION,
    STATS = 10;
GO
