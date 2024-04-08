using System.Net;
using System.Text.Json;
using Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Middlewares;

public class TratamentoErrosMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<TratamentoErrosMiddleware> _logger;

	public TratamentoErrosMiddleware(RequestDelegate next, ILogger<TratamentoErrosMiddleware> logger)
	{
		_next = next ?? throw new ArgumentNullException(nameof(next));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception exception)
		{
			await HandleExceptionAsync(context, exception);
		}
	}

	private async Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		HttpStatusCode statusCode;
		string message;

		switch (exception)
		{
			case BadRequestException:
				statusCode = HttpStatusCode.BadRequest;
				message = exception.Message;
				break;
			case NotFoundException:
				statusCode = HttpStatusCode.NotFound;
				message = exception.Message;
				break;
			case ConflictException:
				statusCode = HttpStatusCode.Conflict;
				message = exception.Message;
				break;
			case LockedException:
				statusCode = HttpStatusCode.Locked;
				message = exception.Message;
				break;
			case UnauthorizedException:
				statusCode = HttpStatusCode.Unauthorized;
				message = exception.Message;
				break;
			case ForbiddenException:
				statusCode = HttpStatusCode.Forbidden;
				message = exception.Message;
				break;
			default:
				statusCode = HttpStatusCode.InternalServerError;
				message = "Erro interno no sistema, tente novamente mais tarde.";
				break;
		}

		_logger.LogError("{@Exception}", exception);
		context.Response.StatusCode = (int)statusCode;
		await context.Response.WriteAsync(JsonSerializer.Serialize(new { statusCode, message }));
	}
}