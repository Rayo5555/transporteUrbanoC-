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

            // Intentar realizar el pago usando la lógica de la tarjeta
            int resultadoPago = tarjeta.pagar(costoBoleto);
            if (tarjeta.saldo < 0)
            {
                Console.WriteLine("Saldo negativo, viaje plus utilizado.");
            }
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