using Application.Common.Interfaces.Entidades.DTOs;
using FluentValidation;

namespace Application.Common.Validacoes.ValidacoesUsuario;

public class ValidacaoCadastroUsuario : AbstractValidator<CriarUsuarioRequest>
{
	public ValidacaoCadastroUsuario()
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

		RuleFor(usuario => usuario.Senha)
			.NotEmpty()
			.WithMessage("Campo de senha é obrigatório.")
			.MaximumLength(255)
			.WithMessage("Máximo de 255 caracteres permitidos.");

		RuleFor(usuario => usuario.ConfirmarSenha)
			.NotEmpty()
			.WithMessage("Campo de confirmar senha é obrigatório.")
			.Equal(usuario => usuario.Senha)
			.WithMessage("Campo de senha e confirmar senha devem ser iguais.");
	}
}