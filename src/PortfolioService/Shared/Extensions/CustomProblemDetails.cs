using BuildingBlocks.Core.Exceptions;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NotFoundException = Ardalis.GuardClauses.NotFoundException;

namespace PortfolioService.Shared.Extensions;

public static class CustomProblemDetails
{
    public static WebApplication UseCustomProblemDetails(this WebApplication app)
    {
        app.UseStatusCodePages(
            statusCodeHandlerApp =>
            {
                statusCodeHandlerApp.Run(
                    async context =>
                    {
                        context.Response.ContentType = "application/problem+json";
                        if (context.RequestServices.GetService<IProblemDetailsService>() is
                            {
                            } problemDetailsService)
                        {
                            await problemDetailsService.WriteAsync(
                                new ProblemDetailsContext
                                {
                                    HttpContext = context,
                                    ProblemDetails =
                                    {
                                        Detail = ReasonPhrases.GetReasonPhrase(context.Response.StatusCode),
                                        Status = context.Response.StatusCode,
                                    },
                                });
                        }
                    });
            });

        app.UseExceptionHandler(
            exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(
                    async context =>
                    {
                        context.Response.ContentType = "application/problem+json";

                        if (context.RequestServices.GetService<IProblemDetailsService>() is
                            {
                            } problemDetailsService)
                        {
                            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                            var exceptionType = exceptionHandlerFeature?.Error;

                            if (exceptionType is not null)
                            {
                                (string Detail, string Title, int StatusCode) details = exceptionType switch
                                    {
                                        ValidationException =>
                                            (
                                                exceptionType.Message,
                                                exceptionType.GetType().Name,
                                                context.Response.StatusCode = StatusCodes.Status400BadRequest
                                            ),
                                        DomainException exception => (
                                                                         exceptionType.Message,
                                                                         exceptionType.GetType().Name,
                                                                         context.Response.StatusCode =
                                                                             (int)exception.StatusCode
                                                                     ),
                                        ArgumentException => (
                                                                 exceptionType.Message,
                                                                 exceptionType.GetType().Name,
                                                                 context.Response.StatusCode =
                                                                     StatusCodes.Status400BadRequest
                                                             ),
                                        NotFoundException =>
                                            (
                                                exceptionType.Message,
                                                exceptionType.GetType().Name,
                                                context.Response.StatusCode = StatusCodes.Status404NotFound
                                            ),
                                        HttpRequestException exception => (
                                                                              exceptionType.Message,
                                                                              exceptionType.GetType().Name,
                                                                              context.Response.StatusCode =
                                                                                  (int)exception.StatusCode!
                                                                          ),
                                        DbUpdateConcurrencyException =>
                                            (
                                                exceptionType.Message,
                                                exceptionType.GetType().Name,
                                                context.Response.StatusCode = StatusCodes.Status409Conflict
                                            ),
                                        RpcException =>
                                            (
                                                exceptionType.Message,
                                                exceptionType.GetType().Name,
                                                context.Response.StatusCode = StatusCodes.Status400BadRequest
                                            ),
                                        _ =>
                                            (
                                                exceptionType.Message,
                                                exceptionType.GetType().Name,
                                                context.Response.StatusCode = StatusCodes.Status500InternalServerError
                                            ),
                                    };

                                var problem = new ProblemDetailsContext
                                              {
                                                  HttpContext = context,
                                                  ProblemDetails =
                                                  {
                                                      Title = details.Title,
                                                      Detail = details.Detail,
                                                      Status = details.StatusCode,
                                                  },
                                              };

                                if (app.Environment.IsDevelopment())
                                {
                                    problem.ProblemDetails.Extensions.Add(
                                        "exception",
                                        exceptionHandlerFeature?.Error.ToString());
                                }

                                await problemDetailsService.WriteAsync(problem);
                            }
                        }
                    });
            });


        return app;
    }
}
