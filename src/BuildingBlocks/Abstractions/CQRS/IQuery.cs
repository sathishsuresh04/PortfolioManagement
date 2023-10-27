using MediatR;

namespace BuildingBlocks.Abstractions.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse>
where TResponse : class
{
}
