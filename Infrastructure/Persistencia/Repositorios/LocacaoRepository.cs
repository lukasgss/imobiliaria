using Application.Common.Interfaces.Entidades.Locacoes;
using Application.Common.Interfaces.Entidades.Locacoes.DTOs;
using Domain.Entidades;
using Infrastructure.Persistencia.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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

	public async Task<Locacao?> EditarLocacao(
		Locacao locacao,
		Imovel imovel,
		Usuario locatario,
		EditarLocacaoRequest editarLocacaoRequest)
	{
		await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync();

		try
		{
			locacao.Imovel = imovel;
			locacao.Locatario = locatario;
			locacao.DataVencimento = editarLocacaoRequest.DataVencimento;
			locacao.ValorMensal = editarLocacaoRequest.ValorMensal;
			// Quando o locatário alterar dados da locação, é necessário que ambos assinem novamente.
			// Data de fechamento é setada como nulo, indicando como se não houvesse sido assinado
			// por ambas as partes ainda
			locacao.LocadorAssinou = false;
			locacao.LocatarioAssinou = false;
			locacao.DataFechamento = null;
			await _dbContext.SaveChangesAsync();

			// Ao alterar a locação, também é removido o inquilino da mesma, visto que ainda não foi assinado
			// o novo contrato proposto.
			imovel.Inquilino = null;
			await _dbContext.SaveChangesAsync();

			await transaction.CommitAsync();

			return locacao;
		}
		catch
		{
			await transaction.RollbackAsync();
			return null;
		}
	}
}