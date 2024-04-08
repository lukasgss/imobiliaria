using System.Collections.Generic;
using System.Linq;
using Application.Common.Extensions.Mapeamento;
using Application.Common.Interfaces.Entidades.Imoveis.DTOs;
using Domain.Entidades;
using Tests.TesteUtils.Constantes;

namespace Tests.TesteUtils.GeradorEntidades;

public static class GeradorImovel
{
	public static Imovel GerarImovel()
	{
		return new Imovel()
		{
			Id = Constants.DadosImovel.Id,
			Endereco = Constants.DadosImovel.Endereco,
			Cep = Constants.DadosImovel.Cep,
			Cidade = Constants.DadosImovel.Cidade,
			Bairro = Constants.DadosImovel.Bairro,
			Estado = Constants.DadosImovel.Estado,
			Numero = Constants.DadosImovel.Numero,
			Complemento = Constants.DadosImovel.Complemento,
			Proprietario = Constants.DadosImovel.Proprietario,
			Inquilino = Constants.DadosImovel.Inquilino,
			Corretor = Constants.DadosImovel.Corretor,
		};
	}

	private static Imovel GerarImovelAlugadoComId(int id)
	{
		return new Imovel()
		{
			Id = id,
			Endereco = Constants.DadosImovel.Endereco,
			Cep = Constants.DadosImovel.Cep,
			Cidade = Constants.DadosImovel.Cidade,
			Bairro = Constants.DadosImovel.Bairro,
			Estado = Constants.DadosImovel.Estado,
			Numero = Constants.DadosImovel.Numero,
			Complemento = Constants.DadosImovel.Complemento,
			Proprietario = Constants.DadosImovel.Proprietario,
			Inquilino = Constants.DadosImovel.Inquilino,
			Corretor = Constants.DadosImovel.Corretor,
		};
	}

	public static IEnumerable<Imovel> GerarListaImoveisAlugados()
	{
		List<Imovel> imoveis = new();
		for (int i = 0; i < 3; i++)
		{
			imoveis.Add(GerarImovelAlugadoComId(i + 1));
		}

		return imoveis;
	}

	public static IEnumerable<Imovel> GerarListaImoveisDisponiveis()
	{
		List<Imovel> imoveis = new();
		for (int i = 0; i < 3; i++)
		{
			imoveis.Add(GerarImovelAlugadoComId(i + 1));
		}

		return imoveis;
	}

	public static CriarImovelRequest GerarCriarImovelRequest()
	{
		return new CriarImovelRequest()
		{
			Endereco = Constants.DadosImovel.Endereco,
			Cep = Constants.DadosImovel.Cep,
			Cidade = Constants.DadosImovel.Cidade,
			Estado = Constants.DadosImovel.Estado,
			Bairro = Constants.DadosImovel.Bairro,
			Numero = Constants.DadosImovel.Numero,
			Complemento = Constants.DadosImovel.Complemento,
			CorretorId = Constants.DadosImovel.CorretorId
		};
	}

	public static EditarImovelRequest GerarEditarImovelRequest()
	{
		return new EditarImovelRequest()
		{
			Endereco = Constants.DadosImovel.Endereco,
			Cep = Constants.DadosImovel.Cep,
			Cidade = Constants.DadosImovel.Cidade,
			Estado = Constants.DadosImovel.Estado,
			Bairro = Constants.DadosImovel.Bairro,
			Numero = Constants.DadosImovel.Numero,
			Complemento = Constants.DadosImovel.Complemento,
			CorretorId = Constants.DadosImovel.CorretorId
		};
	}

	public static List<ImovelResponse> GerarListaImoveisResponse(IEnumerable<Imovel> imoveis)
	{
		return imoveis.Select(imovel => imovel.ToImovelResponse()).ToList();
	}

	public static ImovelResponse GerarImovelResponseComInquilino()
	{
		return new ImovelResponse(
			Id: Constants.DadosImovel.Id,
			Endereco: Constants.DadosImovel.Endereco,
			Cep: Constants.DadosImovel.Cep,
			Cidade: Constants.DadosImovel.Cidade,
			Estado: Constants.DadosImovel.Estado,
			Bairro: Constants.DadosImovel.Bairro,
			Numero: Constants.DadosImovel.Numero,
			Complemento: Constants.DadosImovel.Complemento,
			Proprietario: Constants.DadosImovel.Proprietario.ToUsuarioResponse(),
			Corretor: Constants.DadosImovel.Corretor.ToUsuarioResponse(),
			Inquilino: Constants.DadosImovel.Inquilino.ToUsuarioResponse());
	}

	public static ImovelResponse GerarImovelResponseSemInquilino()
	{
		return new ImovelResponse(
			Id: Constants.DadosImovel.Id,
			Endereco: Constants.DadosImovel.Endereco,
			Cep: Constants.DadosImovel.Cep,
			Cidade: Constants.DadosImovel.Cidade,
			Estado: Constants.DadosImovel.Estado,
			Bairro: Constants.DadosImovel.Bairro,
			Numero: Constants.DadosImovel.Numero,
			Complemento: Constants.DadosImovel.Complemento,
			Proprietario: Constants.DadosImovel.Proprietario.ToUsuarioResponse(),
			Corretor: Constants.DadosImovel.Corretor.ToUsuarioResponse(),
			Inquilino: null);
	}
}