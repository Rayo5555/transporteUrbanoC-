using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransporteUrbano
{
    public class Tarjeta
    {
        public int saldo, id, pendienteDeAcreditar;
        public DateTime ultimoUso;
        public String ultimaLinea;

        public Tarjeta(int id, int saldo = 0)
        {
            if (saldo > 56000)
            {
                pendienteDeAcreditar = saldo - 56000;
                this.saldo = 56000;
            }
            else
            {
                this.saldo = saldo;
                pendienteDeAcreditar = 0;
            }
            this.id = id;
            ultimoUso = new DateTime(1970, 1, 1);
            ultimaLinea = "";
        }
        public void acreditarCarga()
        {
            if ((saldo + pendienteDeAcreditar) > 56000)
            {
                pendienteDeAcreditar = (saldo + pendienteDeAcreditar) - 56000;
                saldo = 56000;
            }
            else
            {
                saldo = saldo + pendienteDeAcreditar;
                pendienteDeAcreditar = 0;
            }
        }
        public int cargar(int money)
        {
            if (money == 2000 || money == 3000 || money == 4000 || money == 5000 || money == 8000 || money == 10000 || money == 15000 || money == 20000 || money == 25000 || money == 30000)
            {
                pendienteDeAcreditar += money;
                acreditarCarga();
                return 1;
            }
            else
            {
                Console.WriteLine("No se ha introducido una cantidad valida, por favor intente nuevamente");
                return 0;
            }
        }
        public int trasbordos(String lineaTomada)
        {
            if(ultimaLinea == lineaTomada || (DateTime.Now - ultimoUso).TotalMinutes > 60 || DateTime.Now.DayOfWeek.ToString() == "Sunday" || DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || ultimaLinea == "")
            {
                ultimoUso = DateTime.Now;
                return 0;
            }else
            {
                ultimaLinea = lineaTomada;
                ultimoUso = DateTime.Now;
                return 1;
            }
        }
        public virtual int pagar(int costo, String lineaTomada)
        {
            if (trasbordos(lineaTomada) == 0)
            {
                if ((saldo + 1200) < costo)
                {
                    Console.WriteLine("No hay saldo suficiente");
                    return 0;
                }
                saldo -= costo;
                acreditarCarga();
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }

    // Franquicia parcial: Medio boleto estudiantil
    public class MedioBoletoEstudiantil : Tarjeta
    {
        private int usos;
        public DateTime ultimaFechaUso = new DateTime(1970, 1, 1);
        public MedioBoletoEstudiantil(int id, int saldo = 0, int usos = 0) : base(id, saldo) {
            this.usos = usos; //campo de usos para controlar los viajes con medio boleto
            ultimoUso = new DateTime(1970, 1, 1);
            ultimaLinea = "";
        }
        public override int pagar(int costo, String lineaTomada)
        {

            if ((DateTime.Now - ultimaFechaUso).TotalDays >= 1)
            {
                usos = 0; // Reiniciar usos si ha pasado un día
            }
            if (trasbordos(lineaTomada) == 1)
            {
                return 2;
            }else
            {
                if (usos < 2)
                {
                    if ((DateTime.Now - ultimaFechaUso).TotalMinutes > 5)
                    {
                        ultimaFechaUso = DateTime.Now;
                        int costoReducido = costo / 2; // 50% de descuento
                        if ((saldo + 1200) < costoReducido)
                        {
                            Console.WriteLine("No hay saldo suficiente");
                            return 0;
                        }
                        saldo -= costoReducido;
                        usos += 1;
                        acreditarCarga();
                        return 1;
                    }
                    else
                    {
                        Console.WriteLine("No pasaron 5 minutos desde su último uso, prueba más tarde");
                        return 0;
                    }
                }
                else
                {
                    if ((saldo + 1200) < costo)
                    {
                        Console.WriteLine("No hay saldo suficiente");
                        return 0;
                    }
                    saldo -= costo;
                    acreditarCarga();
                    Console.WriteLine("Viaje normal, se han agotado los medios boletos disponibles." + usos);
                    return 1;
                }
            }
        }
    }

    // Franquicia completa: Boleto gratuito estudiantil
    public class BoletoGratuitoEstudiantil : Tarjeta
    {
        private int usos;
        public DateTime ultimaFechaUso = new DateTime(1970, 1, 1);
        public BoletoGratuitoEstudiantil(int id, int saldo = 0, int usos = 0) : base(id, saldo)
        {
            this.usos = usos;
            ultimoUso = new DateTime(1970, 1, 1);
            ultimaLinea = "";
        }

        public override int pagar(int costo, String lineaTomada)
        {
            // Verificar si ha pasado un día desde el último uso
            if ((DateTime.Now - ultimoUso).TotalDays >= 1)
            {
                usos = 0; // Reiniciar usos si ha pasado un día
            }
            if (trasbordos(lineaTomada) == 1)
            {
                return 2;
            }else
            {
                if (usos < 2)
                {
                    // Solo 2 viajes gratuitos permitidos
                    Console.WriteLine("Viaje gratuito por estudiante.");
                    usos += 1;
                    ultimaFechaUso = DateTime.Now.Date; // Actualizar la última fecha de uso
                    return 1;
                }
                else
                {
                    if ((saldo + 1200) < costo)
                    {
                        Console.WriteLine("No hay saldo suficiente");
                        return 0;
                    }
                    saldo -= costo;
                    Console.WriteLine("Viaje normal, se han agotado los viajes gratuitos disponibles.");
                    acreditarCarga();
                    return 1;
                }
            }
        }
    }

    // Franquicia completa: Jubilados
    public class FranquiciaCompleta : Tarjeta
    {
        public FranquiciaCompleta(int id, int saldo = 0) : base(id, saldo) {
            ultimoUso = new DateTime(1970, 1, 1);
            ultimaLinea = "";
        }

        public override int pagar(int costo, String lineaTomada)
        {
            if (trasbordos(lineaTomada) == 0)
            {
                // Siempre permite el viaje sin costo
                Console.WriteLine("Viaje gratuito.");
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }
}
