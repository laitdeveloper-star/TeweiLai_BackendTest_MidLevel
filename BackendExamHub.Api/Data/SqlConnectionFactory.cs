using Microsoft.Data.SqlClient;

namespace BackendExamHub.Api.Data;

public interface ISqlConnectionFactory
{
    SqlConnection Create();
}

public sealed class SqlConnectionFactory(IConfiguration configuration) : ISqlConnectionFactory
{
    public SqlConnection Create()
    {
        var connectionString = configuration.GetConnectionString("BackendExamHub")
            ?? throw new InvalidOperationException("Missing connection string: BackendExamHub");

        return new SqlConnection(connectionString);
    }
}
