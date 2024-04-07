namespace Application.Common.Interfaces.Autenticacao;

public interface IGeradorTokenJwt
{
	string GerarToken(int idUsuario, string nomeCompleto);
}