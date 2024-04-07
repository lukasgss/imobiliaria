using Application.Common.Interfaces.Entidades.Imoveis.DTOs;
using FluentValidation;

namespace Application.Common.Validacoes.ValidacoesImovel;

public class ValidacaoEdicaoImovel : AbstractValidator<EditarImovelRequest>
{
	public ValidacaoEdicaoImovel()
	{
		RuleFor(imovel => imovel.Endereco)
			.NotEmpty()
			.WithMessage("Campo de endereço é obrigatório.")
			.MaximumLength(255)
			.WithMessage("Máximo de 255 caracteres permitidos.");

		RuleFor(imovel => imovel.Cep)
			.NotEmpty()
			.WithMessage("Campo de cep é obrigatório.")
			.MaximumLength(20)
			.WithMessage("Máximo de 20 caracteres permitidos.");

		RuleFor(imovel => imovel.Cidade)
			.NotEmpty()
			.WithMessage("Campo de cidade é obrigatório.")
			.MaximumLength(50)
			.WithMessage("Máximo de 50 caracteres permitidos.");

		RuleFor(imovel => imovel.Estado)
			.NotEmpty()
			.WithMessage("Campo de estado é obrigatório.")
			.MaximumLength(50)
			.WithMessage("Máximo de 50 caracteres permitidos.");

		RuleFor(imovel => imovel.Bairro)
			.NotEmpty()
			.WithMessage("Campo de bairro é obrigatório.")
			.MaximumLength(255)
			.WithMessage("Máximo de 255 caracteres permitidos.");

		RuleFor(imovel => imovel.Complemento)
			.MaximumLength(20)
			.WithMessage("Máximo de 20 caracteres permitidos.");

		RuleFor(imovel => imovel.Numero)
			.NotEmpty()
			.WithMessage("Campo de número é obrigatório.");
	}
}