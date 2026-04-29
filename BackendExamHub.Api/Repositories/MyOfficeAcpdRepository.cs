using System.Data;
using BackendExamHub.Api.Data;
using BackendExamHub.Api.Models;
using Microsoft.Data.SqlClient;

namespace BackendExamHub.Api.Repositories;

public sealed class MyOfficeAcpdRepository(ISqlConnectionFactory connectionFactory) : IMyOfficeAcpdRepository
{
    public async Task<IReadOnlyList<MyOfficeAcpd>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                ACPD_SID,
                ACPD_Cname,
                ACPD_Ename,
                ACPD_Sname,
                ACPD_Email,
                ACPD_Status,
                ACPD_Stop,
                ACPD_StopMemo,
                ACPD_LoginID,
                ACPD_LoginPWD,
                ACPD_Memo,
                ACPD_NowDateTime,
                ACPD_NowID,
                ACPD_UPDDateTime,
                ACPD_UPDID
            FROM dbo.MyOffice_ACPD
            ORDER BY ACPD_NowDateTime DESC, ACPD_SID DESC;
            """;

        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var results = new List<MyOfficeAcpd>();
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(Map(reader));
        }

        return results;
    }

    public async Task<MyOfficeAcpd?> GetByIdAsync(string sid, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                ACPD_SID,
                ACPD_Cname,
                ACPD_Ename,
                ACPD_Sname,
                ACPD_Email,
                ACPD_Status,
                ACPD_Stop,
                ACPD_StopMemo,
                ACPD_LoginID,
                ACPD_LoginPWD,
                ACPD_Memo,
                ACPD_NowDateTime,
                ACPD_NowID,
                ACPD_UPDDateTime,
                ACPD_UPDID
            FROM dbo.MyOffice_ACPD
            WHERE ACPD_SID = @sid;
            """;

        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@sid", sid);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<MyOfficeAcpd> CreateAsync(MyOfficeAcpdUpsertRequest request, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            var sid = await GenerateSidAsync(connection, transaction, cancellationToken);

            const string insertSql = """
                INSERT INTO dbo.MyOffice_ACPD
                (
                    ACPD_SID,
                    ACPD_Cname,
                    ACPD_Ename,
                    ACPD_Sname,
                    ACPD_Email,
                    ACPD_Status,
                    ACPD_Stop,
                    ACPD_StopMemo,
                    ACPD_LoginID,
                    ACPD_LoginPWD,
                    ACPD_Memo,
                    ACPD_NowDateTime,
                    ACPD_NowID,
                    ACPD_UPDDateTime,
                    ACPD_UPDID
                )
                VALUES
                (
                    @sid,
                    @cname,
                    @ename,
                    @sname,
                    @email,
                    @status,
                    @stop,
                    @stopMemo,
                    @loginId,
                    @loginPwd,
                    @memo,
                    GETDATE(),
                    @operatorId,
                    GETDATE(),
                    @operatorId
                );
                """;

            await using var insertCommand = new SqlCommand(insertSql, connection, (SqlTransaction)transaction);
            BindUpsertParameters(insertCommand, request, sid);
            await insertCommand.ExecuteNonQueryAsync(cancellationToken);

            await LogAsync(connection, transaction, "CREATE", sid, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return (await GetByIdAsync(sid, cancellationToken))!;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(string sid, MyOfficeAcpdUpsertRequest request, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            const string updateSql = """
                UPDATE dbo.MyOffice_ACPD
                SET
                    ACPD_Cname = @cname,
                    ACPD_Ename = @ename,
                    ACPD_Sname = @sname,
                    ACPD_Email = @email,
                    ACPD_Status = @status,
                    ACPD_Stop = @stop,
                    ACPD_StopMemo = @stopMemo,
                    ACPD_LoginID = @loginId,
                    ACPD_LoginPWD = @loginPwd,
                    ACPD_Memo = @memo,
                    ACPD_UPDDateTime = GETDATE(),
                    ACPD_UPDID = @operatorId
                WHERE ACPD_SID = @sid;
                """;

            await using var updateCommand = new SqlCommand(updateSql, connection, (SqlTransaction)transaction);
            BindUpsertParameters(updateCommand, request, sid);
            var rows = await updateCommand.ExecuteNonQueryAsync(cancellationToken);
            if (rows == 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }

            await LogAsync(connection, transaction, "UPDATE", sid, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string sid, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            const string deleteSql = """
                DELETE FROM dbo.MyOffice_ACPD
                WHERE ACPD_SID = @sid;
                """;

            await using var deleteCommand = new SqlCommand(deleteSql, connection, (SqlTransaction)transaction);
            deleteCommand.Parameters.AddWithValue("@sid", sid);
            var rows = await deleteCommand.ExecuteNonQueryAsync(cancellationToken);
            if (rows == 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }

            await LogAsync(connection, transaction, "DELETE", sid, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static MyOfficeAcpd Map(SqlDataReader reader)
    {
        return new MyOfficeAcpd
        {
            AcpdSid = reader.GetString(reader.GetOrdinal("ACPD_SID")),
            AcpdCname = ReadNullableString(reader, "ACPD_Cname"),
            AcpdEname = ReadNullableString(reader, "ACPD_Ename"),
            AcpdSname = ReadNullableString(reader, "ACPD_Sname"),
            AcpdEmail = ReadNullableString(reader, "ACPD_Email"),
            AcpdStatus = ReadNullableByte(reader, "ACPD_Status"),
            AcpdStop = ReadNullableBool(reader, "ACPD_Stop"),
            AcpdStopMemo = ReadNullableString(reader, "ACPD_StopMemo"),
            AcpdLoginId = ReadNullableString(reader, "ACPD_LoginID"),
            AcpdLoginPwd = ReadNullableString(reader, "ACPD_LoginPWD"),
            AcpdMemo = ReadNullableString(reader, "ACPD_Memo"),
            AcpdNowDateTime = ReadNullableDateTime(reader, "ACPD_NowDateTime"),
            AcpdNowId = ReadNullableString(reader, "ACPD_NowID"),
            AcpdUpdDateTime = ReadNullableDateTime(reader, "ACPD_UPDDateTime"),
            AcpdUpdId = ReadNullableString(reader, "ACPD_UPDID")
        };
    }

    private static void BindUpsertParameters(SqlCommand command, MyOfficeAcpdUpsertRequest request, string sid)
    {
        command.Parameters.AddWithValue("@sid", sid);
        command.Parameters.AddWithValue("@cname", (object?)request.AcpdCname ?? DBNull.Value);
        command.Parameters.AddWithValue("@ename", (object?)request.AcpdEname ?? DBNull.Value);
        command.Parameters.AddWithValue("@sname", (object?)request.AcpdSname ?? DBNull.Value);
        command.Parameters.AddWithValue("@email", (object?)request.AcpdEmail ?? DBNull.Value);
        command.Parameters.AddWithValue("@status", (object?)request.AcpdStatus ?? DBNull.Value);
        command.Parameters.AddWithValue("@stop", (object?)request.AcpdStop ?? DBNull.Value);
        command.Parameters.AddWithValue("@stopMemo", (object?)request.AcpdStopMemo ?? DBNull.Value);
        command.Parameters.AddWithValue("@loginId", (object?)request.AcpdLoginId ?? DBNull.Value);
        command.Parameters.AddWithValue("@loginPwd", (object?)request.AcpdLoginPwd ?? DBNull.Value);
        command.Parameters.AddWithValue("@memo", (object?)request.AcpdMemo ?? DBNull.Value);
        command.Parameters.AddWithValue("@operatorId", (object?)request.OperatorId ?? DBNull.Value);
    }

    private static async Task<string> GenerateSidAsync(SqlConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        await using var command = new SqlCommand("dbo.NEWSID", connection, (SqlTransaction)transaction)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@TableName", "dbo.MyOffice_ACPD");

        var output = new SqlParameter("@ReturnSID", SqlDbType.NVarChar, 20)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(output);

        await command.ExecuteNonQueryAsync(cancellationToken);
        return output.Value?.ToString() ?? throw new InvalidOperationException("NEWSID did not return a SID.");
    }

    private static async Task LogAsync(
        SqlConnection connection,
        IDbTransaction transaction,
        string action,
        string sid,
        CancellationToken cancellationToken)
    {
        await using var command = new SqlCommand("dbo.usp_AddLog", connection, (SqlTransaction)transaction)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@_InBox_ReadID", 0);
        command.Parameters.AddWithValue("@_InBox_SPNAME", "BackendExamHub.Api");
        command.Parameters.AddWithValue("@_InBox_GroupID", Guid.NewGuid());
        command.Parameters.AddWithValue("@_InBox_ExProgram", action);
        command.Parameters.AddWithValue("@_InBox_ActionJSON", $$"""{"sid":"{{sid}}","action":"{{action}}"}""");

        var output = new SqlParameter("@_OutBox_ReturnValues", SqlDbType.NVarChar, -1)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(output);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static string? ReadNullableString(SqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    private static byte? ReadNullableByte(SqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetByte(ordinal);
    }

    private static bool? ReadNullableBool(SqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetBoolean(ordinal);
    }

    private static DateTime? ReadNullableDateTime(SqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
    }
}
