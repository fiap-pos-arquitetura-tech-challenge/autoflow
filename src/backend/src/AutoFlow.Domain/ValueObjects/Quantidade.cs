using AutoFlow.Domain.Exceptions;

namespace AutoFlow.Domain.ValueObjects.Comum;

public sealed class Quantidade
{
    public int Valor { get; private set; }

    private Quantidade()
    {
    }

    public Quantidade(int valor)
    {
        if (valor < 0)
            throw new EstoqueInvalidoException(
                "Quantidade não pode ser negativa.");

        Valor = valor;
    }
}