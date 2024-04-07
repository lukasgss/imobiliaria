namespace Application.Common.Interfaces.Entidades.DTOs;

public class EditarUsuarioRequest
{
	public string NomeCompleto { get; set; } = null!;
	public string Telefone { get; set; } = null!;
	public string Email { get; set; } = null!;
}