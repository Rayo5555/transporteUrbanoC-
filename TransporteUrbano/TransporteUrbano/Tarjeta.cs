using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransporteUrbano
{
    public class Tarjeta
    {
        public int saldo, id;

        public Tarjeta(int id, int saldo = 0)
        {
            this.saldo = saldo;
            this.id = id;
        }

        public int cargar(int money)
        {
            if (money == 2000 || money == 3000 || money == 4000 || money == 5000 || money == 8000 || money == 10000 || money == 15000 || money == 20000 || money == 25000 || money == 30000)
            {
                if ((saldo + money) >= 40000)
                {
                    Console.WriteLine("Supera el limite permitido de saldo");
                    return 2;
                }
                else
                {
                    saldo += money;
                    Console.WriteLine("Se ha cargado saldo correctamente, ahora tiene ", saldo);
                    return 1;
                }
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
            return 1;
        }
    }

    // Franquicia parcial: Medio boleto estudiantil
    public class MedioBoletoEstudiantil : Tarjeta
    {
        public MedioBoletoEstudiantil(int id, int saldo = 0) : base(id, saldo) { }

        public override int pagar(int costo)
        {
            int costoReducido = costo / 2; // 50% de descuento
            if (saldo < costoReducido)
            {
                Console.WriteLine("No hay saldo suficiente");
                return 0;
            }
            saldo -= costoReducido;
            return 1;
        }
    }

    // Franquicia completa: Boleto gratuito estudiantil
    public class BoletoGratuitoEstudiantil : Tarjeta
    {
        public BoletoGratuitoEstudiantil(int id, int saldo=0) : base(id, saldo) { }

        public override int pagar(int costo)
        {
            // Siempre permite el viaje sin costo
            Console.WriteLine("Viaje gratuito por estudiante.");
            return 1;
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
