using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace keko
{
    public struct Data
    {
        public int Dia {get; set;}
        public int Mes {get; set;}
        public int Ano {get; set;}
    }
    public class ClienteInfo
    {
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public Data DataNascimento { get; set; }
        public string Identidade { get; set; }
    }

    public class FinancasSistema
    {
        public void Depositar(double valor)
        {
            _valorDepositado += valor;
        }
        private double _valorDepositado = 0.0;
    }

    public class HotelSistema
    {
        HotelSistema()
        {
            _clientes = new List<ClienteInfo>();
            _quartos = new List<QuartoProxy>();
            _financas = new FinancasSistema();
            _gastosPorQuarto = new Dictionary<int, List<KeyValuePair<QuartoGasto, double>>>();
        }

        private static HotelSistema _singletonInstance;
        public static HotelSistema Instance()
        {
            if (_singletonInstance is null)
            {
                _singletonInstance = new HotelSistema();
            }
            return _singletonInstance;
        }
        public FinancasSistema Financas()
        {
            return _financas;
        }        
        public Dictionary<int, List<KeyValuePair<QuartoGasto, double>>> GastosPorQuarto()
        {
            return _gastosPorQuarto;
        }

        /* -- ADMINISTRACAO -- */
        public int ReservarQuarto(string nomeCliente, Data nascimentoCliente, Data diaEntrada, Data diaSaida, QuartoTipo tipo)
        {
            var clienteRegistrado = PegarCliente(nomeCliente, nascimentoCliente);
            if (clienteRegistrado is null)
            {
                return -1; // cliente nao registrado
            }

            return ReservarQuarto(clienteRegistrado, diaEntrada, diaSaida, tipo);
        }

        public int ReservarQuarto(ClienteInfo cliente, Data diaEntrada, Data diaSaida, QuartoTipo tipo)
        {
            foreach (var q in _quartos)
            {
                if ((q.Tipo() == tipo) && (q.Disponibilidade == QuartoDisponibilidade.Disponivel))
                {
                    q.Disponibilidade = QuartoDisponibilidade.Reservado;
                    q.Hospede = cliente;
                    q.Entrada = diaEntrada;
                    q.Saida = diaSaida;

                    q.ConsumirGasto(QuartoGasto.Diaria);
                    return q.Identificacao;
                }
            }

            return -1;
        }

        public void RegistrarCliente(ClienteInfo cliente)
        {
            _clientes.Add(cliente);
        }

        public ClienteInfo PegarCliente(string nomeCliente, Data nascimentoCliente)
        {
            return _clientes.FirstOrDefault(x => (x.Nome == nomeCliente) && 
            (x.DataNascimento.Dia == nascimentoCliente.Dia) &&
            (x.DataNascimento.Mes == nascimentoCliente.Mes) &&
            (x.DataNascimento.Ano == nascimentoCliente.Ano));
        }

        public bool PossuiQuartoDisponivel(QuartoTipo tipo)
        {
            foreach (var q in _quartos)
            {
                if ((q.Tipo() == tipo) && (q.Disponibilidade == QuartoDisponibilidade.Disponivel))
                {
                    return true;
                }
            }
            return false;
        }

        public List<QuartoProxy> Quartos() { return this._quartos; }
        
        /* -- GASTOS -- */
        public double CustoDaDiaria(QuartoTipo tipo)
        {
            switch (tipo)
            {
                case QuartoTipo.Simples: return 35.00;
                case QuartoTipo.Dupla: return 70.00;
                case QuartoTipo.Tripla: return 120.00;
                default:
                    return 0.0;
            }
        }

        public double GastosTotais(QuartoProxy quarto)
        {
            var ans = EstimarValor(quarto.Tipo(), quarto.Entrada, quarto.Saida);
            foreach (var g in quarto.Gastos)
            {
                ans += CustoGasto(quarto.Tipo(), g);
            }

            return ans;
        }

        public double CustoGasto(QuartoTipo tipoQuarto, QuartoGasto gasto)
        {
            switch (gasto)
            {
                case QuartoGasto.Alimentacao: return 12.0;
                case QuartoGasto.Telefone: return 3.0;
                case QuartoGasto.Diaria: return CustoDaDiaria(tipoQuarto);
                default: return 0.0;
            }
        }

        public void RegistrarGasto(QuartoProxy quarto, QuartoGasto gasto)
        {
            if (!_gastosPorQuarto.ContainsKey(quarto.Identificacao))
            {
                this._gastosPorQuarto[quarto.Identificacao] = new List<KeyValuePair<QuartoGasto, double>> ();
            }

            this._gastosPorQuarto[quarto.Identificacao].Add(new KeyValuePair<QuartoGasto, double>(gasto, CustoGasto(quarto.Tipo(), gasto)));
        }

        public double EstimarValor(QuartoTipo quarto, Data entrada, Data saida)
        {
            var qntdDias = (new DateTime(saida.Ano, saida.Mes, saida.Dia) - new DateTime(entrada.Ano, entrada.Mes, entrada.Dia)).TotalDays;
            return qntdDias * CustoDaDiaria(quarto);
        }

        /* -- UTILITÁRIOS -- */
        public int GerarIdentificadorParaQuarto()
        {
            if ((_quartos == null) || ( _quartos.Count == 0))
            {
                return 0;
            }

            return _quartos.Max(x => x.Identificacao) + 1;
        }

        /* -- MEMBROS PRIVADOS -- */
        private List<ClienteInfo> _clientes;
        private Dictionary<int, List<KeyValuePair<QuartoGasto, double>>> _gastosPorQuarto;
        private List<QuartoProxy> _quartos;
        private FinancasSistema _financas;
    }
}
