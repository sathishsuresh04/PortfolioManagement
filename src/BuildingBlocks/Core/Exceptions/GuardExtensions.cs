using Ardalis.GuardClauses;

namespace BuildingBlocks.Core.Exceptions;

public static class GuardExtensions
{
    public static T NotFound<T>(this IGuardClause guardClause, T input, Exception exception)
    {
        if (input is null) throw exception;

        return input;
    }
}
