using Application.Common.Interfaces.Autenticacao;
using Application.Common.Interfaces.Entidades;
using Application.Common.Interfaces.Providers;
using Application.Common.Providers;
using Application.Services.Autenticacao;
using Application.Services.Entidades;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddScoped<IUsuarioService, UsuarioService>();

		services.AddSingleton<IGeradorTokenJwt, GeradorTokenJwt>();
		services.AddSingleton<IValueProvider, ValueProvider>();

		return services;
	}
}