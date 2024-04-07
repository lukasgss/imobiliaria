using Application.Common.Interfaces.Entidades;
using Infrastructure.Persistencia.DataContext;
using Infrastructure.Persistencia.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<IUsuarioRepository, UsuarioRepository>();

		services.AddDbContext<AppDbContext>(options =>
			options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

		return services;
	}
}