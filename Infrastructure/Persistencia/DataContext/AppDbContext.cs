using Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistencia.DataContext;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	public DbSet<Corretor> Corretores { get; set; } = null!;
	public DbSet<Imovel> Imoveis { get; set; } = null!;
	public DbSet<Inquilino> Inquilinos { get; set; } = null!;
}