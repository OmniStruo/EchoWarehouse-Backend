using Npgsql;

namespace EchoWarehouse.Data;

public class EchoWarehouseDbContext
{
    private readonly NpgsqlDataSource _dataSource;

    public EchoWarehouseDbContext(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public ValueTask<NpgsqlConnection> OpenConnectionAsync()
    {
        return _dataSource.OpenConnectionAsync();
    }
}
