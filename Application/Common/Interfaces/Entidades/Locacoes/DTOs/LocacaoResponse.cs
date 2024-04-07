using Application.Common.Interfaces.Entidades.Imoveis.DTOs;
using Application.Common.Interfaces.Entidades.Usuarios.DTOs;

namespace Application.Common.Interfaces.Entidades.Locacoes.DTOs;

public record LocacaoResponse(int Id,
	ImovelResponse Imovel,
	UsuarioResponse Locador,
	UsuarioResponse Locatario,
	bool LocadorAssinou,
	bool LocatarioAssinou,
	DateOnly DataVencimento,
	DateTime? DataFechamento,
	decimal ValorMensal);