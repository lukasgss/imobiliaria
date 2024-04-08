using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entidades.Imoveis;
using Application.Common.Interfaces.Entidades.Imoveis.DTOs;
using Application.Common.Interfaces.Entidades.Usuarios;
using Application.Services.Entidades;
using Domain.Entidades;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.TesteUtils.GeradorEntidades;
using Xunit;

namespace Tests.TestesUnitarios;

public class ImovelServiceTests
{
	private readonly IUsuarioRepository _usuarioRepositoryMock;
	private readonly IImovelRepository _imovelRepositoryMock;
	private readonly ImovelService _sut;

	private static readonly Imovel Imovel = GeradorImovel.GerarImovel();
	private static readonly IEnumerable<Imovel> ListaImoveisAlugados = GeradorImovel.GerarListaImoveisAlugados();
	private static readonly IEnumerable<Imovel> ListaImoveisDisponiveis = GeradorImovel.GerarListaImoveisDisponiveis();
	private static readonly ImovelResponse ImovelResponseComInquilino = GeradorImovel.GerarImovelResponseComInquilino();
	private static readonly ImovelResponse ImovelResponseSemInquilino = GeradorImovel.GerarImovelResponseSemInquilino();
	private static readonly Usuario Usuario = GeradorUsuario.GerarUsuario();
	private static readonly CriarImovelRequest CriarImovelRequest = GeradorImovel.GerarCriarImovelRequest();
	private static readonly EditarImovelRequest EditarImovelRequest = GeradorImovel.GerarEditarImovelRequest();

	private static readonly List<ImovelResponse> ListaImoveisAlugadosResponse =
		GeradorImovel.GerarListaImoveisResponse(ListaImoveisAlugados);

	private static readonly List<ImovelResponse> ListaImoveisDisponiveisResponse =
		GeradorImovel.GerarListaImoveisResponse(ListaImoveisDisponiveis);

	public ImovelServiceTests()
	{
		_usuarioRepositoryMock = Substitute.For<IUsuarioRepository>();
		_imovelRepositoryMock = Substitute.For<IImovelRepository>();
		ILogger<ImovelService> loggerMock = Substitute.For<ILogger<ImovelService>>();

		_sut = new ImovelService(_usuarioRepositoryMock, _imovelRepositoryMock, loggerMock);
	}

	[Fact]
	public async Task Obter_Imovel_Por_Id_Nao_Existente_Retorna_NotFoundException()
	{
		_imovelRepositoryMock.ObterPorIdAsync(Imovel.Id).ReturnsNull();

		async Task Result() => await _sut.ObterPorIdAsync(Imovel.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Imóvel com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Obter_Imovel_Por_Id_Retorna_Imovel()
	{
		_imovelRepositoryMock.ObterPorIdAsync(Imovel.Id).Returns(Imovel);

		ImovelResponse imovelObtido = await _sut.ObterPorIdAsync(Imovel.Id);

		Assert.Equivalent(ImovelResponseComInquilino, imovelObtido);
	}

	[Fact]
	public async Task Obter_Imoveis_Alugados_Retorna_Imoveis_Alugados()
	{
		_imovelRepositoryMock.ObterImoveisAlugados().Returns(ListaImoveisAlugados);

		var imoveisAlugadosRetornados = await _sut.ObterImoveisAlugados();

		Assert.Equivalent(ListaImoveisAlugadosResponse, imoveisAlugadosRetornados);
	}

	[Fact]
	public async Task Obter_Imoveis_Disponiveis_Retorna_Imoveis_Disponiveis()
	{
		_imovelRepositoryMock.ObterDisponiveisParaAluguel().Returns(ListaImoveisDisponiveis);

		var imoveisDisponiveisRetornados = await _sut.ObterImoveisDisponiveis();

		Assert.Equivalent(ListaImoveisDisponiveisResponse, imoveisDisponiveisRetornados);
	}

	[Fact]
	public async Task Cadastrar_Imovel_Como_Usuario_Nao_Existente_Retorna_NotFoundException()
	{
		_usuarioRepositoryMock.ObterPorIdAsync(Usuario.Id).ReturnsNull();

		async Task Result() => await _sut.CadastrarAsync(CriarImovelRequest, Usuario.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Usuário com o id especificado para proprietário não existe.", exception.Message);
	}

	[Fact]
	public async Task Cadastrar_Imovel_Com_Corretor_Inexistente_Retorna_NotFoundException()
	{
		_usuarioRepositoryMock.ObterPorIdAsync(Usuario.Id).Returns(Usuario);
		_usuarioRepositoryMock.ObterPorIdAsync(CriarImovelRequest.CorretorId!.Value).ReturnsNull();

		async Task Result() => await _sut.CadastrarAsync(CriarImovelRequest, Usuario.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Usuário com o id especificado para corretor não existe.", exception.Message);
	}

	[Fact]
	public async Task Cadastrar_Imovel_Retorna_Imovel_Cadastrado()
	{
		_usuarioRepositoryMock.ObterPorIdAsync(Usuario.Id).Returns(Usuario);
		_usuarioRepositoryMock.ObterPorIdAsync(CriarImovelRequest.CorretorId!.Value).Returns(Imovel.Corretor);

		ImovelResponse imovelCadastrado = await _sut.CadastrarAsync(CriarImovelRequest, Usuario.Id);

		Assert.Equivalent(ImovelResponseSemInquilino, imovelCadastrado);
	}

	[Fact]
	public async Task Editar_Imovel_Nao_Existente_Retorna_NotFoundException()
	{
		_imovelRepositoryMock.ObterPorIdAsync(Imovel.Id).ReturnsNull();

		async Task Result() => await _sut.EditarAsync(EditarImovelRequest, Usuario.Id, Imovel.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Imóvel com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Editar_Imovel_No_Qual_Nao_E_Proprietario_Retorna_ForbiddenException()
	{
		_imovelRepositoryMock.ObterPorIdAsync(Imovel.Id).Returns(Imovel);
		const int idUsuarioDiferente = 99;

		async Task Result() => await _sut.EditarAsync(EditarImovelRequest, idUsuarioDiferente, Imovel.Id);

		var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
		Assert.Equal("Não é possível editar imóveis no qual não é proprietário.", exception.Message);
	}

	[Fact]
	public async Task Editar_Imovel_Com_Corretor_Nao_Existente_Retorna_NotFoundException()
	{
		_imovelRepositoryMock.ObterPorIdAsync(Imovel.Id).Returns(Imovel);
		_usuarioRepositoryMock.ObterPorIdAsync(Imovel.Corretor!.Id).ReturnsNull();

		async Task Result() => await _sut.EditarAsync(EditarImovelRequest, Usuario.Id, Imovel.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Usuário com o id especificado para corretor não existe.", exception.Message);
	}

	[Fact]
	public async Task Editar_Imovel_Retorna_Imovel_Editado()
	{
		_imovelRepositoryMock.ObterPorIdAsync(Imovel.Id).Returns(Imovel);
		_usuarioRepositoryMock.ObterPorIdAsync(Imovel.Corretor!.Id).Returns(Imovel.Corretor);

		ImovelResponse imovelEditado = await _sut.EditarAsync(EditarImovelRequest, Usuario.Id, Imovel.Id);

		Assert.Equivalent(ImovelResponseComInquilino, imovelEditado);
	}

	[Fact]
	public async Task Excluir_Imovel_Nao_Existente_Retorna_NotFoundException()
	{
		_imovelRepositoryMock.ObterPorIdAsync(Imovel.Id).ReturnsNull();

		async Task Result() => await _sut.DeletarAsync(Imovel.Id, Usuario.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Imóvel com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Excluir_Imovel_No_Qual_Nao_E_Proprietario_Retorna_ForbiddenException()
	{
		_imovelRepositoryMock.ObterPorIdAsync(Imovel.Id).Returns(Imovel);
		const int idUsuarioDiferente = 99;

		async Task Result() => await _sut.DeletarAsync(Imovel.Id, idUsuarioDiferente);

		var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
		Assert.Equal("Não é possível excluir imóveis no qual não é proprietário.", exception.Message);
	}
}