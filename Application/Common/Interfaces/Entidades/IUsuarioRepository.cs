using Application.Common.Interfaces.GenericRepository;
using Domain.Entidades;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Interfaces.Entidades;

public interface IUsuarioRepository : IGenericRepository<Usuario>
{
	Task<IdentityResult> RegistrarAsync(Usuario usuario, string senha);
	Task<SignInResult> ChecarCredenciais(Usuario usuario, string senha);
	Task<IdentityResult> SetLockoutEnabledAsync(Usuario usuario, bool habilitado);
	Task<Usuario?> ObterPorIdAsync(int idUsuario);
	Task<Usuario?> ObterPorEmailAsync(string email);
}