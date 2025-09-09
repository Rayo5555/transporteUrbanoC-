using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    
namespace TransporteUrbano
{   
    class Colectivo
    {
        public string linea;
        private int boletosEntregados = 0;
        public Colectivo(string linea)
        {
            this.linea = linea;
        }
    
        public Boleto pagarCon(Tarjeta tarjeta)
        {
            if (tarjeta.saldo < 1580){
                Console.WriteLine("No hay saldo suficiente");
                return null;
            }
            else{
                tarjeta.saldo -= 1580;
                boletosEntregados++;
                string code = linea + boletosEntregados;
                Boleto boleto = new Boleto(1580, code);
                return boleto
            }
        }
    }
}