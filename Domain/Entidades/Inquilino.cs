using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entidades;

public class Inquilino
{
	public int Id { get; set; }
	
	[Required, MaxLength(255)] 
	public string NomeCompleto { get; set; } = null!;

	[Required, MaxLength(20)]
	public string NumeroTelefone { get; set; } = null!;

	[ForeignKey("ImovelId")]
	public virtual Imovel? Imovel { get; set; }
	public int? ImovelId { get; set; }
}