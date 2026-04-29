USE [BackendExamHub];
GO

IF OBJECT_ID('dbo.MyOffice_ACPD', 'U') IS NOT NULL
    DROP TABLE dbo.MyOffice_ACPD;
GO

IF OBJECT_ID('dbo.MyOffice_ExcuteionLog', 'U') IS NOT NULL
    DROP TABLE dbo.MyOffice_ExcuteionLog;
GO

IF OBJECT_ID('dbo.NEWSID', 'P') IS NOT NULL
    DROP PROCEDURE dbo.NEWSID;
GO

IF OBJECT_ID('dbo.usp_AddLog', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_AddLog;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE TABLE [dbo].[MyOffice_ACPD](
    [ACPD_SID] [char](20) NOT NULL,
    [ACPD_Cname] [nvarchar](60) NULL,
    [ACPD_Ename] [nvarchar](40) NULL,
    [ACPD_Sname] [nvarchar](40) NULL,
    [ACPD_Email] [nvarchar](60) NULL,
    [ACPD_Status] [tinyint] NULL,
    [ACPD_Stop] [bit] NULL,
    [ACPD_StopMemo] [nvarchar](60) NULL,
    [ACPD_LoginID] [nvarchar](30) NULL,
    [ACPD_LoginPWD] [nvarchar](60) NULL,
    [ACPD_Memo] [nvarchar](600) NULL,
    [ACPD_NowDateTime] [datetime] NULL,
    [ACPD_NowID] [nvarchar](20) NULL,
    [ACPD_UPDDateTime] [datetime] NULL,
    [ACPD_UPDID] [nvarchar](20) NULL,
 CONSTRAINT [PK_MyOffice_ACPD] PRIMARY KEY CLUSTERED
(
    [ACPD_SID] ASC
));
GO

ALTER TABLE [dbo].[MyOffice_ACPD] ADD CONSTRAINT [DF_MyOffice_ACPD_acpd_status] DEFAULT ((0)) FOR [ACPD_Status];
GO

ALTER TABLE [dbo].[MyOffice_ACPD] ADD CONSTRAINT [DF_MyOffice_ACPD_acpd_stop] DEFAULT ((0)) FOR [ACPD_Stop];
GO

ALTER TABLE [dbo].[MyOffice_ACPD] ADD CONSTRAINT [DF_MyOffice_ACPD_acpd_nowdatetime] DEFAULT (GETDATE()) FOR [ACPD_NowDateTime];
GO

ALTER TABLE [dbo].[MyOffice_ACPD] ADD CONSTRAINT [DF_MyOffice_ACPD_acpd_upddatetime] DEFAULT (GETDATE()) FOR [ACPD_UPDDateTime];
GO

CREATE TABLE [dbo].[MyOffice_ExcuteionLog](
    [DeLog_AutoID] [bigint] IDENTITY(1,1) NOT NULL,
    [DeLog_StoredPrograms] [nvarchar](120) NOT NULL,
    [DeLog_GroupID] [uniqueidentifier] NOT NULL,
    [DeLog_isCustomDebug] [bit] NOT NULL,
    [DeLog_ExecutionProgram] [nvarchar](120) NOT NULL,
    [DeLog_ExecutionInfo] [nvarchar](max) NULL,
    [DeLog_verifyNeeded] [bit] NULL,
    [DeLog_ExDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_MOTC_DataExchangeLog] PRIMARY KEY CLUSTERED
(
    [DeLog_AutoID] ASC
)) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO

ALTER TABLE [dbo].[MyOffice_ExcuteionLog] ADD CONSTRAINT [DF_MOTC_DataExchangeLog_DeLog_isCustomDebug] DEFAULT ((0)) FOR [DeLog_isCustomDebug];
GO

ALTER TABLE [dbo].[MyOffice_ExcuteionLog] ADD CONSTRAINT [DF_MyOffice_ExcuteionLog_DeLog_verifyNeeded] DEFAULT ((0)) FOR [DeLog_verifyNeeded];
GO

ALTER TABLE [dbo].[MyOffice_ExcuteionLog] ADD CONSTRAINT [DF_MOTC_DataExchangeLog_DeLog_ExDateTime] DEFAULT (GETDATE()) FOR [DeLog_ExDateTime];
GO

CREATE PROCEDURE [dbo].[NEWSID]
(
    @TableName nvarchar(128),
    @ReturnSID nvarchar(20) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SIDRowName NVARCHAR(20);

    DECLARE
        @currentYear int,
        @dayOfYear int,
        @secondOfDay int,
        @alphabets char(36),
        @firstDigit char(1),
        @secondDigit char(1),
        @prefix char(2),
        @dayCode char(3),
        @secondCode char(5),
        @sql nvarchar(MAX),
        @randomValue char(10),
        @ParmDefinition nvarchar(500);

    DECLARE @tempTable TABLE
    (
        SID CHAR(20)
    );

    SET @currentYear = YEAR(GETDATE()) - 2000;
    SET @dayOfYear = DATEPART(DAYOFYEAR, GETDATE());
    SET @secondOfDay = DATEPART(SECOND, GETDATE()) + (60 * DATEPART(MINUTE, GETDATE())) + (3600 * DATEPART(HOUR, GETDATE()));

    IF (@currentYear > 1295)
    BEGIN
        SET @currentYear = 1295;
    END;

    SET @alphabets = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    SET @firstDigit = SUBSTRING(@alphabets, (@currentYear / 36) % 36 + 1, 1);
    SET @secondDigit = SUBSTRING(@alphabets, @currentYear % 36 + 1, 1);
    SET @prefix = @firstDigit + @secondDigit;
    SET @dayCode = RIGHT('000' + CONVERT(VARCHAR, @dayOfYear), 3);
    SET @secondCode = RIGHT('00000' + CONVERT(VARCHAR, @secondOfDay), 5);

    SELECT TOP 1
        @SIDRowName = STUFF((
            SELECT ', ' + c.name
            FROM sys.index_columns ic
            JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id
            ORDER BY ic.key_ordinal
            FOR XML PATH('')
        ), 1, 2, '')
    FROM sys.indexes i
    WHERE i.object_id = OBJECT_ID(@TableName) AND i.index_id > 0;

    WHILE 1 = 1
    BEGIN
        SET @randomValue = RIGHT('0000000000' + CAST(ABS(CAST(CAST(NEWID() AS BINARY(5)) AS BIGINT)) % 10000000000 AS VARCHAR(10)), 10);
        SET @ReturnSID = @prefix + @dayCode + @secondCode + @randomValue;
        SET @sql = N'SELECT @SIDRowNameOUT = ' + QUOTENAME(@SIDRowName) + ' FROM ' + QUOTENAME(@TableName) + ' WHERE ' + QUOTENAME(@SIDRowName) + ' = @ReturnSIDIN';
        SET @ParmDefinition = N'@ReturnSIDIN nvarchar(20), @SIDRowNameOUT nvarchar(20) OUTPUT';

        DELETE FROM @tempTable;

        DECLARE @SIDRowNameOUT nvarchar(20);
        EXEC sp_executesql @sql, @ParmDefinition, @ReturnSIDIN = @ReturnSID, @SIDRowNameOUT = @SIDRowNameOUT OUTPUT;

        IF @SIDRowNameOUT IS NULL
            BREAK;
    END;
END;
GO

CREATE PROCEDURE [dbo].[usp_AddLog]
(
    @_InBox_ReadID tinyint,
    @_InBox_SPNAME nvarchar(120),
    @_InBox_GroupID uniqueidentifier,
    @_InBox_ExProgram nvarchar(40),
    @_InBox_ActionJSON nvarchar(Max),
    @_OutBox_ReturnValues nvarchar(Max) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF (@_InBox_ReadID = 0)
    BEGIN
        INSERT INTO dbo.MyOffice_ExcuteionLog
        (
            DeLog_StoredPrograms,
            DeLog_GroupID,
            DeLog_ExecutionProgram,
            DeLog_ExecutionInfo
        )
        VALUES
        (
            @_InBox_SPNAME,
            @_InBox_GroupID,
            @_InBox_ExProgram,
            @_InBox_ActionJSON
        );

        SET @_OutBox_ReturnValues =
        (
            SELECT TOP 100
                DeLog_AutoID AS 'AutoID',
                DeLog_ExecutionProgram AS 'NAME',
                DeLog_ExecutionInfo AS 'Action',
                DeLog_ExDateTime AS 'DateTime'
            FROM dbo.MyOffice_ExcuteionLog WITH (NOLOCK)
            WHERE DeLog_GroupID = @_InBox_GroupID
            ORDER BY DeLog_AutoID
            FOR JSON PATH, ROOT('ProgramLog'), INCLUDE_NULL_VALUES
        );

        RETURN;
    END;
END;
GO
