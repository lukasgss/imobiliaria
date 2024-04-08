using Application.Common.Interfaces.Entidades.Imoveis.DTOs;

namespace Application.Common.Interfaces.Entidades.Imoveis;

public interface IImovelService
{
	Task<ImovelResponse> ObterPorIdAsync(int imovelId);
	Task<IEnumerable<ImovelResponse>> ObterImoveisAlugados();
	Task<IEnumerable<ImovelResponse>> ObterImoveisDisponiveis();
	Task<ImovelResponse> CadastrarAsync(CriarImovelRequest request, int idUsuarioLogado);
	Task<ImovelResponse> EditarAsync(EditarImovelRequest request, int idUsuarioLogado, int idImovel);
	Task DeletarAsync(int idImovel, int idUsuarioLogado);
}