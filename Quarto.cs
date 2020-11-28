using System;
using System.Collections.Generic;
using System.Text;

namespace keko
{
    public enum QuartoTipo
    {
        Simples,
        Dupla,
        Tripla
    }
    public enum QuartoGasto
    {
        Telefone,
        Diaria,
        Alimentacao
    }
    public enum QuartoDisponibilidade
    {
        Reservado,
        Disponivel
    }

    public abstract class QuartoFactory
    {
        public abstract QuartoProxy CriarQuarto();
    }

    public class QuartoSimplesFactory : QuartoFactory
    {
        public override QuartoProxy CriarQuarto()
        {
            var ans = new QuartoSimples();
            ans.Disponibilidade = QuartoDisponibilidade.Disponivel;
            ans.Hospede = null;
            ans.Identificacao = HotelSistema.Instance().GerarIdentificadorParaQuarto();
            
            return ans;
        }
    }    
    
    public class QuartoDuplaFactory : QuartoFactory
    {
        public override QuartoProxy CriarQuarto()
        {
            var ans = new QuartoDupla();
            ans.Disponibilidade = QuartoDisponibilidade.Disponivel;
            ans.Hospede = null;
            ans.Identificacao = HotelSistema.Instance().GerarIdentificadorParaQuarto();

            return ans;
        }
    }    
    
    public class QuartoTriplaFactory : QuartoFactory
    {
        public override QuartoProxy CriarQuarto()
        {
            var ans = new QuartoTripla();
            ans.Disponibilidade = QuartoDisponibilidade.Disponivel;
            ans.Hospede = null;
            ans.Identificacao = HotelSistema.Instance().GerarIdentificadorParaQuarto();

            return ans;
        }
    }

    public abstract class QuartoProxy
    {
        public QuartoProxy()
        {
            Gastos = new List<QuartoGasto>();
        }

        public ClienteInfo Hospede { get; set; }
        public QuartoDisponibilidade Disponibilidade { get; set; }
        public int Identificacao { get; set; }
        public List<QuartoGasto> Gastos { get; set; }
        public Data Entrada { get; set; }
        public Data Saida { get; set; }

        public void ConsumirGasto(QuartoGasto gasto)
        {
            Gastos.Add(gasto);
            HotelSistema.Instance().RegistrarGasto(this, gasto);
        }

        public abstract QuartoTipo Tipo();
        public abstract double CustoDaDiaria();
    }

    public class QuartoSimples : QuartoProxy
    {
        public override QuartoTipo Tipo() { return QuartoTipo.Simples; }
        public override double CustoDaDiaria()
        {
            return HotelSistema.Instance().CustoDaDiaria(Tipo());
        }
    }

    public class QuartoDupla : QuartoProxy
    {
        public override QuartoTipo Tipo() { return QuartoTipo.Dupla; }
        public override double CustoDaDiaria()
        {
            return HotelSistema.Instance().CustoDaDiaria(Tipo());
        }
    }

    public class QuartoTripla : QuartoProxy
    {
        public override QuartoTipo Tipo() { return QuartoTipo.Tripla; }
        public override double CustoDaDiaria()
        {
            return HotelSistema.Instance().CustoDaDiaria(Tipo());
        }
    }
}
