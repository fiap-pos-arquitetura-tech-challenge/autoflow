using AutoFlow.Domain.Models;
using System.Linq.Expressions;

namespace AutoFlow.Application.Interfaces.Repositories
{
    public interface IRepositorio<TBaseModel> : IDisposable where TBaseModel : BaseModel
    {
        Task Adicionar(TBaseModel entidade);
        Task<TBaseModel?> ObterPorId(int id);
        Task<List<TBaseModel>> ObterTodos();
        Task Atualizar(TBaseModel entidade);
        Task Remover(int id);
    }
}
