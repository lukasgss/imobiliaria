using Application.Common.Interfaces.GenericRepository;
using Domain.Entidades;

namespace Application.Common.Interfaces.Entidades.Imoveis;

public interface IImovelRepository : IGenericRepository<Imovel>
{
	Task<Imovel?> ObterPorIdAsync(int imovelId);
	Task<IEnumerable<Imovel>> ObterImoveisAlugados();
	Task<IEnumerable<Imovel>> ObterDisponiveisParaAluguel();
}