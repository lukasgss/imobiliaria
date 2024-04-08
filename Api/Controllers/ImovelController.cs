using Application.Common.Interfaces.Autorizacao;
using Application.Common.Interfaces.Entidades.Imoveis;
using Application.Common.Interfaces.Entidades.Imoveis.DTOs;
using Application.Common.Validacoes.Erros;
using Application.Common.Validacoes.ValidacoesImovel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Api.Controllers;

[ApiController]
[Route("/api/imoveis")]
public class ImovelController : ControllerBase
{
	private readonly IImovelService _imovelService;
	private readonly IAutorizacaoUsuarioService _autorizacaoUsuarioService;

	public ImovelController(IImovelService imovelService, IAutorizacaoUsuarioService autorizacaoUsuarioService)
	{
		_imovelService = imovelService ?? throw new ArgumentNullException(nameof(imovelService));
		_autorizacaoUsuarioService = autorizacaoUsuarioService ??
		                             throw new ArgumentNullException(nameof(autorizacaoUsuarioService));
	}

	[HttpGet("{imovelId:int}", Name = "ObterImovelPorId")]
	public async Task<ActionResult<ImovelResponse>> ObterImovelPorId(int imovelId)
	{
		return await _imovelService.ObterPorIdAsync(imovelId);
	}

	[HttpGet("aluguel/alugados")]
	public async Task<ActionResult<IEnumerable<ImovelResponse>>> ObterImoveisAlugados()
	{
		var imoveisAlugados = await _imovelService.ObterImoveisAlugados();
		return Ok(imoveisAlugados);
	}

	[HttpGet("aluguel/disponiveis")]
	public async Task<ActionResult<IEnumerable<ImovelResponse>>> ObterImoveisDisponiveis()
	{
		var imoveisDisponiveis = await _imovelService.ObterImoveisDisponiveis();
		return Ok(imoveisDisponiveis);
	}

	[Authorize]
	[HttpPost]
	public async Task<ActionResult<ImovelResponse>> Cadastrar(CriarImovelRequest criarImovelRequest)
	{
		ValidacaoCadastroImovel validador = new();
		ValidationResult validationResult = validador.Validate(criarImovelRequest);
		if (!validationResult.IsValid)
		{
			var erros = validationResult.Errors
				.Select(e => new ErroValidacao(e.PropertyName, e.ErrorMessage));
			return BadRequest(erros);
		}

		int idUsuarioLogado = _autorizacaoUsuarioService.ObterIdUsuarioPorTokenJwt(User);
		ImovelResponse imovelCadastrado = await _imovelService.CadastrarAsync(criarImovelRequest, idUsuarioLogado);

		return new CreatedAtRouteResult(nameof(ObterImovelPorId),
			new { imovelId = imovelCadastrado.Id },
			imovelCadastrado);
	}

	[Authorize]
	[HttpPut("{idImovel:int}")]
	public async Task<ActionResult<ImovelResponse>> Editar(EditarImovelRequest editarImovelRequest, int idImovel)
	{
		ValidacaoEdicaoImovel validador = new();
		ValidationResult validationResult = validador.Validate(editarImovelRequest);
		if (!validationResult.IsValid)
		{
			var erros = validationResult.Errors
				.Select(e => new ErroValidacao(e.PropertyName, e.ErrorMessage));
			return BadRequest(erros);
		}

		int idUsuarioLogado = _autorizacaoUsuarioService.ObterIdUsuarioPorTokenJwt(User);

		return await _imovelService.EditarAsync(editarImovelRequest, idUsuarioLogado, idImovel);
	}

	[Authorize]
	[HttpDelete("{idImovel:int}")]
	public async Task<ActionResult> Deletar(int idImovel)
	{
		int idUsuarioLogado = _autorizacaoUsuarioService.ObterIdUsuarioPorTokenJwt(User);

		await _imovelService.DeletarAsync(idImovel, idUsuarioLogado);
		return NoContent();
	}
}