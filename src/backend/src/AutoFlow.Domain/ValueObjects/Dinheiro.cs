using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.Exceptions.PecaInsumo;

namespace AutoFlow.Domain.ValueObjects;

public sealed class Dinheiro
{
    public decimal Valor { get; private set; }

    private Dinheiro()
    {
    }

    public Dinheiro(decimal valor)
    {
        if (valor < 0)
            throw new PecaInsumoInvalidaException(
                "Valor não pode ser negativo.");

        Valor = decimal.Round(valor, 2);
    }

    public static implicit operator decimal(Dinheiro dinheiro)
        => dinheiro.Valor;

    public static implicit operator Dinheiro(decimal valor)
        => new(valor);
}