using AutoFlow.Application.Interfaces.DTOs;
using AutoFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.Application.DTOs
{
    public record VeiculoDto
    (
        int Id,
        int ClienteID,
        string Marca,
        string Modelo,
        int AnoFabricacao,
        int AnoModelo,
        string Cor,
        TipoVeiculo Tipo,
        Combustivel Combustivel,
        string Placa,
        string Chassi,
        int Quilometragem
    );

    public record CriaVeiculoDto
    (
        int ClienteId,
        string Marca,
        string Modelo,
        int AnoFabricacao,
        int AnoModelo,
        string Cor,
        TipoVeiculo Tipo,
        Combustivel Combustivel,
        string Placa,
        string Chassi,
        int Quilometragem
    ) : IVeiculoDTO;

    public record AtualizaVeiculoDto
    (
        int ClienteId,
        string Marca,
        string Modelo,
        int AnoFabricacao,
        int AnoModelo,
        string Cor,
        TipoVeiculo Tipo,
        Combustivel Combustivel,
        string Placa,
        string Chassi,
        int Quilometragem
    ) : IVeiculoDTO;
    
}
