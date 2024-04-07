using Application.Common.Interfaces.Entidades.Locacoes.DTOs;

namespace Application.Common.Interfaces.Entidades.Locacoes;

public interface ILocacaoService
{
	Task<LocacaoResponse> ObterPorIdAsync(int locacaoId);
	Task<LocacaoResponse> CadastrarAsync(CriarLocacaoRequest criarLocacaoRequest, int idLocador);
	Task<LocacaoResponse> EditarAsync(EditarLocacaoRequest editarLocacaoRequest, int idLocador, int idLocacao);
	Task DeletarAsync(int idLocacao, int idUsuario);
	Task<LocacaoResponse> AssinarAsync(int idLocacao, int idUsuario);
}