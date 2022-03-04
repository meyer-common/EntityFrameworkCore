namespace Meyer.Common.EntityFrameworkCore;

/// <summary>
/// An enumeration of status results
/// </summary>
public enum ResultStatus
{
    Undefined,
    Success,
    Partial,
    Error,
    Conflict,
    NotFound,
    Added,
    Updated,
    Deleted
}