namespace Application.Common.Interfaces.Entidades.Usuarios.DTOs;

public class CriarUsuarioRequest
{
	public string NomeCompleto { get; set; } = null!;
	public string Telefone { get; set; } = null!;
	public string Email { get; set; } = null!;
	public string Senha { get; set; } = null!;
	public string ConfirmarSenha { get; set; } = null!;
}