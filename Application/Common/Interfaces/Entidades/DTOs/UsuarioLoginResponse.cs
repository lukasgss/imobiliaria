namespace Application.Common.Interfaces.Entidades.DTOs;

public record UsuarioLoginResponse(int Id, string Email, string NomeCompleto, string NumeroTelefone, string Token);