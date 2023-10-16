using MediatR;

namespace BuildingBlocks.Abstractions.CQRS;

public interface ICommand : IRequest
{
}

public interface ICommand<out TResponse> : IRequest<TResponse>
where TResponse : class
{
}
