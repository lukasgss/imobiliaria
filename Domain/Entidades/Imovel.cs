using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entidades;

public class Imovel
{
	public int Id { get; set; }

	[Required, MaxLength(255)]
	public string Endereco { get; set; } = null!;

	[Required, MaxLength(20)]
	public string Cep { get; set; } = null!;

	[Required, MaxLength(50)]
	public string Cidade { get; set; } = null!;

	[Required, MaxLength(50)]
	public string Estado { get; set; } = null!;

	[Required, MaxLength(50)]
	public string Bairro { get; set; } = null!;

	[Required]
	public int Numero { get; set; }

	[MaxLength(20)]
	public string? Complemento { get; set; }

	[Required, ForeignKey("ProprietarioId")]
	public virtual Usuario Proprietario { get; set; } = null!;

	public int ProprietarioId { get; set; }

	[ForeignKey("CorretorId")]
	public virtual Usuario? Corretor { get; set; }

	public int? CorretorId { get; set; }

	[ForeignKey("InquilinoId")]
	public virtual Usuario? Inquilino { get; set; }

	public int? InquilinoId { get; set; }
}