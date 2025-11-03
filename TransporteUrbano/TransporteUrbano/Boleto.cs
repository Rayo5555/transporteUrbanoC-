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
        public int costo, saldo, idTarjeta, totalAbonado;
        public string codigo, linea, tipo;
        public Boleto(string codigo, int saldo, int idTarjeta, string linea, string tipo, int totalAbonado, int costo = 1580)
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
        }
        public void showBoleto()
        {
            Console.WriteLine("----- BOLETO -----");
            Console.WriteLine("Fecha y hora: " + fecha);
            Console.WriteLine("Codigo del boleto: " + codigo);
            Console.WriteLine("Linea: " + linea);
            Console.WriteLine("Tipo de tarjeta: " + tipo);
            Console.WriteLine("ID de la tarjeta: " + idTarjeta);
            Console.WriteLine("Costo del boleto: " + costo);
            Console.WriteLine("Total abonado: " + totalAbonado);
            Console.WriteLine("Saldo restante en la tarjeta: " + saldo);
            Console.WriteLine("------------------");
        }
    }
}