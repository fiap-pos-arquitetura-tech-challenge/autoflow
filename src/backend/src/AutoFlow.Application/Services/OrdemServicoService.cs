using AutoFlow.Application.DTOs;
using AutoFlow.Application.Interfaces.Repositories;
using AutoFlow.Application.Interfaces.Services;
using AutoFlow.Application.Services.Enums;
using AutoFlow.Application.Validators;
using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.Models;

namespace AutoFlow.Application.Services
{
    public class OrdemServicoService(IOrdemServicoRepositorio ordemServicoRepositorio, IClienteRepositorio clienteRepositorio, IVeiculoRepositorio veiculoRepositorio,
        IServicoRepositorio servicoRepositorio, IPecaInsumoRepositorio pecaInsumoRepositorio, IEstoqueRepositorio estoqueRepositorio, IEmailService emailService) : IOrdemServicoService
    {
        private readonly IOrdemServicoRepositorio _ordemServicoRepositorio = ordemServicoRepositorio;
        private readonly IClienteRepositorio _clienteRepositorio = clienteRepositorio;
        private readonly IVeiculoRepositorio _veiculoRepositorio = veiculoRepositorio;
        private readonly IServicoRepositorio _servicoRepositorio = servicoRepositorio;
        private readonly IPecaInsumoRepositorio _pecaInsumoRepositorio = pecaInsumoRepositorio;
        private readonly IEstoqueRepositorio _estoqueRepositorio = estoqueRepositorio;
        private readonly IEmailService _emailService = emailService;

        public async Task<Result<OrdemServicoDto>> AdicionarAsync(CriaOrdemServicoDto dto)
        {
            var validador = OrdemServicoValidador.Validar(dto);

            if (validador is not null)
                return Falha<OrdemServicoDto>(validador);

            var cliente = await _clienteRepositorio.ObterPorIdAsync(dto.ClienteId);

            if (cliente is null)
                return Result<OrdemServicoDto>.Failure("Cliente não encontrado.", ErrorType.NotFound);

            var veiculo = await _veiculoRepositorio.ObterPorIdAsync(dto.VeiculoId);

            if (veiculo is null)
                return Result<OrdemServicoDto>.Failure("Veículo não encontrado.", ErrorType.NotFound);

            if (veiculo.ClienteId != dto.ClienteId)
                return Result<OrdemServicoDto>.Failure("O veículo informado não pertence ao cliente.", ErrorType.Validation);

            OrdemServico ordemServico;

            try
            {
                ordemServico = new OrdemServico(dto.ClienteId, dto.VeiculoId, dto.AvariasObservadas);
            }
            catch (OrdemServicoInvalidaException ex)
            {
                return Result<OrdemServicoDto>.Failure(ex.Message, ErrorType.Validation);
            }

            if (dto.Servicos is not null)
            {
                foreach (var servicoDto in dto.Servicos)
                {
                    var falhaServico = await TentarAdicionarServicoAsync(ordemServico, servicoDto);

                    if (falhaServico is not null)
                        return falhaServico;
                }
            }

            if (dto.Pecas is not null)
            {
                foreach (var pecaDto in dto.Pecas)
                {
                    var falhaPeca = await TentarAdicionarPecaAsync(ordemServico, pecaDto);

                    if (falhaPeca is not null)
                        return falhaPeca;
                }
            }

            await _ordemServicoRepositorio.AdicionarAsync(ordemServico);

            return Result<OrdemServicoDto>.Success(MapearParaDto(ordemServico));
        }

        public async Task<Result<OrdemServicoDto>> RegistrarAvariasAsync(int id, RegistraAvariasOrdemServicoDto dto)
        {
            return await AlterarOrdemAsync(id, ordem => ordem.RegistrarAvarias(dto.AvariasObservadas));
        }

        public async Task<Result<OrdemServicoDto>> IniciarDiagnosticoAsync(int id)
        {
            return await AlterarOrdemAsync(id, ordem => ordem.IniciarDiagnostico());
        }

        public async Task<Result<OrdemServicoDto>> RegistrarDiagnosticoAsync(int id, RegistraDiagnosticoOrdemServicoDto dto)
        {
            var validador = OrdemServicoValidador.Validar(dto);

            if (validador is not null)
                return Falha<OrdemServicoDto>(validador);

            return await AlterarOrdemAsync(id, ordem => ordem.RegistrarDiagnostico(dto.Diagnostico.Trim()));
        }

        public async Task<Result<OrdemServicoDto>> AdicionarServicoAsync(int id, AdicionaServicoOrdemServicoDto dto)
        {
            var validador = OrdemServicoValidador.Validar(dto);

            if (validador is not null)
                return Falha<OrdemServicoDto>(validador);

            var ordem = await _ordemServicoRepositorio.ObterCompletaPorIdAsync(id);

            if (ordem is null)
                return OrdemNaoEncontrada();

            var falha = await TentarAdicionarServicoAsync(ordem, dto);

            if (falha is not null)
                return falha;

            await _ordemServicoRepositorio.AtualizarAsync(ordem);

            return Result<OrdemServicoDto>.Success(MapearParaDto(ordem));
        }

        public async Task<Result<OrdemServicoDto>> RemoverServicoAsync(int id, int servicoId)
        {
            return await AlterarOrdemAsync(id, ordem => ordem.RemoverServico(servicoId));
        }

        public async Task<Result<OrdemServicoDto>> AdicionarPecaAsync(int id, AdicionaPecaOrdemServicoDto dto)
        {
            var validador = OrdemServicoValidador.Validar(dto);

            if (validador is not null)
                return Falha<OrdemServicoDto>(validador);

            var ordem = await _ordemServicoRepositorio.ObterCompletaPorIdAsync(id);

            if (ordem is null)
                return OrdemNaoEncontrada();

            var falha = await TentarAdicionarPecaAsync(ordem, dto);

            if (falha is not null)
                return falha;

            await _ordemServicoRepositorio.AtualizarAsync(ordem);

            return Result<OrdemServicoDto>.Success(MapearParaDto(ordem));
        }

        public async Task<Result<OrdemServicoDto>> RemoverPecaAsync(int id, int pecaId)
        {
            return await AlterarOrdemAsync(id, ordem => ordem.RemoverPeca(pecaId));
        }

        public async Task<Result<OrdemServicoDto>> GerarOrcamentoAsync(int id)
        {
            return await AlterarOrdemAsync(id, ordem => ordem.GerarOrcamento());
        }

        public async Task<Result<OrdemServicoDto>> AprovarOrcamentoAsync(int id, int clienteId)
        {
            var ordem = await _ordemServicoRepositorio.ObterCompletaPorIdAsync(id);

            if (ordem is null || ordem.ClienteId != clienteId)
                return OrdemNaoEncontrada();

            return await AprovarOrcamentoNaOrdemAsync(ordem);
        }

        public async Task<Result<OrdemServicoDto>> AprovarOrcamentoNotificacaoExternaAsync(int id)
        {
            var ordem = await _ordemServicoRepositorio.ObterCompletaPorIdAsync(id);

            if (ordem is null)
                return OrdemNaoEncontrada();

            return await AprovarOrcamentoNaOrdemAsync(ordem);
        }

        private async Task<Result<OrdemServicoDto>> AprovarOrcamentoNaOrdemAsync(OrdemServico ordem)
        {
            var statusAnterior = ordem.Status;
            var movimentacoesEstoque = new List<(Estoque Estoque, int Quantidade)>();

            var pecasAgrupadas = ordem.Pecas
                .GroupBy(x => x.PecaId)
                .Select(grupo => new
                {
                    PecaId = grupo.Key,
                    Descricao = grupo.First().Descricao,
                    Quantidade = grupo.Sum(x => x.Quantidade)
                });

            foreach (var item in pecasAgrupadas)
            {
                var estoque = await _estoqueRepositorio.ObterPorPecaInsumoAsync(item.PecaId);

                if (estoque is null)
                    return Result<OrdemServicoDto>.Failure($"Estoque da peça/insumo {item.PecaId} não encontrado.", ErrorType.NotFound);

                if (estoque.Quantidade.Valor < item.Quantidade)
                    return Result<OrdemServicoDto>.Failure(
                        $"Estoque insuficiente para {item.Descricao}. Disponível: {estoque.Quantidade.Valor}. Solicitado: {item.Quantidade}.",
                        ErrorType.Validation);

                movimentacoesEstoque.Add((estoque, item.Quantidade));
            }

            try
            {
                ordem.AprovarOrcamento();

                foreach (var movimentacao in movimentacoesEstoque)
                    movimentacao.Estoque.Saida(movimentacao.Quantidade);

                await _ordemServicoRepositorio.AtualizarAsync(ordem);
                await NotificarMudancaStatusAsync(ordem, statusAnterior);

                return Result<OrdemServicoDto>.Success(MapearParaDto(ordem));
            }
            catch (OrdemServicoInvalidaException ex)
            {
                return Result<OrdemServicoDto>.Failure(ex.Message, ErrorType.Conflict);
            }
            catch (EstoqueInvalidoException ex)
            {
                return Result<OrdemServicoDto>.Failure(ex.Message, ErrorType.Validation);
            }
        }

        public async Task<Result<OrdemServicoDto>> IniciarExecucaoServicoAsync(int id, int itemServicoId)
        {
            return await AlterarOrdemAsync(id, ordem => ordem.IniciarExecucaoServico(itemServicoId));
        }

        public async Task<Result<OrdemServicoDto>> FinalizarExecucaoServicoAsync(int id, int itemServicoId)
        {
            return await AlterarOrdemAsync(id, ordem => ordem.FinalizarExecucaoServico(itemServicoId));
        }

        public async Task<Result<OrdemServicoDto>> ReprovarOrcamentoAsync(int id, int clienteId, ReprovaOrcamentoOrdemServicoDto dto)
        {
            var validador = OrdemServicoValidador.Validar(dto);

            if (validador is not null)
                return Falha<OrdemServicoDto>(validador);

            var ordem = await _ordemServicoRepositorio.ObterCompletaPorIdAsync(id);

            if (ordem is null || ordem.ClienteId != clienteId)
                return OrdemNaoEncontrada();

            return await ReprovarOrcamentoNaOrdemAsync(ordem, dto.Justificativa);
        }

        public async Task<Result<OrdemServicoDto>> ReprovarOrcamentoNotificacaoExternaAsync(int id, ReprovaOrcamentoOrdemServicoDto dto)
        {
            var validador = OrdemServicoValidador.Validar(dto);

            if (validador is not null)
                return Falha<OrdemServicoDto>(validador);

            var ordem = await _ordemServicoRepositorio.ObterCompletaPorIdAsync(id);

            if (ordem is null)
                return OrdemNaoEncontrada();

            return await ReprovarOrcamentoNaOrdemAsync(ordem, dto.Justificativa);
        }

        private async Task<Result<OrdemServicoDto>> ReprovarOrcamentoNaOrdemAsync(OrdemServico ordem, string justificativa)
        {
            var statusAnterior = ordem.Status;

            try
            {
                ordem.ReprovarOrcamento(justificativa.Trim());
                await _ordemServicoRepositorio.AtualizarAsync(ordem);
                await NotificarMudancaStatusAsync(ordem, statusAnterior);

                return Result<OrdemServicoDto>.Success(MapearParaDto(ordem));
            }
            catch (OrdemServicoInvalidaException ex)
            {
                return Result<OrdemServicoDto>.Failure(ex.Message, ErrorType.Conflict);
            }
        }

        public async Task<Result<OrdemServicoDto>> FinalizarAsync(int id)
        {
            return await AlterarOrdemAsync(id, ordem => ordem.Finalizar());
        }

        public async Task<Result<OrdemServicoDto>> EntregarAsync(int id)
        {
            return await AlterarOrdemAsync(id, ordem => ordem.Entregar());
        }

        public async Task<Result<OrdemServicoDto>> ObterPorIdAsync(int id)
        {
            var ordem = await _ordemServicoRepositorio.ObterCompletaPorIdAsync(id);

            if (ordem is null)
                return OrdemNaoEncontrada();

            return Result<OrdemServicoDto>.Success(MapearParaDto(ordem));
        }

        public async Task<IEnumerable<OrdemServicoDto>> ObterTodosAsync()
        {
            var ordens = await _ordemServicoRepositorio.ObterTodasCompletasAsync();

            return ordens
                .Where(x => x.Status is not (StatusOrdemServico.Finalizada or StatusOrdemServico.Entregue))
                .OrderBy(x => PrioridadeListagem(x.Status))
                .ThenBy(x => x.DataAbertura)
                .Select(MapearParaDto);
        }

        private static int PrioridadeListagem(StatusOrdemServico status) => status switch
        {
            StatusOrdemServico.EmExecucao => 1,
            StatusOrdemServico.AguardandoAprovacao => 2,
            StatusOrdemServico.EmDiagnostico => 3,
            StatusOrdemServico.Recebida => 4,
            _ => int.MaxValue
        };

        public async Task<Result<OrcamentoClienteDto>> ConsultarOrcamentoClienteAsync(int id, int clienteId)
        {
            var ordem = await _ordemServicoRepositorio.ObterCompletaPorIdAsync(id);

            if (ordem is null || ordem.ClienteId != clienteId)
                return Result<OrcamentoClienteDto>.Failure("Ordem de serviço não encontrada.", ErrorType.NotFound);

            if (ordem.Orcamento is null)
                return Result<OrcamentoClienteDto>.Failure("Orçamento não encontrado.", ErrorType.NotFound);

            var dto = MapearParaDto(ordem);

            return Result<OrcamentoClienteDto>.Success(new OrcamentoClienteDto(
                ordem.Id,
                ordem.Status,
                dto.Servicos,
                dto.Pecas,
                dto.Orcamento!));
        }

        public async Task<Result<AndamentoOrdemServicoDto>> ConsultarAndamentoAsync(int id)
        {
            var ordem = await _ordemServicoRepositorio.ObterCompletaPorIdAsync(id);

            if (ordem is null)
                return Result<AndamentoOrdemServicoDto>.Failure("Ordem de serviço não encontrada.", ErrorType.NotFound);

            return Result<AndamentoOrdemServicoDto>.Success(new AndamentoOrdemServicoDto(ordem.Id, ordem.Status, ordem.DataAbertura, ordem.DiagnosticoIniciadoEm, ordem.OrcamentoGeradoEm,
                ordem.OrcamentoDecididoEm, ordem.ExecucaoIniciadaEm, ordem.FinalizadaEm, ordem.EntregueEm));
        }

        public async Task<Result<AndamentoOrdemServicoDto>> ConsultarAndamentoClienteAsync(int id, int clienteId)
        {
            var ordem = await _ordemServicoRepositorio.ObterCompletaPorIdAsync(id);

            if (ordem is null || ordem.ClienteId != clienteId)
                return Result<AndamentoOrdemServicoDto>.Failure("Ordem de serviço não encontrada.", ErrorType.NotFound);

            return Result<AndamentoOrdemServicoDto>.Success(new AndamentoOrdemServicoDto(ordem.Id, ordem.Status, ordem.DataAbertura, ordem.DiagnosticoIniciadoEm, ordem.OrcamentoGeradoEm,
                ordem.OrcamentoDecididoEm, ordem.ExecucaoIniciadaEm, ordem.FinalizadaEm, ordem.EntregueEm));
        }

        private async Task<Result<OrdemServicoDto>?> TentarAdicionarServicoAsync(OrdemServico ordem, AdicionaServicoOrdemServicoDto dto)
        {
            var servico = await _servicoRepositorio.ObterPorIdAsync(dto.ServicoId);

            if (servico is null)
                return Result<OrdemServicoDto>.Failure("Serviço não encontrado.", ErrorType.NotFound);

            try
            {
                ordem.AdicionarServico(servico.Id, servico.Nome, dto.Quantidade, servico.Preco, servico.TempoMedio);
                return null;
            }
            catch (OrdemServicoInvalidaException ex)
            {
                return Result<OrdemServicoDto>.Failure(ex.Message, ErrorType.Conflict);
            }
        }

        private async Task<Result<OrdemServicoDto>?> TentarAdicionarPecaAsync(OrdemServico ordem, AdicionaPecaOrdemServicoDto dto)
        {
            var peca = await _pecaInsumoRepositorio.ObterPorIdAsync(dto.PecaId);

            if (peca is null)
                return Result<OrdemServicoDto>.Failure("Peça/insumo não encontrada.", ErrorType.NotFound);

            var estoque = await _estoqueRepositorio.ObterPorPecaInsumoAsync(dto.PecaId);

            if (estoque is null)
                return Result<OrdemServicoDto>.Failure("Estoque da peça/insumo não encontrado.", ErrorType.NotFound);

            var quantidadeJaAdicionada = ordem.Pecas
                .Where(x => x.PecaId == dto.PecaId)
                .Sum(x => x.Quantidade);

            var quantidadeTotalNecessaria = quantidadeJaAdicionada + dto.Quantidade;

            if (estoque.Quantidade.Valor < quantidadeTotalNecessaria)
                return Result<OrdemServicoDto>.Failure(
                    $"Estoque insuficiente. Disponível: {estoque.Quantidade.Valor}. Solicitado no total da OS: {quantidadeTotalNecessaria}.",
                    ErrorType.Validation);

            try
            {
                ordem.AdicionarPeca(peca.Id, peca.Nome, dto.Quantidade, peca.Valor.Valor);
                return null;
            }
            catch (OrdemServicoInvalidaException ex)
            {
                return Result<OrdemServicoDto>.Failure(ex.Message, ErrorType.Conflict);
            }
        }

        private async Task<Result<OrdemServicoDto>> AlterarOrdemAsync(int id, Action<OrdemServico> acao)
        {
            var ordem = await _ordemServicoRepositorio.ObterCompletaPorIdAsync(id);

            if (ordem is null)
                return OrdemNaoEncontrada();

            var statusAnterior = ordem.Status;

            try
            {
                acao(ordem);
                await _ordemServicoRepositorio.AtualizarAsync(ordem);
                await NotificarMudancaStatusAsync(ordem, statusAnterior);

                return Result<OrdemServicoDto>.Success(MapearParaDto(ordem));
            }
            catch (OrdemServicoInvalidaException ex)
            {
                return Result<OrdemServicoDto>.Failure(ex.Message, ErrorType.Conflict);
            }
        }

        private async Task NotificarMudancaStatusAsync(OrdemServico ordem, StatusOrdemServico statusAnterior)
        {
            if (ordem.Status == statusAnterior)
                return;

            var cliente = await _clienteRepositorio.ObterPorIdAsync(ordem.ClienteId);

            if (cliente is null)
                return;

            try
            {
                await _emailService.EnviarAsync(
                    cliente.Email.Endereco,
                    $"Atualização da Ordem de Serviço #{ordem.Id}",
                    $"Sua Ordem de Serviço #{ordem.Id} agora está com status: {ordem.Status}.");
            }
            catch (Exception)
            {
            }
        }

        private static Result<T> Falha<T>(Result resultado)
        {
            return Result<T>.Failure(resultado.Error!, resultado.ErrorType);
        }

        private static Result<OrdemServicoDto> OrdemNaoEncontrada()
        {
            return Result<OrdemServicoDto>.Failure("Ordem de serviço não encontrada.", ErrorType.NotFound);
        }

        private static OrdemServicoDto MapearParaDto(OrdemServico ordem)
        {
            var servicos = ordem.Servicos.Select(item => new OrdemServicoItemServicoDto(
                item.Id,
                item.ServicoId,
                item.Descricao,
                item.Quantidade,
                item.ValorUnitario,
                item.TempoPrevisto,
                item.ExecucaoIniciadaEm,
                item.ExecucaoFinalizadaEm,
                item.Subtotal));

            var pecas = ordem.Pecas.Select(item => new OrdemServicoItemPecaDto(
                item.Id,
                item.PecaId,
                item.Descricao,
                item.Quantidade,
                item.ValorUnitario,
                item.Subtotal));

            OrcamentoDto? orcamento = null;

            if (ordem.Orcamento is not null)
            {
                orcamento = new OrcamentoDto(
                    ordem.Orcamento.Id,
                    ordem.Orcamento.Status,
                    ordem.Orcamento.ValorServicos,
                    ordem.Orcamento.ValorPecas,
                    ordem.Orcamento.ValorTotal,
                    ordem.Orcamento.GeradoEm,
                    ordem.Orcamento.DecididoEm,
                    ordem.Orcamento.JustificativaReprovacao);
            }

            return new OrdemServicoDto(ordem.Id, ordem.ClienteId, ordem.VeiculoId, ordem.Status, ordem.AvariasObservadas, ordem.Diagnostico, ordem.DataAbertura, ordem.DiagnosticoIniciadoEm,
                ordem.OrcamentoGeradoEm, ordem.OrcamentoDecididoEm, ordem.ExecucaoIniciadaEm, ordem.FinalizadaEm, ordem.EntregueEm, servicos, pecas, orcamento);
        }
    }
}
