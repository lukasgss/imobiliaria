using System;
using Domain.Entidades;
using Tests.TesteUtils.GeradorEntidades;

namespace Tests.TesteUtils.Constantes;

public static partial class Constants
{
	public static class DadosLocacao
	{
		public const int Id = 0;
		public static readonly Imovel Imovel = GeradorImovel.GerarImovel();
		public static readonly int ImovelId = Imovel.Id;
		public static readonly Usuario Locador = GeradorUsuario.GerarUsuario();
		public static readonly int LocadorId = Locador.Id;
		public static readonly Usuario Locatario = GeradorUsuario.GerarUsuarioComId(3);
		public static readonly int LocatarioId = Locatario.Id;
		public const bool LocadorAssinou = false;
		public const bool LocatarioAssinou = false;
		public static readonly DateTime? DataFechamento = null;
		public static readonly DateTime DataFechamentoLocacaoAssinada = new(2020, 1, 1);
		public static readonly DateOnly DataVencimento = new(2022, 1, 1);
		public const decimal ValorMensal = 1000;
	}
}