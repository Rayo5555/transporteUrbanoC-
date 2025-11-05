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
        public int boletosEntregados = 0;
        public Colectivo(string linea)
        {
            this.linea = linea;
        }

        public virtual Boleto pagarCon(Tarjeta tarjeta)
        {
            int costoBoleto = 1580; // Costo base del boleto
            int saldoInicial = tarjeta.saldo;

            // Intentar realizar el pago usando la lógica de la tarjeta
            int resultadoPago = tarjeta.pagar(costoBoleto, linea);

            if (resultadoPago >= 1) // Pago exitoso
            {
                boletosEntregados++;
                string codigoBoleto = linea + boletosEntregados;
                int idTajeta = tarjeta.id;
                string lineaBoleto = linea;
                string tipoTarjeta = tarjeta.GetType().Name;
                int saldoRestante = tarjeta.saldo;
                int costoRealBoleto = saldoInicial - saldoRestante;
                int totalAbonado = saldoInicial >= costoRealBoleto ? costoRealBoleto : saldoInicial;

                return new Boleto(codigoBoleto, saldoRestante, idTajeta, lineaBoleto, tipoTarjeta, totalAbonado, costoRealBoleto, resultadoPago);
            }
            else
            {
                Console.WriteLine("No se pudo realizar el pago. Verifique su saldo o tipo de tarjeta.");
                return null;
            }
        }
    }
    public class ColectivoInterurbano : Colectivo
    {
        private const int TarifaInterurbana = 3000;

        public ColectivoInterurbano(string linea) : base(linea)
        {
        }

        public override Boleto pagarCon(Tarjeta tarjeta)
        {
            int costoBoleto = TarifaInterurbana; // Tarifa interurbana
            int saldoInicial = tarjeta.saldo;

            // Intentar realizar el pago usando la lógica de la tarjeta
            int resultadoPago = tarjeta.pagar(costoBoleto);

            if (resultadoPago == 1) // Pago exitoso
            {
                boletosEntregados++;
                string codigoBoleto = linea + boletosEntregados;
                int idTarjeta = tarjeta.id;
                string lineaBoleto = linea;
                string tipoTarjeta = tarjeta.GetType().Name;
                int saldoRestante = tarjeta.saldo;
                int costoRealBoleto = saldoInicial - saldoRestante;
                int totalAbonado = saldoInicial >= costoRealBoleto ? costoRealBoleto : saldoInicial;

                return new Boleto(codigoBoleto, saldoRestante, idTarjeta, lineaBoleto, tipoTarjeta, totalAbonado, costoRealBoleto);
            }
            else
            {
                Console.WriteLine("No se pudo realizar el pago. Verifique su saldo o tipo de tarjeta.");
                return null;
            }
        }
    }
}