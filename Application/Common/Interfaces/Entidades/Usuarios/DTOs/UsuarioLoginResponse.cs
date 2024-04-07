namespace Application.Common.Interfaces.Entidades.Usuarios.DTOs;

public record UsuarioLoginResponse(int Id, string Email, string NomeCompleto, string NumeroTelefone, string Token);