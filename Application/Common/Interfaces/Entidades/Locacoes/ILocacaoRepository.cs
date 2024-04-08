using Application.Common.Interfaces.Entidades.Locacoes.DTOs;
using Application.Common.Interfaces.GenericRepository;
using Domain.Entidades;

namespace Application.Common.Interfaces.Entidades.Locacoes;

public interface ILocacaoRepository : IGenericRepository<Locacao>
{
	Task<Locacao?> ObterPorIdAsync(int idLocacao);
	Task<Locacao?> ObterPorIdDoImovelAsync(int idImovel);

	Task<Locacao?> EditarLocacao(
		Locacao locacao,
		Imovel imovel,
		Usuario locatario,
		EditarLocacaoRequest editarLocacaoRequest);
}