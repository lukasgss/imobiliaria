using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entidades;

public class Usuario : IdentityUser<int>
{
	[Required, MaxLength(255)]
	public string NomeCompleto { get; set; } = null!;

	public virtual ICollection<Imovel> ImoveisProprietario { get; set; } = null!;
	public virtual ICollection<Imovel> ImoveisCorretor { get; set; } = null!;
	public virtual ICollection<Imovel> ImoveisInquilino { get; set; } = null!;
	public virtual ICollection<Locacao> LocacaoLocatario { get; set; } = null!;
	public virtual ICollection<Locacao> LocacaoLocador { get; set; } = null!;
}