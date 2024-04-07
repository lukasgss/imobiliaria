namespace Application.Common.Validacoes.Erros;

public class ErroValidacao
{
	public string Campo { get; init; }
	public string Mensagem { get; init; }

	public ErroValidacao(string campo, string mensagem)
	{
		Campo = campo ?? throw new ArgumentNullException(nameof(campo));
		Mensagem = mensagem ?? throw new ArgumentNullException(nameof(mensagem));
	}
}