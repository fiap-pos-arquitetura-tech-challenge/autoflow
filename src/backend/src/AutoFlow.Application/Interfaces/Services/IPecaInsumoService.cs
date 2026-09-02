using AutoFlow.Application.DTOs;
using AutoFlow.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.Application.Interfaces.Services
{
    public interface IPecaInsumoService
    {
        public Task<Result<PecaInsumoDto>> AdicionarAsync(CriaPecaInsumoDto pecaInsumoDto);
        public Task<Result<PecaInsumoDto>> AtualizarAsync(int id, AtualizaPecaInsumoDto pecaInsumoDto);
        public Task<Result> ExcluirAsync(int id);
        public Task<Result<PecaInsumoDto>> ObterPorIdAsync(int id);
        public Task<IEnumerable<PecaInsumoDto>> ObterTodosAsync();
    }
}
