using Application.Common.Interfaces.Autorizacao;
using Application.Common.Interfaces.Entidades;
using Application.Common.Interfaces.Entidades.DTOs;
using Application.Common.Validacoes.Erros;
using Application.Common.Validacoes.ValidacoesUsuario;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/usuarios")]
public class UsuarioController : ControllerBase
{
	private readonly IUsuarioService _usuarioService;
	private readonly IAutorizacaoUsuarioService _autorizacaoUsuarioService;

	public UsuarioController(IUsuarioService usuarioService, IAutorizacaoUsuarioService autorizacaoUsuarioService)
	{
		_usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
		_autorizacaoUsuarioService = autorizacaoUsuarioService ??
		                             throw new ArgumentNullException(nameof(autorizacaoUsuarioService));
	}

	[HttpGet("{idUsuario:int}", Name = "ObterUsuarioPorId")]
	public async Task<ActionResult<UsuarioResponse>> ObterUsuarioPorId(int idUsuario)
	{
		return await _usuarioService.ObterUsuarioPorIdAsync(idUsuario);
	}

	[HttpPost("cadastro")]
	public async Task<ActionResult<UsuarioLoginResponse>> Cadastrar(CriarUsuarioRequest criarUsuarioRequest)
	{
		ValidacaoCadastroUsuario validador = new();
		ValidationResult validationResult = validador.Validate(criarUsuarioRequest);
		if (!validationResult.IsValid)
		{
			var erros = validationResult.Errors
				.Select(e => new ErroValidacao(e.PropertyName, e.ErrorMessage));
			return BadRequest(erros);
		}

		UsuarioLoginResponse usuarioCadastrado = await _usuarioService.CadastrarAsync(criarUsuarioRequest);
		return new CreatedAtRouteResult(
			nameof(ObterUsuarioPorId),
			new { idUsuario = usuarioCadastrado.Id },
			usuarioCadastrado);
	}

	[HttpPost("login")]
	public async Task<ActionResult<UsuarioLoginResponse>> Login(LoginRequest loginRequest)
	{
		ValidacaoLoginUsuario validador = new();
		ValidationResult validationResult = validador.Validate(loginRequest);
		if (!validationResult.IsValid)
		{
			var erros = validationResult.Errors
				.Select(e => new ErroValidacao(e.PropertyName, e.ErrorMessage));
			return BadRequest(erros);
		}

		UsuarioLoginResponse usuarioLogado = await _usuarioService.LoginAsync(loginRequest);

		return Ok(usuarioLogado);
	}

	[Authorize]
	[HttpPut("{idUsuario:int}")]
	public async Task<ActionResult<UsuarioResponse>> Editar(EditarUsuarioRequest editarUsuarioRequest, int idUsuario)
	{
		ValidacaoEdicaoUsuario validador = new();
		ValidationResult validationResult = validador.Validate(editarUsuarioRequest);
		if (!validationResult.IsValid)
		{
			var erros = validationResult.Errors
				.Select(e => new ErroValidacao(e.PropertyName, e.ErrorMessage));
			return BadRequest(erros);
		}

		int idUsuarioLogado = _autorizacaoUsuarioService.ObterIdUsuarioPorTokenJwt(User);

		return await _usuarioService.EditarAsync(idUsuarioLogado, idUsuario, editarUsuarioRequest);
	}

	[Authorize]
	[HttpDelete("{idUsuario:int}")]
	public async Task<ActionResult> Excluir(int idUsuario)
	{
		int idUsuarioLogado = _autorizacaoUsuarioService.ObterIdUsuarioPorTokenJwt(User);

		await _usuarioService.DeletarAsync(idUsuarioLogado, idUsuario);
		return NoContent();
	}
}