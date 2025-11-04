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
        public int costo, saldo, idTarjeta, totalAbonado, transbordo;
        public string codigo, linea, tipo;
        public Boleto(string codigo, int saldo, int idTarjeta, string linea, string tipo, int totalAbonado, int costo = 1580, int transbordo = 0)
        {
            fecha = DateTime.Now;
            this.saldo = saldo;
            this.idTarjeta = idTarjeta;
            this.linea = linea;
            this.codigo = codigo;
            this.costo = costo;
            this.codigo = codigo;
            this.tipo = tipo;
            this.totalAbonado = totalAbonado;
            if (transbordo == 2)
            {
                this.transbordo = 1;
            }
            else
            {
                this.transbordo = 0;
            }
        }
    }

}

