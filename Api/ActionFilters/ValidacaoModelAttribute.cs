using Application.Common.Validacoes.Erros;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Api.ActionFilters;

public class ValidacaoModelAttribute : ActionFilterAttribute
{
	public override void OnActionExecuting(ActionExecutingContext context)
	{
		List<ErroValidacao> validationErrors = new();
		if (context.ModelState.IsValid) return;

		foreach (var model in context.ModelState)
		{
			foreach (ModelError error in model.Value.Errors)
			{
				if (error.ErrorMessage.Contains("JSON"))
				{
					context.Result = new BadRequestObjectResult(new
					{
						status = "error",
						message = "O JSON fornecido é inválido, por favor, forneça um válido e tente novamente."
					});
					return;
				}

				validationErrors.Add(new ErroValidacao(model.Key, error.ErrorMessage));
			}
		}

		context.Result = new BadRequestObjectResult(validationErrors);
	}
}