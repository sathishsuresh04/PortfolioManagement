using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Validator;
using FluentValidation;
using MapsterMapper;
using PortfolioService.Portfolios.Data.Abstractions;
using PortfolioService.Portfolios.Dtos;
using PortfolioService.Portfolios.Exceptions;
using PortfolioService.Portfolios.Models.ValueObjects;

namespace PortfolioService.Portfolios.Features.GetPortfolioById;

public record GetPortfolioById(string Id) : IQuery<PortfolioDto>
{
    internal sealed class GetPortfolioByIdValidator : BaseValidator<GetPortfolioById>
    {
        public GetPortfolioByIdValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required!");
        }
    }
}

internal sealed class GetPortfolioByIdHandler : IQueryHandler<GetPortfolioById, PortfolioDto>
{
    private readonly IMapper _mapper;
    private readonly IPortfolioRepository _portfolioRepository;

    public GetPortfolioByIdHandler(IPortfolioRepository portfolioRepository, IMapper mapper)
    {
        _portfolioRepository = portfolioRepository;
        _mapper = mapper;
    }

    public async Task<PortfolioDto> Handle(GetPortfolioById request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);
        var portfolio = await _portfolioRepository.GetByIdAsync(PortfolioId.Of(request.Id));
        Guard.Against.NotFound(portfolio, new PortfolioNotFoundException(request.Id));
        var portfolioDto = _mapper.Map<PortfolioDto>(portfolio);
        return portfolioDto;
    }
}
