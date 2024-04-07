using Application.Common.Interfaces.Entidades;
using Application.Common.Interfaces.Entidades.DTOs;
using Application.Common.Validacoes.Erros;
using Application.Common.Validacoes.ValidacoesUsuario;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/usuarios")]
public class UsuarioController : ControllerBase
{
	private readonly IUsuarioService _usuarioService;

	public UsuarioController(IUsuarioService usuarioService)
	{
		_usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
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
}