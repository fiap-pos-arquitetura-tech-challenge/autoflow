using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.Exceptions.PecaInsumo;
using AutoFlow.Domain.ValueObjects;

namespace AutoFlow.Domain.Models;

public class PecaInsumo : BaseModel
{
    public string Nome { get; private set; }
    public Dinheiro Valor { get; private set; }

    public PecaInsumo()
    {
        Nome = null!;
        Valor = null!;
    }

    public PecaInsumo(string nome, decimal valor)
    {
        ValidarNome(nome);

        Nome = nome;
        Valor = new Dinheiro(valor);
    }

    public void Atualizar(string nome, decimal valor)
    {
        ValidarNome(nome);

        Nome = nome;
        Valor = new Dinheiro(valor);
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new PecaInsumoInvalidaException(
                "Nome é obrigatório.");
    }
}