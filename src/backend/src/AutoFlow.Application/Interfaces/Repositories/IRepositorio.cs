using AutoFlow.Domain.Models;
using System.Linq.Expressions;

namespace AutoFlow.Application.Interfaces.Repositories
{
    public interface IRepositorio<TBaseModel> where TBaseModel : BaseModel
    {
        Task AdicionarAsync(TBaseModel entidade);
        Task AtualizarAsync(TBaseModel entidade);
        Task ExcluirAsync(TBaseModel entidade);
        Task<TBaseModel?> ObterPorIdAsync(int id);
        Task<List<TBaseModel>> ObterTodosAsync();
    }
}
