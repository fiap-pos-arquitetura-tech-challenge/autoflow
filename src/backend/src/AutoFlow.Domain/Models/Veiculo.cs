using AutoFlow.Domain.Enums;
using AutoFlow.Domain.Exceptions;
using AutoFlow.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFlow.Domain.Models
{
    public class Veiculo : BaseModel
    {
        public int ClienteId { get; private set; }

        public Cliente Cliente { get; private set; }

        public string Marca { get; private set; }

        public string Modelo { get; private set; }

        public int AnoFabricacao { get; private set; }

        public int AnoModelo { get; private set; }

        public string Cor { get; private set; }

        public TipoVeiculo Tipo { get; private set; }

        public Combustivel Combustivel { get; private set; }

        public Placa Placa { get; private set; }

        public Chassi Chassi { get; private set; }

        public Quilometragem Quilometragem { get; private set; }

        public Veiculo()
        {

        }
        public Veiculo(
            int clienteId,
            string marca,
            string modelo,
            int anoFabricacao,
            int anoModelo,
            string cor,
            TipoVeiculo tipo,
            Combustivel combustivel,
            Placa placa,
            Chassi chassi,
            Quilometragem quilometragem)
        {
            ClienteId = clienteId;
            Marca = marca;
            Modelo = modelo;
            AnoFabricacao = anoFabricacao;
            AnoModelo = anoModelo;
            Cor = cor;
            Tipo = tipo;
            Combustivel = combustivel;
            Placa = placa;
            Chassi = chassi;
            Quilometragem = quilometragem;

            Validar();
        }

        private void Validar()
        {
            if (string.IsNullOrWhiteSpace(Marca))
                throw new ArgumentException("Marca é obrigatória.");

            if (string.IsNullOrWhiteSpace(Modelo))
                throw new ArgumentException("Modelo é obrigatório.");

            if (AnoFabricacao < 1900)
                throw new ArgumentException("Ano de fabricação inválido.");
        }

        public void AtualizarQuilometragem(int novaQuilometragem)
        {
            if (novaQuilometragem < Quilometragem.Valor)
                throw new QuilometragemInvalidaException(
                    "A quilometragem não pode diminuir.");

            Quilometragem = new Quilometragem(novaQuilometragem);
        }

        public void Atualizar(
            int clienteId,
            string marca,
            string modelo,
            int anoFabricacao,
            int anoModelo,
            string cor,
            TipoVeiculo tipo,
            Combustivel combustivel,
            string placa,
            string chassi,
            int quilometragem)
        {
            ClienteId = clienteId;
            Marca = marca;
            Modelo = modelo;
            AnoFabricacao = anoFabricacao;
            AnoModelo = anoModelo;
            Cor = cor;
            Tipo = tipo;
            Combustivel = combustivel;
            Placa = new Placa(placa);
            Chassi = new Chassi(chassi);
            Quilometragem = new Quilometragem(quilometragem);

            Validar();
        }
    }
}
