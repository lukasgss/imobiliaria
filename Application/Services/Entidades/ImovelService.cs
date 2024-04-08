using Application.Common.Exceptions;
using Application.Common.Extensions.Mapeamento;
using Application.Common.Interfaces.Entidades.Imoveis;
using Application.Common.Interfaces.Entidades.Imoveis.DTOs;
using Application.Common.Interfaces.Entidades.Usuarios;
using Domain.Entidades;
using Microsoft.Extensions.Logging;

namespace Application.Services.Entidades;

public class ImovelService : IImovelService
{
	private readonly IUsuarioRepository _usuarioRepository;
	private readonly IImovelRepository _imovelRepository;
	private readonly ILogger<ImovelService> _logger;

	public ImovelService(
		IUsuarioRepository usuarioRepository,
		IImovelRepository imovelRepository,
		ILogger<ImovelService> logger)
	{
		_usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
		_imovelRepository = imovelRepository ?? throw new ArgumentNullException(nameof(imovelRepository));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<ImovelResponse> ObterPorIdAsync(int imovelId)
	{
		Imovel? imovel = await _imovelRepository.ObterPorIdAsync(imovelId);
		if (imovel is null)
		{
			_logger.LogInformation("Imóvel com id {IdImovel} não existe.", imovelId);
			throw new NotFoundException("Imóvel com o id especificado não existe.");
		}

		return imovel.ToImovelResponse();
	}

	public async Task<IEnumerable<ImovelResponse>> ObterImoveisAlugados()
	{
		var imoveis = await _imovelRepository.ObterImoveisAlugados();

		return imoveis.ToImovelResponseList();
	}

	public async Task<IEnumerable<ImovelResponse>> ObterImoveisDisponiveis()
	{
		var imoveis = await _imovelRepository.ObterDisponiveisParaAluguel();

		return imoveis.ToImovelResponseList();
	}

	public async Task<ImovelResponse> CadastrarAsync(CriarImovelRequest request, int idUsuarioLogado)
	{
		Usuario? proprietario = await _usuarioRepository.ObterPorIdAsync(idUsuarioLogado);
		if (proprietario is null)
		{
			_logger.LogInformation("Usuário com id {IdUsuario} não existe.", idUsuarioLogado);
			throw new NotFoundException("Usuário com o id especificado para proprietário não existe.");
		}

		Usuario? corretor = null;
		if (request.CorretorId is not null)
		{
			corretor = await _usuarioRepository.ObterPorIdAsync(request.CorretorId.Value);
			if (corretor is null)
			{
				_logger.LogInformation("Usuário com id {IdUsuario} não existe.", request.CorretorId.Value);
				throw new NotFoundException("Usuário com o id especificado para corretor não existe.");
			}
		}

		Imovel imovel = new()
		{
			Endereco = request.Endereco,
			Cep = request.Cep,
			Cidade = request.Cidade,
			Estado = request.Estado,
			Bairro = request.Bairro,
			Numero = request.Numero,
			Complemento = request.Complemento,
			Proprietario = proprietario,
			Corretor = corretor,
			Inquilino = null
		};

		_imovelRepository.Add(imovel);
		await _imovelRepository.CommitAsync();

		return imovel.ToImovelResponse();
	}

	public async Task<ImovelResponse> EditarAsync(EditarImovelRequest request, int idUsuarioLogado, int idImovel)
	{
		Imovel? imovelParaEditar = await _imovelRepository.ObterPorIdAsync(idImovel);
		if (imovelParaEditar is null)
		{
			_logger.LogInformation("Imóvel com id {IdImovel} não existe.", idImovel);
			throw new NotFoundException("Imovel com o id especificado não existe.");
		}

		if (imovelParaEditar.Proprietario.Id != idUsuarioLogado)
		{
			_logger.LogInformation("Usuário {IdUsuario} tentou editar imóvel {IdImovel}, no qual não é proprietário.",
				idUsuarioLogado, idImovel);
			throw new ForbiddenException("Não é possível editar imóveis no qual não é proprietário.");
		}

		Usuario? corretor = null;
		if (request.CorretorId is not null)
		{
			corretor = await _usuarioRepository.ObterPorIdAsync(request.CorretorId.Value);
			if (corretor is null)
			{
				_logger.LogInformation("Usuário com o id {UsuarioId} não existe.", request.CorretorId.Value);
				throw new NotFoundException("Usuário com o id especificado para corretor não existe.");
			}
		}

		imovelParaEditar.Endereco = request.Endereco;
		imovelParaEditar.Complemento = request.Cep;
		imovelParaEditar.Cidade = request.Cidade;
		imovelParaEditar.Estado = request.Estado;
		imovelParaEditar.Bairro = request.Bairro;
		imovelParaEditar.Numero = request.Numero;
		imovelParaEditar.Complemento = request.Complemento;
		imovelParaEditar.Corretor = corretor;

		await _usuarioRepository.CommitAsync();

		return imovelParaEditar.ToImovelResponse();
	}

	public async Task DeletarAsync(int idImovel, int idUsuarioLogado)
	{
		Imovel? imovel = await _imovelRepository.ObterPorIdAsync(idImovel);
		if (imovel is null)
		{
			_logger.LogInformation("Imóvel com id {IdImovel} não existe.", idImovel);
			throw new NotFoundException("Imóvel com o id especificado não existe.");
		}

		if (imovel.Proprietario.Id != idUsuarioLogado)
		{
			_logger.LogInformation("Usuário {IdUsuario} tentou excluir imóvel {IdImovel}, no qual não é proprietário.",
				idUsuarioLogado, idImovel);
			throw new ForbiddenException("Não é possível excluir imóveis no qual não é proprietário.");
		}

		_imovelRepository.Delete(imovel);
		await _imovelRepository.CommitAsync();
	}
}