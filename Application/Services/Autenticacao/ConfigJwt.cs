namespace Application.Services.Autenticacao;

public class ConfigJwt
{
	public const string NomeSecao = "ConfigJwt";

	public string SecretKey { get; init; } = null!;
	public string Audience { get; init; } = null!;
	public string Issuer { get; init; } = null!;
	public int TempoExpiracaoEmMin { get; init; }
}