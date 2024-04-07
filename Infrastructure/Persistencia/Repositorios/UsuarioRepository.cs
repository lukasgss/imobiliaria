using Application.Common.Interfaces.Entidades;
using Domain.Entidades;
using Infrastructure.Persistencia.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistencia.Repositorios;

public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{
	private readonly AppDbContext _dbContext;
	private readonly UserManager<Usuario> _userManager;
	private readonly SignInManager<Usuario> _signInManager;

	public UsuarioRepository(
		AppDbContext dbContext,
		UserManager<Usuario> userManager,
		SignInManager<Usuario> signInManager) : base(dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
		_signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
	}

	public async Task<IdentityResult> RegistrarAsync(Usuario usuario, string senha)
	{
		return await _userManager.CreateAsync(usuario, senha);
	}

	public async Task<SignInResult> ChecarCredenciais(Usuario usuario, string senha)
	{
		return await _signInManager.CheckPasswordSignInAsync(
			user: usuario,
			password: senha,
			lockoutOnFailure: true);
	}

	public async Task<IdentityResult> SetLockoutEnabledAsync(Usuario usuario, bool habilitado)
	{
		return await _userManager.SetLockoutEnabledAsync(usuario, habilitado);
	}

	public async Task<Usuario?> ObterPorIdAsync(int idUsuario)
	{
		return await _dbContext.Usuarios
			.SingleOrDefaultAsync(usuario => usuario.Id == idUsuario);
	}

	public async Task<Usuario?> ObterPorEmailAsync(string email)
	{
		return await _dbContext.Usuarios
			.SingleOrDefaultAsync(usuario => usuario.Email == email);
	}
}