using System;
using System.Text.RegularExpressions;

namespace HiperStream
{
    public class CepHelpers
    {
        public bool ValidaCep(string cep)
        {
            try
            {
                Regex Rgx = new Regex(@"^\d{8}$");

                return Rgx.IsMatch(cep);
            }

            catch (Exception)
            {
                return false;
            }
        }
    }
}
