using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meyer.Common.EntityFrameworkCore;

public interface IMultiTenantDbContextFactory<T> where T : DbContext
{
    Task<T> CreateDbContextAsync(string schema = null, CancellationToken cancellationToken = default);
}

public class MultiTenantDbContextFactory<T> : IMultiTenantDbContextFactory<T> where T : DbContext
{
    private readonly IDbContextFactory<T> factory;
    private readonly DbConnectionOptions connectionOptions;

    public MultiTenantDbContextFactory(IDbContextFactory<T> factory, DbConnectionOptions connectionOptions)
    {
        this.factory = factory;
        this.connectionOptions = connectionOptions;
    }

    public async Task<T> CreateDbContextAsync(string schema = null, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(schema))
        {
            var newSearchPath = connectionOptions.SearchPath.Split(',').ToList();
            newSearchPath.Insert(0, schema.ToString());
            connectionOptions.SearchPath = string.Join(',', newSearchPath);
        }

        return await factory.CreateDbContextAsync(cancellationToken);
    }
}