using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Validator;
using FluentValidation;
using PortfolioService.Portfolios.Data.Abstractions;
using PortfolioService.Portfolios.Exceptions;
using PortfolioService.Portfolios.ValueObjects;

namespace PortfolioService.Portfolios.Features.DeletePortfolioById;

public record DeletePortfolioById(string Id) : ICommand
{
    internal sealed class DeletePortfolioByIdValidator : BaseValidator<DeletePortfolioById>
    {
        public DeletePortfolioByIdValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required!");
        }
    }
}

public sealed class DeletePortfolioByIdHandler : ICommandHandler<DeletePortfolioById>
{
    private readonly IPortfolioRepository _portfolioRepository;

    public DeletePortfolioByIdHandler(IPortfolioRepository portfolioRepository)
    {
        _portfolioRepository = portfolioRepository;
    }

    public async Task Handle(DeletePortfolioById request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);
        Guard.Against.Null(request.Id);
        var portfolio = await _portfolioRepository.GetByIdAsync(PortfolioId.Of(request.Id));
        Guard.Against.NotFound(portfolio, new PortfolioNotFoundException(request.Id));

        portfolio.SetSoftDelete(DateTime.UtcNow);
        _portfolioRepository.Update(portfolio);
        // _portfolioRepository.Remove(PortfolioId.Of(request.Id));
        await _portfolioRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
