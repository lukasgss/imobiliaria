using Application.Common.Interfaces.Entidades.Imoveis.DTOs;
using Domain.Entidades;

namespace Application.Common.Extensions.Mapeamento;

public static class MapeamentoImoveis
{
	public static ImovelResponse ToImovelResponse(this Imovel imovel)
	{
		return new ImovelResponse(
			Id: imovel.Id,
			Endereco: imovel.Endereco,
			Cep: imovel.Cep,
			Cidade: imovel.Cidade,
			Estado: imovel.Estado,
			Bairro: imovel.Bairro,
			Numero: imovel.Numero,
			Complemento: imovel.Complemento,
			Proprietario: imovel.Proprietario.ToUsuarioResponse(),
			Corretor: imovel.Corretor?.ToUsuarioResponse(),
			Inquilino: imovel.Inquilino?.ToUsuarioResponse());
	}

	public static IEnumerable<ImovelResponse> ToImovelResponseList(this IEnumerable<Imovel> imoveis)
	{
		return imoveis.Select(imovel => imovel.ToImovelResponse()).ToList();
	}
}