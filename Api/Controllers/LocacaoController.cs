using Application.Common.Interfaces.Autorizacao;
using Application.Common.Interfaces.Entidades.Locacoes;
using Application.Common.Interfaces.Entidades.Locacoes.DTOs;
using Application.Common.Validacoes.Erros;
using Application.Common.Validacoes.ValidacoesLocacao;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/locacoes")]
public class LocacaoController : ControllerBase
{
	private readonly ILocacaoService _locacaoService;
	private readonly IAutorizacaoUsuarioService _autorizacaoUsuarioService;

	public LocacaoController(ILocacaoService locacaoService, IAutorizacaoUsuarioService autorizacaoUsuarioService)
	{
		_locacaoService = locacaoService ?? throw new ArgumentNullException(nameof(locacaoService));
		_autorizacaoUsuarioService = autorizacaoUsuarioService ??
		                             throw new ArgumentNullException(nameof(autorizacaoUsuarioService));
	}

	[HttpGet("{idLocacao:int}", Name = "ObterLocacaoPorId")]
	public async Task<ActionResult<LocacaoResponse>> ObterLocacaoPorId(int idLocacao)
	{
		return await _locacaoService.ObterPorIdAsync(idLocacao);
	}

	[Authorize]
	[HttpPost]
	public async Task<ActionResult<LocacaoResponse>> Cadastrar(CriarLocacaoRequest criarLocacaoRequest)
	{
		ValidacaoCadastroLocacao validador = new();
		ValidationResult validationResult = validador.Validate(criarLocacaoRequest);
		if (!validationResult.IsValid)
		{
			var erros = validationResult.Errors.Select(e => new ErroValidacao(e.PropertyName, e.ErrorMessage));
			return BadRequest(erros);
		}

		int idUsuario = _autorizacaoUsuarioService.ObterIdUsuarioPorTokenJwt(User);

		LocacaoResponse locacaoCadastrada = await _locacaoService.CadastrarAsync(criarLocacaoRequest, idUsuario);
		return new CreatedAtRouteResult(
			nameof(ObterLocacaoPorId),
			new { idLocacao = locacaoCadastrada.Id },
			locacaoCadastrada);
	}

	[Authorize]
	[HttpPost("assinar/{idLocacao:int}")]
	public async Task<ActionResult<LocacaoResponse>> Assinar(int idLocacao)
	{
		int idUsuario = _autorizacaoUsuarioService.ObterIdUsuarioPorTokenJwt(User);

		LocacaoResponse locacaoAssinada = await _locacaoService.AssinarAsync(idLocacao, idUsuario);
		return Ok(locacaoAssinada);
	}

	[Authorize]
	[HttpPut("{idLocacao:int}")]
	public async Task<ActionResult<LocacaoResponse>> Editar(EditarLocacaoRequest editarLocacaoRequest, int idLocacao)
	{
		ValidacaoEditarLocacao validador = new();
		ValidationResult validationResult = validador.Validate(editarLocacaoRequest);
		if (!validationResult.IsValid)
		{
			var erros = validationResult.Errors.Select(e => new ErroValidacao(e.PropertyName, e.ErrorMessage));
			return BadRequest(erros);
		}

		int idUsuario = _autorizacaoUsuarioService.ObterIdUsuarioPorTokenJwt(User);

		return await _locacaoService.EditarAsync(editarLocacaoRequest, idUsuario, idLocacao);
	}

	[Authorize]
	[HttpDelete("{idLocacao:int}")]
	public async Task<ActionResult> Excluir(int idLocacao)
	{
		int idUsuario = _autorizacaoUsuarioService.ObterIdUsuarioPorTokenJwt(User);

		await _locacaoService.DeletarAsync(idLocacao, idUsuario);
		return NoContent();
	}
}