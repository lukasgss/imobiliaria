using System.ComponentModel.DataAnnotations;

namespace Domain.Entidades;

public class Corretor
{
	public int Id { get; set; }
	
	[Required, MaxLength(255)] 
	public string NomeCompleto { get; set; } = null!;

	[Required, MaxLength(20)]
	public string NumeroTelefone { get; set; } = null!;

	public virtual ICollection<Imovel> Imoveis { get; set; } = null!;
}