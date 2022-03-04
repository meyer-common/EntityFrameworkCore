using System;

namespace Meyer.Common.EntityFrameworkCore;

/// <summary>
/// Represents an exception thrown when a concurrency violation is encountered while
/// saving to the database. A concurrency violation occurs when an unexpected number
/// of rows are affected during save. This is usually because the data in the database
/// has been modified since it was loaded into memory.
/// Ex: entity was already deleted from the database or never existed
/// </summary>
public class ContextConcurrencyException : Exception
{
    /// <inheritdoc/>
    public ContextConcurrencyException(string message) : base(message) { }

    /// <inheritdoc/>
    public ContextConcurrencyException(string message, Exception innerException) 
        : base(message, innerException) { }
}