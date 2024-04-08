using Application.Common.Interfaces.Entidades.Imoveis;
using Domain.Entidades;
using Infrastructure.Persistencia.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistencia.Repositorios;

public class ImovelRepository : GenericRepository<Imovel>, IImovelRepository
{
	private readonly AppDbContext _dbContext;

	public ImovelRepository(AppDbContext dbContext) : base(dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<Imovel?> ObterPorIdAsync(int imovelId)
	{
		return await _dbContext.Imoveis
			.Include(imovel => imovel.Proprietario)
			.Include(imovel => imovel.Corretor)
			.Include(imovel => imovel.Inquilino)
			.SingleOrDefaultAsync(imovel => imovel.Id == imovelId);
	}

	public async Task<IEnumerable<Imovel>> ObterImoveisAlugados()
	{
		return await _dbContext.Imoveis
			.Include(imovel => imovel.Proprietario)
			.Include(imovel => imovel.Corretor)
			.Include(imovel => imovel.Inquilino)
			.Where(imovel => imovel.Inquilino != null)
			.ToListAsync();
	}

	public async Task<IEnumerable<Imovel>> ObterDisponiveisParaAluguel()
	{
		return await _dbContext.Imoveis
			.Include(imovel => imovel.Proprietario)
			.Include(imovel => imovel.Corretor)
			.Include(imovel => imovel.Inquilino)
			.Where(imovel => imovel.Inquilino == null)
			.ToListAsync();
	}
}