using Application.Common.Interfaces.Entidades.Usuarios.DTOs;
using Domain.Entidades;

namespace Application.Common.Extensions.Mapeamento;

public static class MapeamentoUsuarios
{
	public static UsuarioResponse ToUsuarioResponse(this Usuario usuario)
	{
		return new UsuarioResponse(Email: usuario.Email!,
			NomeCompleto: usuario.NomeCompleto,
			NumeroTelefone: usuario.PhoneNumber!);
	}

	public static UsuarioLoginResponse ToUsuarioLoginResponse(this Usuario usuario, string jwtToken)
	{
		return new UsuarioLoginResponse(
			Id: usuario.Id,
			Email: usuario.Email!,
			NomeCompleto: usuario.NomeCompleto,
			NumeroTelefone: usuario.PhoneNumber!,
			Token: jwtToken);
	}
}