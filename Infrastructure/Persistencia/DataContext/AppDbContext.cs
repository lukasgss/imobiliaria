using Domain.Entidades;
using Infrastructure.Conversoes;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistencia.DataContext;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public DbSet<Usuario> Usuarios { get; set; } = null!;
	public DbSet<Imovel> Imoveis { get; set; } = null!;
	public DbSet<Locacao> Locacoes { get; set; } = null!;

	protected override void ConfigureConventions(ModelConfigurationBuilder builder)
	{
		builder.Properties<DateOnly>()
			.HaveConversion<DateOnlyConverter>()
			.HaveColumnType("date");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Imovel>()
			.HasOne(imovel => imovel.Proprietario)
			.WithMany(usuario => usuario.ImoveisProprietario)
			.HasForeignKey(imovel => imovel.ProprietarioId)
			.IsRequired();

		modelBuilder.Entity<Imovel>()
			.HasOne(imovel => imovel.Corretor)
			.WithMany(usuario => usuario.ImoveisCorretor)
			.HasForeignKey(imovel => imovel.CorretorId)
			.IsRequired(false);

		modelBuilder.Entity<Imovel>()
			.HasOne(imovel => imovel.Inquilino)
			.WithMany(usuario => usuario.ImoveisInquilino)
			.HasForeignKey(imovel => imovel.InquilinoId)
			.IsRequired(false);

		modelBuilder.Entity<Locacao>()
			.HasOne(locacao => locacao.Locador)
			.WithMany(usuario => usuario.LocacaoLocador)
			.HasForeignKey(locacao => locacao.LocadorId)
			.OnDelete(DeleteBehavior.NoAction)
			.IsRequired();

		modelBuilder.Entity<Locacao>()
			.HasOne(locacao => locacao.Locatario)
			.WithMany(usuario => usuario.LocacaoLocatario)
			.HasForeignKey(locacao => locacao.LocatarioId)
			.OnDelete(DeleteBehavior.NoAction)
			.IsRequired();
	}
}