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
            int saldoInicial = tarjeta.saldo;

            // Intentar realizar el pago usando la lógica de la tarjeta
            int resultadoPago = tarjeta.pagar(costoBoleto);

            if (resultadoPago == 1) // Pago exitoso
            {
                boletosEntregados++;
                string codigoBoleto = linea + boletosEntregados;
                int saldoRestante = tarjeta.saldo;
                int costoRealBoleto = saldoInicial - saldoRestante;

                return new Boleto(codigoBoleto, costoRealBoleto);
            }
            else
            {
                Console.WriteLine("No se pudo realizar el pago. Verifique su saldo o tipo de tarjeta.");
                return null;
            }
        }
    }
}