using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransporteUrbano
{
    public class Boleto
    {
        public DateTime fecha;
        public int costo, saldo, id;
        public string codigo, linea;
        public Boleto(string codigo, int costo=1580)
        {
            this.costo = costo;
            this.codigo = codigo;
        }
    }
}