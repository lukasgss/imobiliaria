using Application.Common.Exceptions;
using Application.Common.Extensions.Mapeamento;
using Application.Common.Interfaces.Entidades.Imoveis;
using Application.Common.Interfaces.Entidades.Locacoes;
using Application.Common.Interfaces.Entidades.Locacoes.DTOs;
using Application.Common.Interfaces.Entidades.Usuarios;
using Application.Common.Interfaces.Providers;
using Domain.Entidades;
using Microsoft.Extensions.Logging;

namespace Application.Services.Entidades;

public class LocacaoService : ILocacaoService
{
	private readonly ILocacaoRepository _locacaoRepository;
	private readonly IImovelRepository _imovelRepository;
	private readonly IUsuarioRepository _usuarioRepository;
	private readonly IValueProvider _valueProvider;
	private readonly ILogger<LocacaoService> _logger;

	public LocacaoService(
		ILocacaoRepository locacaoRepository,
		IImovelRepository imovelRepository,
		IUsuarioRepository usuarioRepository,
		IValueProvider valueProvider,
		ILogger<LocacaoService> logger)
	{
		_locacaoRepository = locacaoRepository ?? throw new ArgumentNullException(nameof(locacaoRepository));
		_imovelRepository = imovelRepository ?? throw new ArgumentNullException(nameof(imovelRepository));
		_usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<LocacaoResponse> ObterPorIdAsync(int locacaoId)
	{
		Locacao? locacao = await _locacaoRepository.ObterPorIdAsync(locacaoId);
		if (locacao is null)
		{
			_logger.LogInformation("Locação com id {IdLocacao} não existe.", locacaoId);
			throw new NotFoundException("Locação com o id especificado não existe.");
		}

		return locacao.ToLocacaoResponse();
	}

	// A pessoa que está cadastrando a locação é sempre o dono do imóvel
	public async Task<LocacaoResponse> CadastrarAsync(CriarLocacaoRequest criarLocacaoRequest, int idLocador)
	{
		ValidarSeLocadorELocatarioSaoOsMesmos(criarLocacaoRequest.IdLocatario, idLocador);

		Imovel imovel = await ValidarEObterImovelAsync(criarLocacaoRequest.IdImovel);

		await ValidarSeImovelJaPossuiLocacaoParaCadastroAsync(criarLocacaoRequest.IdImovel);

		ValidarSeUsuarioEProprietarioDoImovel(imovel, idLocador);

		Usuario locatario = await ValidarEObterLocatario(criarLocacaoRequest.IdLocatario);
		Usuario? locador = await _usuarioRepository.ObterPorIdAsync(idLocador);

		Locacao locacaoParaCriar = new()
		{
			Imovel = imovel,
			Locador = locador!,
			Locatario = locatario,
			LocadorAssinou = false,
			LocatarioAssinou = false,
			DataFechamento = null,
			DataVencimento = criarLocacaoRequest.DataVencimento,
			ValorMensal = criarLocacaoRequest.ValorMensal
		};

		_locacaoRepository.Add(locacaoParaCriar);
		await _locacaoRepository.CommitAsync();

		return locacaoParaCriar.ToLocacaoResponse();
	}

	public async Task<LocacaoResponse> EditarAsync(EditarLocacaoRequest editarLocacaoRequest, int idLocador,
		int idLocacao)
	{
		ValidarSeLocadorELocatarioSaoOsMesmos(editarLocacaoRequest.IdLocatario, idLocador);

		Locacao locacaoDb = await ValidarEObterLocacaoAsync(idLocacao);
		if (locacaoDb.Locador.Id != idLocador)
		{
			_logger.LogInformation("Usuário {IdUsuario} tentou editar locação de {IdLocador}",
				idLocador, locacaoDb.Locador.Id);
			throw new UnauthorizedException("Não é possível editar locações em que não é o locador.");
		}

		Imovel imovel = await ValidarEObterImovelAsync(editarLocacaoRequest.IdImovel);

		await ValidarSeImovelJaPossuiLocacaoParaEdicaoAsync(editarLocacaoRequest.IdImovel, idLocacao);
		ValidarSeUsuarioEProprietarioDoImovel(imovel, idLocador);

		Usuario locatario = await ValidarEObterLocatario(editarLocacaoRequest.IdLocatario);

		locacaoDb.Imovel = imovel;
		locacaoDb.Locatario = locatario;
		locacaoDb.DataVencimento = editarLocacaoRequest.DataVencimento;
		locacaoDb.ValorMensal = editarLocacaoRequest.ValorMensal;
		// Quando o locatário alterar dados da locação, é necessário que ambos assinem novamente.
		// Data de fechamento é setada como nulo, indicando como se não houvesse sido assinado
		// por ambas as partes ainda
		locacaoDb.LocadorAssinou = false;
		locacaoDb.LocatarioAssinou = false;
		locacaoDb.DataFechamento = null;

		await _locacaoRepository.CommitAsync();

		return locacaoDb.ToLocacaoResponse();
	}

	public async Task DeletarAsync(int idLocacao, int idUsuario)
	{
		Locacao locacao = await ValidarEObterLocacaoAsync(idLocacao);
		if (locacao.Locador.Id != idUsuario)
		{
			_logger.LogInformation("Usuário {IdUsuario} tentou excluir locação de {IdLocador}",
				idUsuario, locacao.Locador.Id);
			throw new UnauthorizedException("Não é possível excluir locações em que não é o locador.");
		}

		_locacaoRepository.Delete(locacao);
		await _locacaoRepository.CommitAsync();
	}

	public async Task<LocacaoResponse> AssinarAsync(int idLocacao, int idUsuario)
	{
		Locacao locacao = await ValidarEObterLocacaoAsync(idLocacao);
		// Caso o usuário seja o locador
		if (locacao.Locador.Id == idUsuario)
		{
			ChecarSeUsuarioJaAssinou(locacao.LocadorAssinou);

			locacao.LocadorAssinou = true;
			if (locacao.LocatarioAssinou)
			{
				locacao.DataFechamento = _valueProvider.UtcNow();

				Imovel imovel = (await _imovelRepository.ObterPorIdAsync(locacao.Imovel.Id))!;
				Usuario inquilino = (await _usuarioRepository.ObterPorIdAsync(locacao.Locatario.Id))!;
				imovel.Inquilino = inquilino;
			}
		}

		// Caso o usuário seja o locatário
		else if (locacao.Locatario.Id == idUsuario)
		{
			ChecarSeUsuarioJaAssinou(locacao.LocatarioAssinou);

			locacao.LocatarioAssinou = true;
			if (locacao.LocadorAssinou)
			{
				locacao.DataFechamento = _valueProvider.UtcNow();

				Imovel imovel = (await _imovelRepository.ObterPorIdAsync(locacao.Imovel.Id))!;
				Usuario inquilino = (await _usuarioRepository.ObterPorIdAsync(locacao.Locatario.Id))!;
				imovel.Inquilino = inquilino;
			}
		}
		else
		{
			_logger.LogInformation(
				"Usuário {IdUsuario} tentou assinar locação {IdLocacao}, em que não é nem locador e nem locatário.",
				idUsuario, idLocacao);
			throw new UnauthorizedException("Não é possível assinar locações em que não é locador ou locatário.");
		}

		await _locacaoRepository.CommitAsync();
		return locacao.ToLocacaoResponse();
	}

	private async Task<Locacao> ValidarEObterLocacaoAsync(int idLocacao)
	{
		Locacao? locacao = await _locacaoRepository.ObterPorIdAsync(idLocacao);
		if (locacao is null)
		{
			_logger.LogInformation("Locação com id {IdLocacao} não existe.", idLocacao);
			throw new NotFoundException("Locação com o id especificado não existe.");
		}

		return locacao;
	}

	private async Task<Imovel> ValidarEObterImovelAsync(int idImovel)
	{
		Imovel? imovel = await _imovelRepository.ObterPorIdAsync(idImovel);
		if (imovel is null)
		{
			_logger.LogInformation("Imóvel com id {IdImovel} não existe.", idImovel);
			throw new NotFoundException("Imóvel com o id especificado não existe.");
		}

		return imovel;
	}

	private void ValidarSeUsuarioEProprietarioDoImovel(Imovel imovel, int idUsuario)
	{
		if (imovel.Proprietario.Id != idUsuario)
		{
			_logger.LogInformation(
				"Usuário {IdUsuario} tentou realizar ações de locação em imóvel {IdImovel}, no qual não é dono.",
				idUsuario, imovel.Id);
			throw new UnauthorizedException(
				"Não é possível realizar ou editar locações de imóveis em que não é proprietário.");
		}
	}

	private async Task<Usuario> ValidarEObterLocatario(int idLocatario)
	{
		Usuario? locatario = await _usuarioRepository.ObterPorIdAsync(idLocatario);
		if (locatario is null)
		{
			_logger.LogInformation("Usuário com id {IdUsuario} não existe.", idLocatario);
			throw new NotFoundException("Usuário com o id especificado para locatário não existe.");
		}

		return locatario;
	}

	private void ValidarSeLocadorELocatarioSaoOsMesmos(int idLocatario, int idLocador)
	{
		if (idLocatario == idLocador)
		{
			_logger.LogInformation("Usuário de id {IdLocatario} tentou se cadastrar como locatário e locador.",
				idLocatario);
			throw new BadRequestException(
				"Não é possível ser o locatário e locador da locação.");
		}
	}

	private async Task ValidarSeImovelJaPossuiLocacaoParaCadastroAsync(int idImovel)
	{
		Locacao? locacaoDb = await _locacaoRepository.ObterPorIdDoImovelAsync(idImovel);
		if (locacaoDb is not null)
		{
			_logger.LogInformation("Imóvel {IdImovel} já possui uma locação cadastrada.", idImovel);
			throw new ConflictException("Imóvel já possui uma locação cadastrada.");
		}
	}

	private async Task ValidarSeImovelJaPossuiLocacaoParaEdicaoAsync(int idImovel, int idLocacao)
	{
		Locacao? locacaoDb = await _locacaoRepository.ObterPorIdDoImovelAsync(idImovel);
		if (locacaoDb is not null && locacaoDb.Id != idLocacao)
		{
			_logger.LogInformation("Locação de id {IdLocacao} já possui uma locação cadastrada.", idLocacao);
			throw new ConflictException("Imóvel já possui uma locação cadastrada.");
		}
	}

	private void ChecarSeUsuarioJaAssinou(bool usuarioJaAssinou)
	{
		if (usuarioJaAssinou)
		{
			_logger.LogInformation("Locação já foi assinada pelo usuário.");
			throw new BadRequestException("Locação já foi assinada por você, não é possível assiná-la novamente.");
		}
	}
}