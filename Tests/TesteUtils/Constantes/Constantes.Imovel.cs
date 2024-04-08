using Domain.Entidades;
using Tests.TesteUtils.GeradorEntidades;

namespace Tests.TesteUtils.Constantes;

public static partial class Constants
{
	public static class DadosImovel
	{
		public const int Id = 0;
		public const string Endereco = "Endereço";
		public const string Cep = "31180330";
		public const string Cidade = "Cidade";
		public const string Bairro = "Bairro";
		public const string Estado = "Estado";
		public const int Numero = 100;
		public const string Complemento = "Complemento";
		public static readonly Usuario Proprietario = GeradorUsuario.GerarUsuario();
		public static readonly int ProprietarioId = Proprietario.Id;
		public static readonly Usuario Corretor = GeradorUsuario.GerarUsuarioComId(2);
		public static readonly int CorretorId = Corretor.Id;
		public static readonly Usuario Inquilino = GeradorUsuario.GerarUsuarioComId(3);
		public static readonly int InquilinoId = Inquilino.Id;
	}
}