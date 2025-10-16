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
            if (tarjeta.saldo < 380){
                Console.WriteLine("No hay saldo suficiente");
                return null;
            }
            else{
                tarjeta.saldo -= 1580;
                Console.WriteLine(tarjeta.saldo);
                if(tarjeta.saldo < 0)
                    Console.WriteLine("Ten en cuenta que tu saldo ahora está en negativo, cargue su tarjeta lo antes posible");
                boletosEntregados++;
                string code = linea + boletosEntregados;
                Boleto boleto = new Boleto(code, 1580);
                return boleto;
            }
        }
    }
}