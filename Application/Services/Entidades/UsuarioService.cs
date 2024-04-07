using Application.Common.Exceptions;
using Application.Common.Extensions.Mapeamento;
using Application.Common.Interfaces.Autenticacao;
using Application.Common.Interfaces.Entidades.Usuarios;
using Application.Common.Interfaces.Entidades.Usuarios.DTOs;
using Domain.Entidades;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Entidades;

public class UsuarioService : IUsuarioService
{
	private readonly IUsuarioRepository _usuarioRepository;
	private readonly IGeradorTokenJwt _geradorTokenJwt;

	public UsuarioService(IUsuarioRepository usuarioRepository, IGeradorTokenJwt geradorTokenJwt)
	{
		_usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
		_geradorTokenJwt = geradorTokenJwt ?? throw new ArgumentNullException(nameof(geradorTokenJwt));
	}

	public async Task<UsuarioResponse> ObterUsuarioPorIdAsync(int idUsuario)
	{
		Usuario? usuario = await _usuarioRepository.ObterPorIdAsync(idUsuario);
		if (usuario is null)
		{
			throw new NotFoundException("Usuário com o id especificado não foi encontrado.");
		}

		return usuario.ToUsuarioResponse();
	}

	public async Task<UsuarioLoginResponse> CadastrarAsync(CriarUsuarioRequest request)
	{
		Usuario usuarioParaCriar = new()
		{
			NomeCompleto = request.NomeCompleto,
			UserName = request.Email,
			PhoneNumber = request.Telefone,
			Email = request.Email,
			EmailConfirmed = true,
		};

		Usuario? usuarioJaExiste = await _usuarioRepository.ObterPorEmailAsync(request.Email);
		if (usuarioJaExiste is not null)
		{
			throw new ConflictException("Usuário com o e-mail especificado já existe.");
		}

		IdentityResult resultadoCadastro =
			await _usuarioRepository.RegistrarAsync(usuarioParaCriar, request.Senha);

		IdentityResult resultadoLockout =
			await _usuarioRepository.SetLockoutEnabledAsync(usuarioParaCriar, habilitado: false);

		if (!resultadoCadastro.Succeeded || !resultadoLockout.Succeeded)
		{
			throw new InternalServerErrorException();
		}

		string jwtToken = _geradorTokenJwt.GerarToken(usuarioParaCriar.Id, usuarioParaCriar.NomeCompleto);

		return usuarioParaCriar.ToUsuarioLoginResponse(jwtToken);
	}

	public async Task<UsuarioLoginResponse> LoginAsync(LoginRequest request)
	{
		Usuario? usuarioParaLogar = await _usuarioRepository.ObterPorEmailAsync(request.Email);
		if (usuarioParaLogar is null)
		{
			usuarioParaLogar = new Usuario()
			{
				SecurityStamp = Guid.NewGuid().ToString()
			};
		}

		SignInResult signInResult =
			await _usuarioRepository.ChecarCredenciais(usuarioParaLogar, request.Senha);

		if (!signInResult.Succeeded || usuarioParaLogar is null)
		{
			if (signInResult.IsLockedOut)
			{
				throw new LockedException("Essa conta está bloqueada, aguarde e tente novamente.");
			}

			throw new UnauthorizedException("Credenciais inválidas.");
		}

		string jwtToken = _geradorTokenJwt.GerarToken(usuarioParaLogar.Id, usuarioParaLogar.NomeCompleto);

		return usuarioParaLogar.ToUsuarioLoginResponse(jwtToken);
	}

	public async Task<UsuarioResponse> EditarAsync(int idUsuarioLogado, int idUsuarioRota, EditarUsuarioRequest request)
	{
		if (idUsuarioLogado != idUsuarioRota)
		{
			throw new ForbiddenException("Não é possível editar dados de outros usuários.");
		}

		Usuario? usuario = await _usuarioRepository.ObterPorIdAsync(idUsuarioLogado);
		if (usuario is null)
		{
			throw new NotFoundException("Usuário com o id especificado não existe.");
		}

		usuario.NomeCompleto = request.NomeCompleto;
		usuario.PhoneNumber = request.Telefone;
		usuario.Email = request.Email;

		await _usuarioRepository.CommitAsync();

		return usuario.ToUsuarioResponse();
	}

	public async Task DeletarAsync(int idUsuarioLogado, int idUsuarioADeletar)
	{
		if (idUsuarioLogado != idUsuarioADeletar)
		{
			throw new ForbiddenException("Não é possível excluir outros usuários.");
		}

		Usuario? usuario = await _usuarioRepository.ObterPorIdAsync(idUsuarioADeletar);
		if (usuario is null)
		{
			throw new NotFoundException("Usuário com o id especificado não existe.");
		}

		_usuarioRepository.Delete(usuario);
		await _usuarioRepository.CommitAsync();
	}
}