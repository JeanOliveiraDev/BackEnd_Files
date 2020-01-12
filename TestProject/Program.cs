using System;

namespace HiperStream
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                FaturaService faturaService = new FaturaService();

                Console.WriteLine("Gerando arquivos de saída...");

                faturaService.FaturasZeradas();
                faturaService.Faturas();

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }
}
