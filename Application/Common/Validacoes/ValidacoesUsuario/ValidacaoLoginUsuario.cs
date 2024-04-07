using Application.Common.Interfaces.Entidades.Usuarios.DTOs;
using FluentValidation;

namespace Application.Common.Validacoes.ValidacoesUsuario;

public class ValidacaoLoginUsuario : AbstractValidator<LoginRequest>
{
	public ValidacaoLoginUsuario()
	{
		RuleFor(usuario => usuario.Email)
			.NotEmpty()
			.WithMessage("Campo de e-mail é obrigatório.")
			.MaximumLength(100)
			.WithMessage("Máximo de 100 caracteres permitidos.")
			.EmailAddress()
			.WithMessage("Endereço de e-mail inválido.");

		RuleFor(usuario => usuario.Senha)
			.NotEmpty()
			.WithMessage("Campo de senha é obrigatório.")
			.MaximumLength(255)
			.WithMessage("Máximo de 255 caracteres permitidos.");
	}
}