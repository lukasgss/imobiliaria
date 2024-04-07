using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces.Autenticacao;
using Application.Common.Interfaces.Providers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Autenticacao;

public class GeradorTokenJwt : IGeradorTokenJwt
{
	private readonly IValueProvider _valueProvider;
	private readonly ConfigJwt _configJwt;

	public GeradorTokenJwt(IValueProvider valueProvider, IOptions<ConfigJwt> opcoesJwt)
	{
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
		_configJwt = opcoesJwt.Value ?? throw new ArgumentNullException(nameof(opcoesJwt));
	}

	public string GerarToken(int idUsuario, string nomeCompleto)
	{
		SigningCredentials signingCredentials = new(
			new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configJwt.SecretKey)),
			SecurityAlgorithms.HmacSha256);

		Claim[] claims =
		{
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new(JwtRegisteredClaimNames.Sub, idUsuario.ToString()),
			new(JwtRegisteredClaimNames.Name, nomeCompleto)
		};

		JwtSecurityToken jwtSecurityToken = new(
			issuer: _configJwt.Issuer,
			audience: _configJwt.Audience,
			claims: claims,
			expires: _valueProvider.UtcNow().AddMinutes(_configJwt.TempoExpiracaoEmMin),
			signingCredentials: signingCredentials);

		return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
	}
}