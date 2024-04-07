namespace Application.Common.Interfaces.Entidades.Imoveis.DTOs;

public class CriarImovelRequest
{
	public string Endereco { get; init; } = null!;

	public string Cep { get; init; } = null!;

	public string Cidade { get; init; } = null!;

	public string Estado { get; init; } = null!;

	public string Bairro { get; init; } = null!;

	public int Numero { get; init; }

	public string? Complemento { get; init; }

	public int? CorretorId { get; init; }
}