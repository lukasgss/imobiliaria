using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entidades.Imoveis;
using Application.Common.Interfaces.Entidades.Locacoes;
using Application.Common.Interfaces.Entidades.Locacoes.DTOs;
using Application.Common.Interfaces.Entidades.Usuarios;
using Application.Common.Interfaces.Providers;
using Application.Services.Entidades;
using Domain.Entidades;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.TesteUtils.Constantes;
using Tests.TesteUtils.GeradorEntidades;
using Xunit;

namespace Tests.TestesUnitarios;

public class LocacaoServiceTests
{
	private readonly ILocacaoRepository _locacaoRepositoryMock;
	private readonly IImovelRepository _imovelRepositoryMock;
	private readonly IUsuarioRepository _usuarioRepositoryMock;
	private readonly IValueProvider _valueProviderMock;
	private readonly LocacaoService _sut;

	private static readonly Locacao Locacao = GeradorLocacao.GerarLocacao();
	private static readonly Locacao LocacaoJaAssinada = GeradorLocacao.GerarLocacaoJaAssinada();
	private static readonly Imovel Imovel = GeradorImovel.GerarImovel();
	private static readonly Usuario Usuario = GeradorUsuario.GerarUsuario();
	private static readonly Usuario Locatario = GeradorUsuario.GerarUsuarioComId(Constants.DadosLocacao.LocatarioId);
	private static readonly CriarLocacaoRequest CriarLocacaoRequest = GeradorLocacao.GerarCriarLocacaoRequest();
	private static readonly EditarLocacaoRequest EditarLocacaoRequest = GeradorLocacao.GerarEditarLocacaoRequest();
	private static readonly LocacaoResponse LocacaoResponse = GeradorLocacao.GerarLocacaoResponse();

	public LocacaoServiceTests()
	{
		_locacaoRepositoryMock = Substitute.For<ILocacaoRepository>();
		_imovelRepositoryMock = Substitute.For<IImovelRepository>();
		_usuarioRepositoryMock = Substitute.For<IUsuarioRepository>();
		_valueProviderMock = Substitute.For<IValueProvider>();
		ILogger<LocacaoService> loggerMock = Substitute.For<ILogger<LocacaoService>>();

		_sut = new LocacaoService(
			_locacaoRepositoryMock,
			_imovelRepositoryMock,
			_usuarioRepositoryMock,
			_valueProviderMock, loggerMock);
	}

	[Fact]
	public async Task Obter_Locacao_Por_Id_Inexistente_Retorna_NotFoundException()
	{
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).ReturnsNull();

		async Task Result() => await _sut.ObterPorIdAsync(Locacao.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Locação com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Obter_Locacao_Por_Id_Retorna_Locacao()
	{
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).Returns(Locacao);

		LocacaoResponse locacaoRetornada = await _sut.ObterPorIdAsync(Locacao.Id);

		Assert.Equivalent(LocacaoResponse, locacaoRetornada);
	}

	[Fact]
	public async Task Cadastrar_Locacao_Sendo_Locatario_E_Locador_Retorna_BadRequestException()
	{
		CriarLocacaoRequest locacaoRequestSendoLocatarioELocador = new() { IdLocatario = Usuario.Id };

		async Task Result() =>
			await _sut.CadastrarAsync(locacaoRequestSendoLocatarioELocador, Usuario.Id);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Não é possível ser o locatário e o locador da locação.",
			exception.Message);
	}

	[Fact]
	public async Task Cadastrar_Locacao_Com_Imovel_Inexistente_Retorna_NotFoundException()
	{
		_imovelRepositoryMock.ObterPorIdAsync(Constants.DadosImovel.Id).ReturnsNull();

		async Task Result() => await _sut.CadastrarAsync(CriarLocacaoRequest, Constants.DadosLocacao.LocadorId);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Imóvel com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Cadastrar_Locacao_Com_Imovel_Que_Ja_Possui_Locacao_Retorna_ConflictException()
	{
		_imovelRepositoryMock.ObterPorIdAsync(Constants.DadosImovel.Id).Returns(Imovel);
		_locacaoRepositoryMock.ObterPorIdDoImovelAsync(Imovel.Id).Returns(Locacao);

		async Task Result() => await _sut.CadastrarAsync(CriarLocacaoRequest, Constants.DadosLocacao.LocadorId);

		var exception = await Assert.ThrowsAsync<ConflictException>(Result);
		Assert.Equal("Imóvel já possui uma locação cadastrada.", exception.Message);
	}

	[Fact]
	public async Task Cadastrar_Locacao_Sem_Ser_Dono_Do_Imovel_Retorna_UnauthorizedException()
	{
		_imovelRepositoryMock.ObterPorIdAsync(Constants.DadosImovel.Id).Returns(Imovel);
		_locacaoRepositoryMock.ObterPorIdDoImovelAsync(Imovel.Id).ReturnsNull();
		const int idUsuarioQueNaoEDono = 99;

		async Task Result() => await _sut.CadastrarAsync(CriarLocacaoRequest, idUsuarioQueNaoEDono);

		var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
		Assert.Equal("Não é possível realizar ou editar locações de imóveis em que não é proprietário.",
			exception.Message);
	}

	[Fact]
	public async Task Cadastrar_Imovel_Com_Locatario_Inexistente_Retorna_NotFoundException()
	{
		_imovelRepositoryMock.ObterPorIdAsync(Constants.DadosImovel.Id).Returns(Imovel);
		_locacaoRepositoryMock.ObterPorIdDoImovelAsync(Imovel.Id).ReturnsNull();
		_usuarioRepositoryMock.ObterPorIdAsync(CriarLocacaoRequest.IdLocatario).ReturnsNull();

		async Task Result() => await _sut.CadastrarAsync(CriarLocacaoRequest, Constants.DadosLocacao.LocadorId);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Usuário com o id especificado para locatário não existe.", exception.Message);
	}

	[Fact]
	public async Task Cadastrar_Locacao_Retorna_Locacao_Cadastrada()
	{
		_imovelRepositoryMock.ObterPorIdAsync(Constants.DadosImovel.Id).Returns(Imovel);
		_locacaoRepositoryMock.ObterPorIdDoImovelAsync(Imovel.Id).ReturnsNull();
		_usuarioRepositoryMock.ObterPorIdAsync(CriarLocacaoRequest.IdLocatario).Returns(Locatario);
		_usuarioRepositoryMock.ObterPorIdAsync(Constants.DadosLocacao.LocadorId).Returns(Usuario);

		LocacaoResponse respostaLocacao =
			await _sut.CadastrarAsync(CriarLocacaoRequest, Constants.DadosLocacao.LocadorId);

		Assert.Equivalent(LocacaoResponse, respostaLocacao);
	}

	[Fact]
	public async Task Editar_Locacao_Sendo_Locatario_E_Locador_Retorna_BadRequestException()
	{
		EditarLocacaoRequest locacaoRequestSendoLocatarioELocador = new() { IdLocatario = Usuario.Id };

		async Task Result() =>
			await _sut.EditarAsync(locacaoRequestSendoLocatarioELocador, Usuario.Id, Locacao.Id);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Não é possível ser o locatário e o locador da locação.",
			exception.Message);
	}

	[Fact]
	public async Task Editar_Locacao_Inexistente_Retorna_NotFoundException()
	{
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).ReturnsNull();

		async Task Result() => await _sut.EditarAsync(EditarLocacaoRequest, Usuario.Id, Locacao.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Locação com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Editar_Locacao_Com_Imovel_Inexistente_Retorna_NotFound_Exception()
	{
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).Returns(Locacao);
		_imovelRepositoryMock.ObterPorIdAsync(Imovel.Id).ReturnsNull();

		async Task Result() => await _sut.EditarAsync(EditarLocacaoRequest, Usuario.Id, Locacao.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Imóvel com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Editar_Locacao_Com_Imovel_Que_Ja_Possui_Locacao_Retorna_ConflictException()
	{
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).Returns(Locacao);
		_imovelRepositoryMock.ObterPorIdAsync(Imovel.Id).Returns(Imovel);
		_locacaoRepositoryMock.ObterPorIdDoImovelAsync(EditarLocacaoRequest.IdImovel)
			.Returns(new Locacao() { Id = 99 });

		async Task Result() =>
			await _sut.EditarAsync(EditarLocacaoRequest, Constants.DadosLocacao.Locador.Id, Locacao.Id);

		var exception = await Assert.ThrowsAsync<ConflictException>(Result);
		Assert.Equal("Imóvel já possui uma locação cadastrada.", exception.Message);
	}

	[Fact]
	public async Task Editar_Locacao_Em_Que_Nao_E_Locador_Retorna_UnauthorizedException()
	{
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).Returns(Locacao);
		_imovelRepositoryMock.ObterPorIdAsync(Imovel.Id).Returns(Imovel);
		_locacaoRepositoryMock.ObterPorIdDoImovelAsync(EditarLocacaoRequest.IdImovel).ReturnsNull();
		const int idUsuarioQueNaoELocador = 99;

		async Task Result() =>
			await _sut.EditarAsync(EditarLocacaoRequest, idUsuarioQueNaoELocador, Locacao.Id);

		var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
		Assert.Equal("Não é possível editar locações em que não é o locador.", exception.Message);
	}

	[Fact]
	public async Task Editar_Locacao_Em_Que_Nao_E_Dono_Retorna_UnauthorizedException()
	{
		const int idUsuarioQueNaoEDono = 99;
		Locacao locacao = new() { Locador = new() { Id = idUsuarioQueNaoEDono } };
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).Returns(locacao);
		_imovelRepositoryMock.ObterPorIdAsync(Imovel.Id).Returns(Imovel);
		_locacaoRepositoryMock.ObterPorIdDoImovelAsync(EditarLocacaoRequest.IdImovel).ReturnsNull();

		async Task Result() =>
			await _sut.EditarAsync(EditarLocacaoRequest, idUsuarioQueNaoEDono, Locacao.Id);

		var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
		Assert.Equal("Não é possível realizar ou editar locações de imóveis em que não é proprietário.",
			exception.Message);
	}

	[Fact]
	public async Task Editar_Locacao_Com_Locatario_Inexistente_Retorna_NotFoundException()
	{
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).Returns(Locacao);
		_imovelRepositoryMock.ObterPorIdAsync(Imovel.Id).Returns(Imovel);
		_locacaoRepositoryMock.ObterPorIdDoImovelAsync(EditarLocacaoRequest.IdImovel).ReturnsNull();
		_usuarioRepositoryMock.ObterPorIdAsync(Locatario.Id).ReturnsNull();

		async Task Result() => await _sut.EditarAsync(EditarLocacaoRequest, Usuario.Id, Locacao.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Usuário com o id especificado para locatário não existe.", exception.Message);
	}

	[Fact]
	public async Task Editar_Locacao_Retorna_Locacao_Editada()
	{
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).Returns(Locacao);
		_imovelRepositoryMock.ObterPorIdAsync(Imovel.Id).Returns(Imovel);
		_locacaoRepositoryMock.ObterPorIdDoImovelAsync(EditarLocacaoRequest.IdImovel).ReturnsNull();
		_usuarioRepositoryMock.ObterPorIdAsync(Locatario.Id).Returns(Locatario);
		_locacaoRepositoryMock.EditarLocacao(Locacao, Imovel, Locatario, EditarLocacaoRequest)
			.Returns(Locacao);

		LocacaoResponse respostaLocacao =
			await _sut.EditarAsync(EditarLocacaoRequest, Usuario.Id, Locacao.Id);

		Assert.Equivalent(LocacaoResponse, respostaLocacao);
	}

	[Fact]
	public async Task Excluir_Locacao_Inexistente_Retorna_NotFoundException()
	{
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).ReturnsNull();

		async Task Result() => await _sut.DeletarAsync(Locacao.Id, Usuario.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Locação com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Excluir_Locacao_Em_Que_Nao_E_Dono_Retorna_UnauthorizedException()
	{
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).Returns(Locacao);
		const int idUsuarioQueNaoELocador = 99;

		async Task Result() => await _sut.DeletarAsync(Locacao.Id, idUsuarioQueNaoELocador);

		var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
		Assert.Equal("Não é possível excluir locações em que não é o locador.", exception.Message);
	}

	[Fact]
	public async Task Assinar_Locacao_Inexistente_Retorna_NotFoundException()
	{
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).ReturnsNull();

		async Task Result() => await _sut.AssinarAsync(Locacao.Id, Usuario.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Locação com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Assinar_Locacao_Sendo_Locador_Ja_Tendo_Assinado_Retorna_BadRequestException()
	{
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).Returns(LocacaoJaAssinada);

		async Task Result() => await _sut.AssinarAsync(LocacaoJaAssinada.Id, LocacaoJaAssinada.Locador.Id);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Locação já foi assinada por você, não é possível assiná-la novamente.", exception.Message);
	}

	[Fact]
	public async Task Assinar_Locacao_Sendo_Locatario_Ja_Tendo_Assinado_Retorna_BadRequestException()
	{
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).Returns(LocacaoJaAssinada);

		async Task Result() => await _sut.AssinarAsync(LocacaoJaAssinada.Id, LocacaoJaAssinada.Locatario.Id);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Locação já foi assinada por você, não é possível assiná-la novamente.", exception.Message);
	}

	[Fact]
	public async Task Assinar_Locacao_Em_Que_Nao_E_Locador_Ou_Locatario_Retorna_UnauthorizedException()
	{
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).Returns(Locacao);
		const int idUsuarioNaoLocatarioOuLocador = 99;

		async Task Result() => await _sut.AssinarAsync(LocacaoJaAssinada.Id, idUsuarioNaoLocatarioOuLocador);

		var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
		Assert.Equal("Não é possível assinar locações em que não é locador ou locatário.", exception.Message);
	}

	[Fact]
	public async Task Assinar_Locacao_Retorna_Locacao_Assinada()
	{
		Locacao locacaoJaAssinadaPeloLocador = GeradorLocacao.GerarLocacaoJaAssinadaPeloLocador();
		_locacaoRepositoryMock.ObterPorIdAsync(Locacao.Id).Returns(locacaoJaAssinadaPeloLocador);
		_valueProviderMock.UtcNow().Returns(Constants.DadosLocacao.DataFechamentoLocacaoAssinada);
		_imovelRepositoryMock.ObterPorIdAsync(Locacao.ImovelId).Returns(Imovel);
		_usuarioRepositoryMock.ObterPorIdAsync(Locacao.Locatario.Id).Returns(Locatario);
		LocacaoResponse respostaLocacaoEsperada = GeradorLocacao.GerarLocacaoResponseAssinada();

		LocacaoResponse respostaLocacao = await _sut.AssinarAsync(Locacao.Id, Locatario.Id);

		Assert.Equivalent(respostaLocacaoEsperada, respostaLocacao);
	}
}