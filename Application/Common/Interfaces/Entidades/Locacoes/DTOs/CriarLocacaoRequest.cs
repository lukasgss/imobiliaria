namespace Application.Common.Interfaces.Entidades.Locacoes.DTOs;

public class CriarLocacaoRequest
{
	public int IdImovel { get; init; }
	public int IdLocatario { get; init; }
	public DateOnly DataVencimento { get; init; }
	public decimal ValorMensal { get; init; }
}