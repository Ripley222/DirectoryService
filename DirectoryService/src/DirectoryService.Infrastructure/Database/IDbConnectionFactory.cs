using System.Data;

namespace DirectoryService.Infrastructure.Database;

public interface IDbConnectionFactory
{
    public Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}