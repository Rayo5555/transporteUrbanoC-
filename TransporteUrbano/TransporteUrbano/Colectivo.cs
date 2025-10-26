using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    
namespace TransporteUrbano
{   
    public class Colectivo
    {
        public string linea;
        private int boletosEntregados = 0;
        public Colectivo(string linea)
        {
            this.linea = linea;
        }

        public Boleto pagarCon(Tarjeta tarjeta)
        {
            int costoBoleto = 1580; // Costo base del boleto

            int resultadoPago = tarjeta.pagar(costoBoleto); //cheequeamos si se puede hacer el pago o no

            if (resultadoPago == 1) // Pago exitoso
            {
                boletosEntregados++;
                string codigoBoleto = linea + boletosEntregados;
                return new Boleto(codigoBoleto, costoBoleto);
            }
            else
            {
                Console.WriteLine("No se pudo realizar el pago. Verifique su saldo o tipo de tarjeta.");
                return null;
            }
        }
    }
}