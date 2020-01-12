using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace TestProject
{
    public class FaturaService
    {
        StringBuilder builder = new StringBuilder();
        string caminhoArquivo = ConfigurationManager.AppSettings["caminhoArquivo"];
        string caminhoArquivoSaida = ConfigurationManager.AppSettings["caminhoArquivoSaida"];
        CepHelpers cep = new CepHelpers(); int cont = 0;
        string lines;

        public List<Cliente> CepValidos()
        {
            try
            {
                if (File.Exists(caminhoArquivo))
                {
                    var cepValidos =
                    from c in
                        (from linha in File.ReadAllLines(caminhoArquivo)
                         let clienteDados = linha.Split(';')
                         select new Cliente()
                         {
                             NomeCliente = clienteDados[0].Trim(),
                             EnderecoCompleto = clienteDados[1].Trim() + " " + clienteDados[2].Trim() + " "
                                + clienteDados[3].Trim() + " " + clienteDados[4].Trim() + " " + clienteDados[5].Trim(),
                             CEP = clienteDados[1].Trim(),
                             ValorFatura = clienteDados[6].Trim(),
                             NumeroPaginas = clienteDados[7].Trim()
                         })
                    where cep.ValidaCep(c.CEP)
                    select c;
                    return cepValidos.ToList();
                }
                else
                    return new List<Cliente>();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void FaturasZeradas()
        {
            try
            {
                List<Cliente> listaClientes = CepValidos();

                if (listaClientes.Count > 0)
                {
                    var faturasZeradas = listaClientes.Where((Cliente c) => { return c.ValorFatura == "" || c.ValorFatura == "0"; });

                    foreach (var line in faturasZeradas)
                    {
                        lines = line.NomeCliente.ToString() + ";" + line.EnderecoCompleto.ToString() + ";" +
                            line.ValorFatura.ToString() + ";" + line.NumeroPaginas.ToString();
                        var temp = lines.Split(';');
                        builder.AppendLine(string.Join(";", temp));
                        cont++;
                    }

                    File.WriteAllText(caminhoArquivoSaida + "FaturasZeradas.csv", builder.ToString());
                    builder.Clear();

                    Console.WriteLine(".csv de faturas zeradas gerada com sucesso!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao gerar arquivo de faturas zeradas: " + ex.Message);
            }
        }

        public void Faturas()
        {
            try
            {
                if (File.Exists(caminhoArquivo))
                {
                    List<Cliente> listaClientes = CepValidos();
                    var sixPages = new List<Cliente>();
                    var twelvePages = new List<Cliente>();
                    var twelvePagesMore = new List<Cliente>();
                    var errorList = new List<Cliente>();

                    if (listaClientes.Count > 0)
                    {
                        foreach (var item in listaClientes)
                        {
                            try
                            {
                                if (Convert.ToInt32(item.NumeroPaginas) <= 6 && item.ValorFatura != "" && item.ValorFatura != "0")
                                {
                                    if (Convert.ToInt32(item.NumeroPaginas) % 2 == 0)
                                        sixPages.Add(item);
                                    else
                                    {
                                        item.NumeroPaginas = (Convert.ToInt32(item.NumeroPaginas) + 1).ToString();
                                        sixPages.Add(item);
                                    }
                                }
                                else if (Convert.ToInt32(item.NumeroPaginas) > 6 && Convert.ToInt32(item.NumeroPaginas) <= 12 && item.ValorFatura != "" && item.ValorFatura != "0")
                                {
                                    if (Convert.ToInt32(item.NumeroPaginas) % 2 == 0)
                                        twelvePages.Add(item);
                                    else
                                    {
                                        item.NumeroPaginas = (Convert.ToInt32(item.NumeroPaginas) + 1).ToString();
                                        twelvePages.Add(item);
                                    }
                                }
                                else if (Convert.ToInt32(item.NumeroPaginas) > 12 && item.ValorFatura != "" && item.ValorFatura != "0")
                                {
                                    if (Convert.ToInt32(item.NumeroPaginas) % 2 == 0)
                                        twelvePagesMore.Add(item);
                                    else
                                    {
                                        item.NumeroPaginas = (Convert.ToInt32(item.NumeroPaginas) + 1).ToString();
                                        twelvePagesMore.Add(item);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                errorList.Add(item);
                            }
                        }
                    }

                    foreach (var line in sixPages)
                    {
                        lines = line.NomeCliente.ToString() + ";" + line.EnderecoCompleto.ToString() + ";" +
                            line.ValorFatura.ToString() + ";" + line.NumeroPaginas.ToString();
                        var temp = lines.Split(';');

                        builder.AppendLine(string.Join(";", temp));
                        cont++;
                    }

                    File.WriteAllText(caminhoArquivoSaida + "Faturas6pag.csv", builder.ToString());
                    builder.Clear();

                    foreach (var line in twelvePages)
                    {
                        lines = line.NomeCliente.ToString() + ";" + line.EnderecoCompleto.ToString() + ";" +
                            line.ValorFatura.ToString() + ";" + line.NumeroPaginas.ToString();
                        var temp = lines.Split(';');
                        builder.AppendLine(string.Join(";", temp));
                        cont++;
                    }

                    File.WriteAllText(caminhoArquivoSaida + "Faturas7-12pag.csv", builder.ToString());
                    builder.Clear();

                    foreach (var line in twelvePagesMore)
                    {
                        lines = line.NomeCliente.ToString() + ";" + line.EnderecoCompleto.ToString() + ";" +
                            line.ValorFatura.ToString() + ";" + line.NumeroPaginas.ToString();
                        var temp = lines.Split(';');
                        builder.AppendLine(string.Join(";", temp));
                        cont++;
                    }

                    File.WriteAllText(caminhoArquivoSaida + "Faturas12pag.csv", builder.ToString());
                    builder.Clear();

                    //Gerando arquivo com as linhas que deram erro
                    foreach (var line in errorList)
                    {
                        lines = line.NomeCliente.ToString() + ";" + line.EnderecoCompleto.ToString() + ";" +
                            line.ValorFatura.ToString() + ";" + line.NumeroPaginas.ToString();
                        var temp = lines.Split(';');
                        builder.AppendLine(string.Join(";", temp));
                        cont++;
                    }
                    File.WriteAllText(caminhoArquivoSaida + "ErrorList.csv", builder.ToString());
                    builder.Clear();

                    Console.WriteLine("Arquivos gerados com sucesso, clique para sair!");
                }
                else
                {
                    Console.WriteLine("Caminho de arquivo de entrada está errado, por favor verifique o caminho no App.config!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao gerar arquivos: " + ex.Message);
            }
        }
    }
}
