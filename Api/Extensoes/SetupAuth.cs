using System.Text;
using Application.Services.Autenticacao;
using Domain.Entidades;
using Infrastructure.Persistencia.DataContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Api.Extensoes;

public static class SetupAuth
{
	public static void ConfigurarAuth(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddIdentity<Usuario, IdentityRole<int>>(options =>
			{
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequiredLength = 4;
				options.User.RequireUniqueEmail = true;
			})
			.AddEntityFrameworkStores<AppDbContext>()
			.AddDefaultTokenProviders();

		services.AddAuthentication(opt =>
			{
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				IConfigurationSection configJwt = configuration.GetSection(ConfigJwt.NomeSecao);
				string secret = configJwt["SecretKey"]!;

				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = configJwt["Issuer"],
					ValidAudience = configJwt["Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(secret)
					)
				};
			});

		services.Configure<ConfigJwt>(configuration.GetSection(ConfigJwt.NomeSecao));
	}
}