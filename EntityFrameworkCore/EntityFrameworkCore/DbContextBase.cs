using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meyer.Common.EntityFrameworkCore;

/// <inheritdoc/>
public abstract class DbContextBase : DbContext
{
    private static readonly HashSet<(Type CrlType, string PgTypeName)> mappings = new();

    /// <inheritdoc/>
    public DbContextBase(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Maps a CLR enum to a PostgreSQL enum type.
    /// </summary>
    /// <typeparam name="TEnum">The .NET enum type to be mapped</typeparam>
    /// <param name="pgTypeName">A PostgreSQL type name for the corresponding enum type in the database</param>
    protected static void MapPostgresEnum<TEnum>(string pgTypeName) where TEnum : struct, Enum
    {
        var type = typeof(TEnum);

        var wasAdded = mappings.Add((type, pgTypeName));

        if (!wasAdded)
            return;

        if (mappings.Count(x => x.CrlType == type) > 1)
            throw new ArgumentException($"Duplicate enum mapping for type {typeof(TEnum).Name}");

        NpgsqlConnection.GlobalTypeMapper.MapEnum<TEnum>(pgTypeName);
    }

    /// <summary>
    /// Creates a LINQ query based on a raw SQL query.
    /// If the database provider supports composing on the supplied SQL, you can compose
    /// on top of the raw SQL query using LINQ operators: context.Blogs.FromSqlRaw("SELECT * FROM dbo.Blogs").OrderBy(b =>
    /// b.Name).
    /// As with any API that accepts SQL it is important to parameterize any user input
    /// to protect against a SQL injection attack. You can include parameter place holders
    /// in the SQL query string and then supply parameter values as additional arguments.
    /// Any parameter values you supply will automatically be converted to a DbParameter:
    /// context.Blogs.FromSqlRaw("SELECT * FROM [dbo].[SearchBlogs]({0})", userSuppliedSearchTerm)
    /// However, never pass a concatenated or interpolated string ($"") with non-validated
    /// user-provided values into this method. Doing so may expose your application to
    /// SQL injection attacks. To use the interpolated string syntax, consider using
    /// Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSqlInterpolated``1(Microsoft.EntityFrameworkCore.DbSet{``0},System.FormattableString)
    /// to create parameters.
    /// This overload also accepts DbParameter instances as parameter values. This allows
    /// you to use named parameters in the SQL query string:
    /// context.Blogs.FromSqlRaw("SELECT * FROM [dbo].[SearchBlogs]({@searchTerm})",
    /// new SqlParameter("@searchTerm", userSuppliedSearchTerm))
    /// </summary>
    /// <typeparam name="TEntity">The type of the elements of source.</typeparam>
    /// <param name="source">
    /// An System.Linq.IQueryable`1 to use as the base of the raw SQL query (typically a Microsoft.EntityFrameworkCore.DbSet`1).
    /// </param>
    /// <param name="sql">The raw SQL query.</param>
    /// <param name="parameters">The values to be assigned to parameters.</param>
    /// <returns>An System.Linq.IQueryable`1 representing the raw SQL query.</returns>
    public virtual IQueryable<TEntity> FromSqlRaw<TEntity>(DbSet<TEntity> source, string sql, params object[] parameters) where TEntity : class
    {
        return source.FromSqlRaw(sql, parameters);
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// This method will automatically call Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges
    /// to discover any changes to entity instances before saving to the underlying database.
    /// This can be disabled via Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled.
    /// Multiple active operations on the same context instance are not supported. Use
    /// 'await' to ensure that any asynchronous operations have completed before calling
    /// another method on this context.
    /// </summary>
    /// <param name="cancellationToken">
    /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous save operation. The task result contains
    /// the number of state entries written to the database.
    /// </returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.TrackAll || Database.IsInMemory()
                ? await base.SaveChangesAsync(cancellationToken)
                : throw new InvalidOperationException("This DBContext is read only.");
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException)
        {
            var exception = e.InnerException as PostgresException;

            switch (exception?.SqlState)
            {
                case PostgresErrorCodes.UniqueViolation:
                    throw new ContextConstraintException(ConstraintViolationTypes.UniqueViolation, e.Message, e);
                case PostgresErrorCodes.ForeignKeyViolation:
                    throw new ContextConstraintException(ConstraintViolationTypes.ForeignKeyViolation, e.Message, e);
                default:
                    throw;
            }
        }
        catch (DbUpdateConcurrencyException e)
        {
            throw new ContextConcurrencyException(e.Message, e);
        }
    }
}