using Application.Common.Interfaces.Entidades.DTOs;

namespace Application.Common.Interfaces.Entidades;

public interface IUsuarioService
{
	Task<UsuarioResponse> ObterUsuarioPorIdAsync(int idUsuario);
	Task<UsuarioLoginResponse> CadastrarAsync(CriarUsuarioRequest request);
	Task<UsuarioLoginResponse> LoginAsync(LoginRequest request);
	Task<UsuarioResponse> EditarAsync(int idUsuarioLogado, int idUsuarioRota, EditarUsuarioRequest request);
	Task DeletarAsync(int idUsuarioLogado, int idUsuarioADeletar);
}