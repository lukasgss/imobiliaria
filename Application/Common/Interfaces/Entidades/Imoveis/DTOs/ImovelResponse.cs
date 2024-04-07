using Application.Common.Interfaces.Entidades.Usuarios.DTOs;

namespace Application.Common.Interfaces.Entidades.Imoveis.DTOs;

public record ImovelResponse(int Id,
	string Endereco,
	string Cep,
	string Cidade,
	string Estado,
	string Bairro,
	int Numero,
	string? Complemento,
	UsuarioResponse Proprietario,
	UsuarioResponse? Corretor,
	UsuarioResponse? Inquilino);