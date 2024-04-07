using Application.Common.Interfaces.Entidades.Locacoes.DTOs;
using Domain.Entidades;

namespace Application.Common.Extensions.Mapeamento;

public static class MapeamentoLocacao
{
	public static LocacaoResponse ToLocacaoResponse(this Locacao locacao)
	{
		return new LocacaoResponse(Id: locacao.Id,
			Imovel: locacao.Imovel.ToImovelResponse(),
			Locador: locacao.Locador.ToUsuarioResponse(),
			Locatario: locacao.Locatario.ToUsuarioResponse(),
			LocadorAssinou: locacao.LocadorAssinou,
			LocatarioAssinou: locacao.LocatarioAssinou,
			DataFechamento: locacao.DataFechamento,
			DataVencimento: locacao.DataVencimento,
			ValorMensal: locacao.ValorMensal);
	}
}