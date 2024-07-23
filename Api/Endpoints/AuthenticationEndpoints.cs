using Api.Helpers;
using Api.Utils;
using Application.Common.Exceptions;
using Application.Common.Models;
using Application.UseCases.Auth.Commands.Login;
using Application.UseCases.Auth.Commands.Refresh;
using Application.UseCases.Auth.Commands.Register;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Endpoints;

public class AuthenticationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/auth")
            .WithTags("Authentication");

        group.MapPost("register", Register)
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict);
        
        group.MapPost("login", Login)
            .Produces<JwtToken>()
            .Produces(StatusCodes.Status400BadRequest);
        
        group.MapPost("refresh", Refresh)
            .Produces<JwtToken>()
            .Produces(StatusCodes.Status400BadRequest);
        
        group.MapPost("logout", Logout)
            .RequireAuthorization(AppConstants.BaseAuthPolicy)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private async Task<Results<Created<Guid>, Conflict<string>>> Register(RegisterCommand command, ISender sender)
    {
        try
        {
            var result = await sender.Send(command);
            return TypedResults.Created(result.ToString(), result);
        }
        catch (AlreadyExistException exception)
        {
            return TypedResults.Conflict(exception.Message);
        }
    }

    private async Task<Results<Ok<JwtToken>, BadRequest<string>>> Login(
        LoginCommand command, 
        ISender sender, 
        HttpContext context)
    {
        try
        {
            var result = await sender.Send(command);
          
            CookieManager.SetCookie(
                context.Response,
                AppConstants.RefreshCookieName,
                result.CookieToken.Token,
                result.CookieToken.Expires
            );
            
            return TypedResults.Ok(result.JwtToken);
        }
        catch (AccessDeniedException exception)
        {
            return TypedResults.BadRequest(exception.Message);
        }
    }

    private async Task<Results<Ok<JwtToken>, BadRequest<string>>> Refresh(ISender sender, HttpContext context)
    {
        try
        {
            var userRefreshToken = context.Request.Cookies[AppConstants.RefreshCookieName];
            var result = await sender.Send(new RefreshCommand(userRefreshToken));
            
            CookieManager.SetCookie(
                context.Response,
                AppConstants.RefreshCookieName,
                result.CookieToken.Token,
                result.CookieToken.Expires
            );

            return TypedResults.Ok(result.JwtToken);
        }
        catch (UnauthorizedException exception)
        {
            return TypedResults.BadRequest(exception.Message);
        }
    }

    private async Task<Results<NoContent, BadRequest<string>>> Logout(ISender sender, HttpContext context)
    {
        try
        {
            var userRefreshToken = context.Request.Cookies[AppConstants.RefreshCookieName];
            var result = await sender.Send(new RefreshCommand(userRefreshToken));
        
            CookieManager.RemoveCookie(context.Response, AppConstants.RefreshCookieName);

            return TypedResults.NoContent();
        }
        catch (UnauthorizedException exception)
        {
            return TypedResults.BadRequest(exception.Message);
        }
    }
}