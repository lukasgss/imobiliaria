using System.Security.Claims;

namespace Application.Common.Interfaces.Autorizacao;

public interface IAutorizacaoUsuarioService
{
	int ObterIdUsuarioPorTokenJwt(ClaimsPrincipal user);
}