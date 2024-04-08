using Application.Common.Extensions.Mapeamento;
using Application.Common.Interfaces.Entidades.Locacoes.DTOs;
using Domain.Entidades;
using Tests.TesteUtils.Constantes;

namespace Tests.TesteUtils.GeradorEntidades;

public static class GeradorLocacao
{
	public static Locacao GerarLocacao()
	{
		return new Locacao()
		{
			Id = Constants.DadosLocacao.Id,
			Imovel = Constants.DadosLocacao.Imovel,
			ImovelId = Constants.DadosLocacao.ImovelId,
			Locador = Constants.DadosLocacao.Locador,
			LocadorId = Constants.DadosLocacao.LocadorId,
			Locatario = Constants.DadosLocacao.Locatario,
			LocatarioId = Constants.DadosLocacao.LocatarioId,
			LocadorAssinou = Constants.DadosLocacao.LocadorAssinou,
			LocatarioAssinou = Constants.DadosLocacao.LocatarioAssinou,
			DataFechamento = Constants.DadosLocacao.DataFechamento,
			DataVencimento = Constants.DadosLocacao.DataVencimento,
			ValorMensal = Constants.DadosLocacao.ValorMensal
		};
	}

	public static Locacao GerarLocacaoJaAssinada()
	{
		return new Locacao()
		{
			Id = Constants.DadosLocacao.Id,
			Imovel = Constants.DadosLocacao.Imovel,
			ImovelId = Constants.DadosLocacao.ImovelId,
			Locador = Constants.DadosLocacao.Locador,
			LocadorId = Constants.DadosLocacao.LocadorId,
			Locatario = Constants.DadosLocacao.Locatario,
			LocatarioId = Constants.DadosLocacao.LocatarioId,
			LocadorAssinou = true,
			LocatarioAssinou = true,
			DataFechamento = Constants.DadosLocacao.DataFechamentoLocacaoAssinada,
			DataVencimento = Constants.DadosLocacao.DataVencimento,
			ValorMensal = Constants.DadosLocacao.ValorMensal
		};
	}

	public static Locacao GerarLocacaoJaAssinadaPeloLocador()
	{
		return new Locacao()
		{
			Id = Constants.DadosLocacao.Id,
			Imovel = Constants.DadosLocacao.Imovel,
			ImovelId = Constants.DadosLocacao.ImovelId,
			Locador = Constants.DadosLocacao.Locador,
			LocadorId = Constants.DadosLocacao.LocadorId,
			Locatario = Constants.DadosLocacao.Locatario,
			LocatarioId = Constants.DadosLocacao.LocatarioId,
			LocadorAssinou = true,
			LocatarioAssinou = false,
			DataFechamento = Constants.DadosLocacao.DataFechamento,
			DataVencimento = Constants.DadosLocacao.DataVencimento,
			ValorMensal = Constants.DadosLocacao.ValorMensal
		};
	}

	public static CriarLocacaoRequest GerarCriarLocacaoRequest()
	{
		return new CriarLocacaoRequest()
		{
			IdImovel = Constants.DadosImovel.Id,
			IdLocatario = Constants.DadosLocacao.LocatarioId,
			DataVencimento = Constants.DadosLocacao.DataVencimento,
			ValorMensal = Constants.DadosLocacao.ValorMensal
		};
	}

	public static EditarLocacaoRequest GerarEditarLocacaoRequest()
	{
		return new EditarLocacaoRequest()
		{
			IdImovel = Constants.DadosImovel.Id,
			IdLocatario = Constants.DadosLocacao.LocatarioId,
			DataVencimento = Constants.DadosLocacao.DataVencimento,
			ValorMensal = Constants.DadosLocacao.ValorMensal
		};
	}

	public static LocacaoResponse GerarLocacaoResponse()
	{
		return new LocacaoResponse(
			Id: Constants.DadosLocacao.Id,
			Imovel: Constants.DadosLocacao.Imovel.ToImovelResponse(),
			Locador: Constants.DadosLocacao.Locador.ToUsuarioResponse(),
			Locatario: Constants.DadosLocacao.Locatario.ToUsuarioResponse(),
			LocadorAssinou: Constants.DadosLocacao.LocadorAssinou,
			LocatarioAssinou: Constants.DadosLocacao.LocatarioAssinou,
			DataFechamento: Constants.DadosLocacao.DataFechamento,
			DataVencimento: Constants.DadosLocacao.DataVencimento,
			ValorMensal: Constants.DadosLocacao.ValorMensal);
	}

	public static LocacaoResponse GerarLocacaoResponseAssinada()
	{
		return new LocacaoResponse(
			Id: Constants.DadosLocacao.Id,
			Imovel: Constants.DadosLocacao.Imovel.ToImovelResponse(),
			Locador: Constants.DadosLocacao.Locador.ToUsuarioResponse(),
			Locatario: Constants.DadosLocacao.Locatario.ToUsuarioResponse(),
			LocadorAssinou: true,
			LocatarioAssinou: true,
			DataFechamento: Constants.DadosLocacao.DataFechamentoLocacaoAssinada,
			DataVencimento: Constants.DadosLocacao.DataVencimento,
			ValorMensal: Constants.DadosLocacao.ValorMensal);
	}
}