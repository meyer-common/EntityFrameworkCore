namespace Meyer.Common.EntityFrameworkCore;

/// <summary>
/// Represents application configuration for a database
/// </summary>
public class DbContextConnectionOptions
{
    public const string SectionName = nameof(DbContextConnectionOptions);

    /// <summary>
    /// Gets or sets the host address of a read-only instance
    /// </summary>
    public string ReadHost { get; set; }

    /// <summary>
    /// Gets or sets the host address of a writable instance
    /// </summary>
    public string WriteHost { get; set; }

    /// <summary>
    /// Gets or sets the port number of the instance
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the name of the target database
    /// </summary>
    public string Database { get; set; }

    /// <summary>
    /// Gets or sets the username used to connect
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password used to connect
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets if the connection should be made to an in-memory database
    /// </summary>
    public bool UseInMemory { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether the channel will be encrypted while bypassing 
    /// walking the certificate chain to validate trust. 
    /// </summary>
    public bool TrustServerCertificate { get; set; }

    /// <summary>
    /// Gets or sets whether the connection requires ssl
    /// </summary>
    public string SslMode { get; set; }

    /// <summary>
    /// Gets or sets the Schemas to search
    /// </summary>
    public string SearchPath { get; set; }
}