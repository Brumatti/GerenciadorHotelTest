using System;
using System.Linq;

namespace keko
{
    class Program
    {
        static void ReservarQuarto()
        {
            Console.Write("Nome do cliente: ");
            string nomeCliente = Console.ReadLine();

            Console.Write("Data de nascimento (dd mm aa): ");
            var dataNascimentoStr = Console.ReadLine().Split(' ');
            var dataNascimento = new Data()
            {
                Dia = int.Parse(dataNascimentoStr[0]),
                Mes = int.Parse(dataNascimentoStr[1]),
                Ano = int.Parse(dataNascimentoStr[2])
            };

            Console.Write("Data de entrada (dd mm aa): ");
            var dataEntradaStr = Console.ReadLine().Split(' ');
            var dataEntrada = new Data()
            {
                Dia = int.Parse(dataEntradaStr[0]),
                Mes = int.Parse(dataEntradaStr[1]),
                Ano = int.Parse(dataEntradaStr[2])
            };

            Console.Write("Data de saída (dd mm aa): ");
            var dataSaidaStr = Console.ReadLine().Split(' ');
            var dataSaida = new Data()
            {
                Dia = int.Parse(dataSaidaStr[0]),
                Mes = int.Parse(dataSaidaStr[1]),
                Ano = int.Parse(dataSaidaStr[2])
            };

            Console.Write("Tipo do quarto (Simples, Dupla, Tripla): ");
            var tipoQuarto = Enum.Parse<QuartoTipo>(Console.ReadLine());
            if (!HotelSistema.Instance().PossuiQuartoDisponivel(tipoQuarto))
            {
                Console.WriteLine("Não há quartos desse tipo disponível.");
                return;
            }

            var custoTotal = HotelSistema.Instance().EstimarValor(tipoQuarto, dataEntrada, dataSaida);
            Console.WriteLine("O valor estimado para sua estadia total é de: R$ {0}", custoTotal);
            Console.Write("Confirmar? (sim/nao): ");
            var pagarAgora = Console.ReadLine();
            if (char.ToLower( pagarAgora[0] ) == 'n')
            {
                Console.WriteLine("Reserva cancelada.");
                return;
            }
            else
            {
                Console.WriteLine("OK!");
            }

            if (-1 == HotelSistema.Instance().ReservarQuarto(nomeCliente, dataNascimento, dataEntrada, dataSaida, tipoQuarto))
            {
                Console.WriteLine("O cliente não está cadastrado. Precisamos de mais algumas informações");

                Console.Write("Endereço: ");
                string endereco = Console.ReadLine();

                Console.Write("Telefone: ");
                string telefone = Console.ReadLine();

                Console.Write("Bairro: ");
                string bairro = Console.ReadLine();

                Console.Write("Cidade: ");
                string cidade = Console.ReadLine();

                Console.Write("Estado: ");
                string estado = Console.ReadLine();

                Console.Write("RG: ");
                string rg = Console.ReadLine();

                var cliente = new ClienteInfo()
                {
                    Nome = nomeCliente,
                    DataNascimento = dataNascimento,
                    Endereco = endereco,
                    Telefone = telefone,
                    Bairro = bairro,
                    Cidade = cidade,
                    Estado = estado,
                    Identidade = rg
                };
                HotelSistema.Instance().RegistrarCliente(cliente);
            }

            int quartoId = HotelSistema.Instance().ReservarQuarto(nomeCliente, dataNascimento, dataEntrada, dataSaida, tipoQuarto);
            Console.WriteLine("Quarto {0} reservado com sucesso!", quartoId);
        }

        static void ListarGastos()
        {
            var quartos = HotelSistema.Instance().Quartos();
            foreach (var q in quartos)
            {
                if (q.Disponibilidade == QuartoDisponibilidade.Reservado)
                {
                    Console.WriteLine("Gastos para o quarto {0}: ", q.Identificacao);
                    foreach (var g in q.Gastos)
                    {
                        Console.WriteLine("- {0}", g.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Quarto {0} não reservado.", q.Identificacao);
                }
                Console.WriteLine("-------------------");

            }
        }

        static void FecharConta()
        {
            Console.Write("Nome do cliente: ");
            string nomeCliente = Console.ReadLine();

            Console.Write("Data de nascimento (dd mm aa): ");
            var dataNascimentoStr = Console.ReadLine().Split(' ');
            var dataNascimento = new Data()
            {
                Dia = int.Parse(dataNascimentoStr[0]),
                Mes = int.Parse(dataNascimentoStr[1]),
                Ano = int.Parse(dataNascimentoStr[2])
            };

            var cliente = HotelSistema.Instance().PegarCliente(nomeCliente, dataNascimento);
            if (cliente == null)
            {
                Console.WriteLine("O cliente não está registrado no sistema!");
                return;
            }

            QuartoProxy quartoHospedado = null;

            var quartos = HotelSistema.Instance().Quartos();
            foreach (var q in quartos)
            {
                if (q.Hospede == null)
                {
                    continue;
                }
                if (q.Hospede.Identidade == cliente.Identidade)
                {
                    quartoHospedado = q;

                    // marcar quarto como disponivel
                    q.Disponibilidade = QuartoDisponibilidade.Disponivel;
                    q.Hospede = null;
                    q.Gastos.Clear();
                    break;
                }
            }

            if (quartoHospedado == null)
            {
                Console.WriteLine("O cliente não reservou nenhum quarto!");
                return;
            }

            var gastosTotais = HotelSistema.Instance().GastosTotais(quartoHospedado);
            Console.Write("Confirmar depósito de R$ {0}", gastosTotais);
            Console.ReadKey();

            HotelSistema.Instance().Financas().Depositar(gastosTotais);
            Console.WriteLine("Conta finalizada!");
        }

        static void RelatorioDiario()
        {
            var gpq = HotelSistema.Instance().GastosPorQuarto();

            var receitaTotal = 0.0;
            foreach (var (id, todosGastos) in gpq)
            {
                Console.WriteLine("Gastos para o quarto {0}: ", id);
                foreach (var (g, valor) in todosGastos)
                {
                    Console.WriteLine("- {0} (valor R$ {1})", g.ToString(), valor);

                    receitaTotal += valor;
                }
                Console.WriteLine("-------------------");
            }

            Console.WriteLine("Receita total do hotel: R$ {0}", receitaTotal);
        }

        static void Main(string[] args)
        {
            // adicionar 5 quartos para exemplo
            HotelSistema.Instance().Quartos().Add(new QuartoSimplesFactory().CriarQuarto());
            HotelSistema.Instance().Quartos().Add(new QuartoSimplesFactory().CriarQuarto());
            HotelSistema.Instance().Quartos().Add(new QuartoDuplaFactory().CriarQuarto());
            HotelSistema.Instance().Quartos().Add(new QuartoDuplaFactory().CriarQuarto());
            HotelSistema.Instance().Quartos().Add(new QuartoTriplaFactory().CriarQuarto());

            Console.WriteLine("Gerenciador de Hotel 1.0");
            while (true)
            {
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine("Menu");
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine("1) Reservar quarto");
                Console.WriteLine("2) Controle de gastos dos hóspedes");
                Console.WriteLine("3) Fechar as contas");
                Console.WriteLine("4) Gerar relatório do dia");

                Console.Write("> ");
                int opt = 0;
                int.TryParse( Console.ReadLine(), out opt );

                Console.Write("\n\n");

                switch (opt)
                {
                    case 1: /* Reservar quarto */
                        ReservarQuarto();
                        break;

                    case 2: /* Controle de gastos */
                        ListarGastos();
                        break;

                    case 3: /* Fechamento de conta */
                        FecharConta();
                        break;

                    case 4: /* Relatórios diários */
                        RelatorioDiario();
                        break;

                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }
            }
        }
    }
}
