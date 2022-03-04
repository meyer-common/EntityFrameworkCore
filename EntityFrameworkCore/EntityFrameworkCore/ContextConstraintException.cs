using System;
using System.Data;

namespace Meyer.Common.EntityFrameworkCore;

/// <summary>
/// Represents an exception thrown when the backend reports constraint violations. Ex: foreign key or unique constraint
/// </summary>
public class ContextConstraintException : ConstraintException
{
    /// <summary>
    /// Gets the violated constraint type reported by the backend
    /// </summary>
    public ConstraintViolationTypes ConstraintErrorCodes { get; }

    /// <inheritdoc/>
    public ContextConstraintException(ConstraintViolationTypes constraintType, string message, Exception innerException)
        : base(message, innerException)
    {
        ConstraintErrorCodes = constraintType;
    }
}

/// <summary>
/// Represents types of constraint violations
/// </summary>
public enum ConstraintViolationTypes
{
    UniqueViolation,
    ForeignKeyViolation
}