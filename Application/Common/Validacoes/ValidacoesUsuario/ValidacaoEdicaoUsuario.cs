using Application.Common.Interfaces.Entidades.DTOs;
using FluentValidation;

namespace Application.Common.Validacoes.ValidacoesUsuario;

public class ValidacaoEdicaoUsuario : AbstractValidator<EditarUsuarioRequest>
{
	public ValidacaoEdicaoUsuario()
	{
		RuleFor(usuario => usuario.NomeCompleto)
			.NotEmpty()
			.WithMessage("Campo de nome completo é obrigatório.")
			.MaximumLength(255)
			.WithMessage("Máximo de 255 caracteres permitidos.");

		RuleFor(usuario => usuario.Telefone)
			.NotEmpty()
			.WithMessage("Campo de telefone é obrigatório.")
			.MaximumLength(30)
			.WithMessage("Máximo de 30 caracteres permitidos.");

		RuleFor(usuario => usuario.Email)
			.NotEmpty()
			.WithMessage("Campo de e-mail é obrigatório.")
			.MaximumLength(100)
			.WithMessage("Máximo de 100 caracteres permitidos.")
			.EmailAddress()
			.WithMessage("Endereço de e-mail inválido.");
	}
}