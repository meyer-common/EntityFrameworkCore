using Npgsql;
using System;

namespace Meyer.Common.EntityFrameworkCore;

public static class DbConnectionFactory
{
    public static NpgsqlConnectionStringBuilder GetConnectionStringBuilder(DbConnectionOptions connectionOptions, DatabaseAccess databaseAccess)
    {
        return new NpgsqlConnectionStringBuilder
        {
            Host = databaseAccess == DatabaseAccess.ReadWrite
                ? connectionOptions.WriteHost
                : connectionOptions.ReadHost,
            Port = connectionOptions.Port,
            Database = connectionOptions.Database,
            Username = connectionOptions.Username,
            Password = connectionOptions.Password,
            SearchPath = connectionOptions.SearchPath,
            TrustServerCertificate = connectionOptions.TrustServerCertificate,
            SslMode = Enum.Parse<SslMode>(connectionOptions.SslMode, true)
        };
    }

    public enum DatabaseAccess
    {
        Read,
        ReadWrite
    }
}