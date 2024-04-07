using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entidades;

public class Locacao
{
	public int Id { get; set; }

	[Required, ForeignKey("ImovelId")]
	public virtual Imovel Imovel { get; set; } = null!;

	public int ImovelId { get; set; }

	[Required, ForeignKey("LocadorId")]
	public virtual Usuario Locador { get; set; } = null!;

	public int LocadorId { get; set; }

	[Required, ForeignKey("LocatarioId")]
	public virtual Usuario Locatario { get; set; } = null!;

	public int LocatarioId { get; set; }

	[Required]
	public bool LocadorAssinou { get; set; } = false;

	[Required]
	public bool LocatarioAssinou { get; set; } = false;

	public DateTime? DataFechamento { get; set; }

	[Required]
	public DateOnly DataVencimento { get; set; }

	[Required, Column(TypeName = "decimal(12, 2)")]
	public decimal ValorMensal { get; set; }
}