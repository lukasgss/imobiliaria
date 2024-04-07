using Application.Common.Interfaces.Entidades.Locacoes;
using Domain.Entidades;
using Infrastructure.Persistencia.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistencia.Repositorios;

public class LocacaoRepository : GenericRepository<Locacao>, ILocacaoRepository
{
	private readonly AppDbContext _dbContext;

	public LocacaoRepository(AppDbContext dbContext) : base(dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<Locacao?> ObterPorIdAsync(int idLocacao)
	{
		return await _dbContext.Locacoes
			.Include(locacao => locacao.Imovel)
			.ThenInclude(imovel => imovel.Proprietario)
			.Include(locacao => locacao.Locador)
			.Include(locacao => locacao.Locatario)
			.SingleOrDefaultAsync(locacao => locacao.Id == idLocacao);
	}

	public async Task<Locacao?> ObterPorIdDoImovelAsync(int idImovel)
	{
		return await _dbContext.Locacoes
			.Include(locacao => locacao.Imovel)
			.ThenInclude(imovel => imovel.Proprietario)
			.Include(locacao => locacao.Locador)
			.Include(locacao => locacao.Locatario)
			.SingleOrDefaultAsync(locacao => locacao.Imovel.Id == idImovel);
	}
}