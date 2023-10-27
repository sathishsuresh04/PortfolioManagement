using MediatR;

namespace BuildingBlocks.Abstractions.CQRS;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
where TQuery : IQuery<TResponse>
where TResponse : class
{
}
