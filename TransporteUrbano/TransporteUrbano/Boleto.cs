using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransporteUrbano
{
    class Boleto
    {
        public int costo;
        public string codigo;
        public Boleto(int costo=1580, string codigo)
        {
            this.costo = costo;
            this.codigo = codigo;
        }
    }
}