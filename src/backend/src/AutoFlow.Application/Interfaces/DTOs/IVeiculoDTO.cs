using AutoFlow.Domain.Enums;

namespace AutoFlow.Application.Interfaces.DTOs
{
    public interface IVeiculoDTO
    {
        int ClienteId { get;}
        string Marca { get;}
        string Modelo { get;}
        int AnoFabricacao { get;}
        int AnoModelo { get; }
        string Cor { get;}
        TipoVeiculo Tipo { get;}
        Combustivel Combustivel { get;}
        string Placa { get;}
        string Chassi { get;}
        int Quilometragem { get;}
    }
}
