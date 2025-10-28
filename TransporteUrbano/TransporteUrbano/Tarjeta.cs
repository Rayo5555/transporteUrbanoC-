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

        public virtual int pagar(int costo)
        {
            if (saldo < costo)
            {
                Console.WriteLine("No hay saldo suficiente");
                return 0;
            }
            saldo -= costo;
            acreditarCarga();
            return 1;
        }
    }

    // Franquicia parcial: Medio boleto estudiantil
    public class MedioBoletoEstudiantil : Tarjeta
    {
        public MedioBoletoEstudiantil(int id, int saldo = 0) : base(id, saldo) { }
        DateTime ultimoPago = new DateTime();
        public override int pagar(int costo)
        {
            if ((DateTime.Now - ultimoPago).TotalMinutes > 5)
            {
                ultimoPago = DateTime.Now;
                int costoReducido = costo / 2; // 50% de descuento
                if (saldo < costoReducido)
                {
                    Console.WriteLine("No hay saldo suficiente");
                    return 0;
                }
                saldo -= costoReducido;
                acreditarCarga();
                return 1;
            }
            else
            {
                if (saldo < costo)
                {
                    Console.WriteLine("No hay saldo suficiente");
                    return 0;
                }
                saldo -= costo;
                acreditarCarga();
                return 1;
            }
        }
    }

    // Franquicia completa: Boleto gratuito estudiantil
    public class BoletoGratuitoEstudiantil : Tarjeta
    {
        public BoletoGratuitoEstudiantil(int id, int saldo = 0) : base(id, saldo) { }
        public int usos = 2;
        public override int pagar(int costo)
        {
            if (usos > 0)
            {
                // Siempre permite el viaje sin costo
                Console.WriteLine("Viaje gratuito por estudiante.");
                return 1;
            }
            else
            {
                if (saldo < costo)
                {
                    Console.WriteLine("No hay saldo suficiente");
                    return 0;
                }
                saldo -= costo;
                acreditarCarga();
                return 1;
            }
        }
    }

    // Franquicia completa: Jubilados
    public class FranquiciaCompleta : Tarjeta
    {
        public FranquiciaCompleta(int id, int saldo = 0) : base(id, saldo) { }

        public override int pagar(int costo)
        {
            // Siempre permite el viaje sin costo
            Console.WriteLine("Viaje gratuito.");
            return 1;
        }
    }
}
