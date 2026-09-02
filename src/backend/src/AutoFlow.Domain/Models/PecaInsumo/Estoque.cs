using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.ValueObjects.Comum;

namespace AutoFlow.Domain.Models;

public class Estoque : BaseModel
{
    public PecaInsumo PecaInsumo { get; private set; }
    public Quantidade Quantidade { get; private set; }

    public Estoque()
    {
        PecaInsumo = null!;
        Quantidade = null!;
    }

    public Estoque(PecaInsumo pecaInsumo, int quantidadeInicial = 0)
    {
        if (pecaInsumo is null)
            throw new EstoqueInvalidoException(
                "Peça/insumo é obrigatório.");

        if (quantidadeInicial < 0)
            throw new EstoqueInvalidoException(
                "Quantidade inicial não pode ser negativa.");

        PecaInsumo = pecaInsumo;
        Quantidade = new Quantidade(quantidadeInicial);
    }

    public void Entrada(int quantidade)
    {
        ValidarQuantidadeMovimentacao(quantidade);

        Quantidade = new Quantidade(
            Quantidade.Valor + quantidade);
    }

    public void Saida(int quantidade)
    {
        ValidarQuantidadeMovimentacao(quantidade);

        var novaQuantidade =
            Quantidade.Valor - quantidade;

        if (novaQuantidade < 0)
        {
            throw new EstoqueInvalidoException(
                $"Estoque insuficiente. " +
                $"Disponível: {Quantidade.Valor}. " +
                $"Solicitado: {quantidade}.");
        }

        Quantidade = new Quantidade(novaQuantidade);
    }

    public void Ajustar(int quantidade)
    {
        if (quantidade < 0)
        {
            throw new EstoqueInvalidoException(
                "Quantidade de ajuste não pode ser negativa.");
        }

        Quantidade = new Quantidade(quantidade);

    }

    private static void ValidarQuantidadeMovimentacao(
        int quantidade)
    {
        if (quantidade <= 0)
        {
            throw new EstoqueInvalidoException(
                "A quantidade deve ser maior que zero.");
        }
    }
}