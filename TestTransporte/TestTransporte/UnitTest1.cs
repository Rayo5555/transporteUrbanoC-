using NUnit.Framework;
using System;
using System.Threading;
using TransporteUrbano;

namespace TestTransporte
{

    public static class Reloj
    {
        public static Func<DateTime> Ahora = () => DateTime.Now;
    }

    public class Tests
    {
        private TransporteUrbano.Colectivo colectivo132;
        private TransporteUrbano.Colectivo colectivo145_133;
        [SetUp]
        public void Setup()
        {

            colectivo145_133 = new TransporteUrbano.Colectivo("145/133");
            colectivo132 = new TransporteUrbano.Colectivo("132");

            Tarjeta.Reloj = () => new DateTime(2025, 11, 4, 10, 0, 0);

        }

        [Test]
        public void TestMedioBoletoEstudiantil()
        {
            // Crear una tarjeta de medio boleto estudiantil con saldo suficiente
            var tarjeta = new MedioBoletoEstudiantil(1, 2000);

            // Intentar pagar un boleto
            var boleto = colectivo132.pagarCon(tarjeta);

            // Validar que el boleto se generó y el saldo se descontó correctamente
            Assert.IsNotNull(boleto, "El boleto debería generarse correctamente.");
            Assert.AreEqual(1210, tarjeta.saldo, "El saldo debería descontarse al 50% del costo del boleto (790).");
            tarjeta.cargar(3000);
            Assert.AreEqual(4210, tarjeta.saldo, "El saldo debería ser 4210 después de cargar 3000 adicionales.");
        }

        [Test]
        public void TestBoletoGratuitoEstudiantil()
        {
            // Crear una tarjeta de boleto gratuito estudiantil
            var tarjeta = new BoletoGratuitoEstudiantil(2, 0);
            // Intentar pagar un boleto
            var boleto = colectivo145_133.pagarCon(tarjeta);

            // Validar que el boleto se generó y no se descontó saldo
            Assert.IsNotNull(boleto, "El boleto debería generarse correctamente.");
            Assert.AreEqual(0, tarjeta.saldo, "El saldo debería permanecer en 0 ya que el viaje es gratuito.");
            tarjeta.cargar(2000);
            var boleto2 = colectivo145_133.pagarCon(tarjeta);
            Assert.IsNotNull(boleto2, "El boleto debería generarse correctamente incluso después de cargar saldo.");
            Assert.AreEqual(2000, tarjeta.saldo, "El saldo debería permanecer en 2000 ya que el viaje es gratuito.");
        }

        [Test]
        public void TestFranquiciaCompleta()
        {
            // Crear una tarjeta de franquicia completa
            var tarjeta = new FranquiciaCompleta(3, 0);

            // Intentar pagar un boleto
            var boleto = colectivo132.pagarCon(tarjeta);

            // Validar que el boleto se generó y no se descontó saldo
            Assert.IsNotNull(boleto, "El boleto debería generarse correctamente.");
            Assert.AreEqual(0, tarjeta.saldo, "El saldo debería permanecer en 0 ya que el viaje es gratuito.");
            var tarjeta2 = new FranquiciaCompleta(3, -1000);
            var boleto2 = colectivo132.pagarCon(tarjeta2);
            Assert.IsNotNull(boleto2, "El boleto debeía generarse correctamente incluso con saldo negativo.");
            Assert.AreEqual(-1000, tarjeta2.saldo, "El saldo debería permanecer en -1000 ya que el viaje es gratuito.");
        }

        [Test]
        public void TestCargaInvalida()
        {
            var tarjeta = new Tarjeta(5, 1000);
            int resultado = tarjeta.cargar(2500); // Cantidad no válida
            Assert.AreEqual(0, resultado, "Debería retornar 0 al intentar cargar una cantidad inválida.");
            Assert.AreEqual(1000, tarjeta.saldo, "El saldo no debería cambiar al intentar cargar una cantidad inválida.");
        }
        [Test]
        public void TestViajeLímite()
        {
            var tarjeta = new Tarjeta(6, 380); // Saldo JUSTO
            var boleto = colectivo145_133.pagarCon(tarjeta);
            Assert.IsNotNull(boleto, "El boleto debería generarse con saldo justo.");
            Assert.AreEqual(-1200, tarjeta.saldo, "El saldo debería ser el mínimo negativo permitido");
        }
        [Test]
        public void AcreditarSaldo()
        {
            var tarjeta = new Tarjeta(4, 58000);
            Assert.AreEqual(56000, tarjeta.saldo, "El saldo debería ser 56000 después de la acreditación inicial.");
            Assert.AreEqual(2000, tarjeta.pendienteDeAcreditar, "El pendiente de acreditar debería ser 2000 después de la acreditación inicial.");
            var boleto = colectivo145_133.pagarCon(tarjeta);
            Assert.AreEqual(56000, tarjeta.saldo, "El saldo debería mantenerse en 56000");
            Assert.AreEqual(420, tarjeta.pendienteDeAcreditar, "Debería consumir 1580 del pendiente");
            var boleto2 = colectivo145_133.pagarCon(tarjeta);
            Assert.AreEqual(54840, tarjeta.saldo, "El saldo debería ser 54840 después de pagar otro boleto.");
            Assert.AreEqual(0, tarjeta.pendienteDeAcreditar, "El pendiente de acreditar debería ser 0 después de pagar otro boleto.");
            tarjeta.cargar(3000);
            Assert.AreEqual(56000, tarjeta.saldo, "El saldo debería ser 56000 después de la acreditación.");
            Assert.AreEqual(1840, tarjeta.pendienteDeAcreditar, "El pendiente de acreditar debería ser 1840 después de la acreditación.");
        }
        [Test]
        public void AcreditarSaldo2()
        {

            Tarjeta.Reloj = () => new DateTime(2025, 11, 4, 10, 0, 0);
            var tarjeta = new MedioBoletoEstudiantil(4, 57000);

            Assert.AreEqual(56000, tarjeta.saldo, "El saldo debería ser 56000 después de la acreditación inicial.");
            Assert.AreEqual(1000, tarjeta.pendienteDeAcreditar, "El pendiente de acreditar debería ser 2000 después de la acreditación inicial.");
            var boleto = colectivo145_133.pagarCon(tarjeta);
            tarjeta.ultimaFechaUso = tarjeta.ultimaFechaUso.AddMinutes(-5);
            Assert.AreEqual(56000, tarjeta.saldo, "El saldo debería mantenerse en 56000");
            Assert.AreEqual(210, tarjeta.pendienteDeAcreditar, "Debería consumir 790 del pendiente");
            var boleto2 = colectivo145_133.pagarCon(tarjeta);

            Assert.AreEqual(55420, tarjeta.saldo, "El saldo debería ser 55420 después de pagar otro boleto.");
            Assert.AreEqual(0, tarjeta.pendienteDeAcreditar, "El pendiente de acreditar debería ser 0 después de pagar otro boleto.");
            tarjeta.cargar(3000);
            Assert.AreEqual(56000, tarjeta.saldo, "El saldo debería ser 56000 después de la acreditación.");
            Assert.AreEqual(2420, tarjeta.pendienteDeAcreditar, "El pendiente de acreditar debería ser 2420 después de la acreditación.");
        }
        [Test]
        public void InfoBoleto()
        {
            var tarjeta = new Tarjeta(7, 2000);
            var boleto = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto, "El boleto debería generarse correctamente.");
            Assert.AreEqual("1321", boleto.codigo, "El código del boleto debería ser '1321'.");
            Assert.AreEqual("132", boleto.linea, "La línea del boleto debería ser '132'.");
            Assert.AreEqual("Tarjeta", boleto.tipo, "El tipo de tarjeta debería ser 'Tarjeta'.");
            Assert.AreEqual(7, boleto.idTarjeta, "El ID de la tarjeta debería ser 7.");
            Assert.AreEqual(1580, boleto.costo, "El costo del boleto debería ser 1580.");
            Assert.AreEqual(1580, boleto.totalAbonado, "El total abonado debería ser 1580.");
            Assert.AreEqual(420, boleto.saldo, "El saldo restante en la tarjeta debería ser 420.");
            var boleto2 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto2, "El boleto debería generarse correctamente.");
            Assert.AreEqual("1322", boleto2.codigo, "El código del boleto debería ser '1322'.");
            Assert.AreEqual("132", boleto2.linea, "La línea del boleto debería ser '132'.");
            Assert.AreEqual("Tarjeta", boleto2.tipo, "El tipo de tarjeta debería ser 'Tarjeta'.");
            Assert.AreEqual(7, boleto.idTarjeta, "El ID de la tarjeta debería ser 7.");
            Assert.AreEqual(1580, boleto2.costo, "El costo del boleto debería ser 1580.");
            Assert.AreEqual(420, boleto2.totalAbonado, "El total abonado debería ser 420.");
            Assert.AreEqual(-1160, boleto2.saldo, "El saldo restante en la tarjeta debería ser -1160.");
        }
        [Test]
        public void TestBoletoGratuito_NoMasDeDosViajesGratuitosPorDia()
        {
            var tarjeta = new BoletoGratuitoEstudiantil(1, 2000);

            // Primer viaje gratuito
            var boleto1 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "El primer boleto debería generarse correctamente.");
            Assert.AreEqual(0, boleto1.costo, "El primer viaje debería ser gratuito.");

            // Se intenta el segundo viaje gratuito
            var boleto3 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "El seegundo boleto debería generarse correctamente.");
            Assert.AreEqual(0, boleto1.costo, "El segundo viaje debería ser gratuito.");

            // Tercer viaje (debería cobrarse con el precio completo)
            var boleto4 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto4, "El tercer boleto debería generarse correctamente.");
            Assert.AreEqual(1580, boleto4.costo, "El tercer viaje debería cobrarse con el precio completo.");
        }
        [Test]
        public void RecargaUsosGratuitol()
        {
            var tarjeta = new BoletoGratuitoEstudiantil(4, 2000);
            // Realizar dos viajes con medio boleto
            var boleto1 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "El primer boleto debería generarse correctamente.");
            Assert.AreEqual(0, boleto1.costo, "El primer viaje debería cobrarse gratis.");

            var boleto2 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto2, "El segundo boleto debería generarse correctamente.");
            Assert.AreEqual(0, boleto2.costo, "El segundo viaje debería cobrarse gratis.");

            tarjeta.ultimaFechaUso = tarjeta.ultimaFechaUso.AddDays(-1); // Simular que pasó un día

            // Ahora los usos deberían haberse reiniciado
            var boleto3 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto3, "El tercer boleto debería generarse correctamente después de un día.");
            Assert.AreEqual(0, boleto3.costo, "El tercer viaje debería cobrarse gratis.");
        }

        [Test]
        public void TestMedioBoleto_NoViajeEnMenosDeCincoMinutos()
        {
            var tarjeta = new MedioBoletoEstudiantil(2, 2000);

            // Primer viaje con medio boleto
            var boleto1 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "El primer boleto debería generarse correctamente.");
            Assert.AreEqual(790, boleto1.costo, "El primer viaje debería cobrarse con el 50% del costo.");

            tarjeta.ultimaFechaUso = tarjeta.ultimaFechaUso.AddMinutes(-4);
            tarjeta.ultimaFechaUso = tarjeta.ultimaFechaUso.AddSeconds(-59);

            // Intentar un segundo viaje antes de 5 minutos
            var boleto2 = colectivo132.pagarCon(tarjeta);
            Assert.IsNull(boleto2, "No debería permitirse un segundo viaje antes de 5 minutos.");

        }

        [Test]
        public void TestMedioBoleto_NoMasDeDosViajesPorDia()
        {
            var tarjeta = new MedioBoletoEstudiantil(3, 2000);

            // Primer viaje con medio boleto
            var boleto1 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "El primer boleto debería generarse correctamente.");
            Assert.AreEqual(790, boleto1.costo, "El primer viaje debería cobrarse con el 50% del costo.");

            // Esperar 5 minutos para el siguiente viaje
            tarjeta.ultimaFechaUso = tarjeta.ultimaFechaUso.AddMinutes(-5); // Simular que pasaron 5 minutos

            // Segundo viaje con medio boleto
            var boleto2 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto2, "El segundo boleto debería generarse correctamente.");
            Assert.AreEqual(790, boleto2.costo, "El segundo viaje debería cobrarse con el 50% del costo.");

            // Tercer viaje (debería cobrarse con el precio completo)
            var boleto3 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto3, "El tercer boleto debería generarse correctamente.");
            Assert.AreEqual(1580, boleto3.costo, "El tercer viaje debería cobrarse con el precio completo.");
        }
        [Test]
        public void RecargaUsosMedioBoletoEstudiantil()
        {
            var tarjeta = new MedioBoletoEstudiantil(4, 2000);
            // Realizar dos viajes con medio boleto
            var boleto1 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "El primer boleto debería generarse correctamente.");
            Assert.AreEqual(790, boleto1.costo, "El primer viaje debería cobrarse con el 50% del costo.");

            tarjeta.ultimaFechaUso = tarjeta.ultimaFechaUso.AddMinutes(-5); // Simular que pasaron 5 minutos

            var boleto2 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto2, "El segundo boleto debería generarse correctamente.");
            Assert.AreEqual(790, boleto2.costo, "El segundo viaje debería cobrarse con el 50% del costo.");

            tarjeta.ultimaFechaUso = tarjeta.ultimaFechaUso.AddDays(-1); // Simular que pasó un día

            // Ahora los usos deberían haberse reiniciado
            var boleto3 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto3, "El tercer boleto debería generarse correctamente después de un día.");
            Assert.AreEqual(790, boleto3.costo, "El tercer viaje debería cobrarse con el 50% del costo.");
        }
        [Test]
        public void TestColectivoInterurbano()
        {
            var colectivoInterurbanoA = new ColectivoInterurbano("ACostanera");
            var tarjeta = new Tarjeta(8, 5000);
            var tarjeta2 = new MedioBoletoEstudiantil(9, 5000);
            var tarjeta3 = new BoletoGratuitoEstudiantil(10, 5000);
            var tarjeta4 = new FranquiciaCompleta(11, 5000);
            var boleto = colectivoInterurbanoA.pagarCon(tarjeta);
            Assert.IsNotNull(boleto, "El boleto debería generarse correctamente para el colectivo interurbano.");
            Assert.AreEqual(3000, boleto.costo, "El costo del boleto interurbano debería ser 3000.");
            Assert.AreEqual(2000, tarjeta.saldo, "El saldo debería descontarse correctamente después de pagar el boleto interurbano.");
            var boleto2 = colectivoInterurbanoA.pagarCon(tarjeta2);
            Assert.IsNotNull(boleto2, "El boleto debería generarse correctamente para el colectivo interurbano con medio boleto.");
            Assert.AreEqual(1500, boleto2.costo, "El costo del boleto interurbano con medio boleto debería ser 1500.");
            Assert.AreEqual(3500, tarjeta2.saldo, "El saldo debería descontarse correctamente después de pagar el boleto interurbano con medio boleto.");
            var boleto3 = colectivoInterurbanoA.pagarCon(tarjeta3);
            Assert.IsNotNull(boleto3, "El boleto debería generarse correctamente para el colectivo interurbano con boleto gratuito.");
            Assert.AreEqual(0, boleto3.costo, "El costo del boleto interurbano con boleto gratuito debería ser 0.");
            var boleto4 = colectivoInterurbanoA.pagarCon(tarjeta4);
            Assert.IsNotNull(boleto4, "El boleto debería generarse correctamente para el colectivo interurbano con franquicia completa.");
            Assert.AreEqual(0, boleto4.costo, "El costo del boleto interurbano con franquicia completa debería ser 0.");
            tarjeta.saldo = 1000; // Ajustar saldo para probar falta de fondos
            var boleto5 = colectivoInterurbanoA.pagarCon(tarjeta);
            Assert.IsNull(boleto5, "El boleto NO debería generarse por falta de saldo.");
            Assert.AreEqual(1000, tarjeta.saldo, "El saldo debería permanecer igual después de un intento fallido de pago.");

            Assert.AreEqual("ACostanera1", boleto.codigo, "El código del boleto debería ser '1321'.");
            Assert.AreEqual("ACostanera", boleto.linea, "La línea del boleto debería ser '132'.");
            Assert.AreEqual("Tarjeta", boleto.tipo, "El tipo de tarjeta debería ser 'Tarjeta'.");
            Assert.AreEqual(8, boleto.idTarjeta, "El ID de la tarjeta debería ser 7.");
            Assert.AreEqual(3000, boleto.costo, "El costo del boleto debería ser 1580.");
            Assert.AreEqual(3000, boleto.totalAbonado, "El total abonado debería ser 1580.");
            Assert.AreEqual(2000, boleto.saldo, "El saldo restante en la tarjeta debería ser 420.");
        }
        [Test]
        public void TarjetaFrecuenteDescuentos()
        {
            var tarjeta = new Tarjeta(5, 20000);
            // Viaje número 29 (precio normal)
            tarjeta.usoFrecuente = 28;
            var boleto1 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "El boleto debería generarse correctamente para el viaje 29.");
            Assert.AreEqual(1580, boleto1.costo, "El costo del boleto debería ser 1580 para el viaje 29.");
            Assert.AreEqual(18420, tarjeta.saldo, "El saldo debería descontarse correctamente para el viaje 29.");
            // Viaje número 30 (descuento 20% aplicado)
            var boleto2 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto2, "El boleto debería generarse correctamente para el viaje 30.");
            Assert.AreEqual(1264, boleto2.costo, "El costo del boleto debería ser 1264 para el viaje 30 con descuento.");
            Assert.AreEqual(17156, tarjeta.saldo, "El saldo debería descontarse correctamente para el viaje 30 con descuento.");
            tarjeta.usoFrecuente = 58;
            // Viaje número 59 (descuento 20% aplicado)
            var boleto3 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto3, "El boleto debería generarse correctamente para el viaje 59.");
            Assert.AreEqual(1264, boleto3.costo, "El costo del boleto debería ser 1264 para el viaje 59 con descuento.");
            Assert.AreEqual(15892, tarjeta.saldo, "El saldo debería descontarse correctamente para el viaje 59 con descuento.");
            // Viaje número 60 (descuento 25% aplicado)
            var boleto4 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto4, "El boleto debería generarse correctamente para el viaje 60.");
            Assert.AreEqual(1185, boleto4.costo, "El costo del boleto debería ser 1185 para el viaje 60 con descuento.");
            Assert.AreEqual(14707, tarjeta.saldo, "El saldo debería descontarse correctamente para el viaje 60 con descuento.");
            tarjeta.usoFrecuente = 79;
            // Viaje número 80 (descuento 25% aplicado)
            var boleto5 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto5, "El boleto debería generarse correctamente para el viaje 79.");
            Assert.AreEqual(1185, boleto5.costo, "El costo del boleto debería ser 1185 para el viaje 79 con descuento.");
            Assert.AreEqual(13522, tarjeta.saldo, "El saldo debería descontarse correctamente para el viaje 79 con descuento.");
            // Viaje número 81 (valor normal nuevamente)
            var boleto6 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto6, "El boleto debería generarse correctamente para el viaje 80.");
            Assert.AreEqual(1580, boleto6.costo, "El costo del boleto debería ser 1580 para el viaje 80.");
            Assert.AreEqual(11942, tarjeta.saldo, "El saldo debería descontarse correctamente para el viaje 80.");


        }
        [Test]
        public void TarjetaFrecuenteReset()
        {
            var tarjeta = new Tarjeta(6, 5000);
            tarjeta.usoFrecuente = 50;
            var boleto1 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "El boleto debería generarse correctamente.");
            Assert.AreEqual(1264, boleto1.costo, "El costo del boleto debería ser 1264 con descuento.");
            Assert.AreEqual(3736, tarjeta.saldo, "El saldo debería descontarse correctamente.");
            // Simular que pasó un mes
            tarjeta.primerViajeMes = tarjeta.primerViajeMes.AddMonths(-1);
            var boleto2 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto2, "El boleto debería generarse correctamente.");
            Assert.AreEqual(1580, boleto2.costo, "El costo del boleto debería ser 1580 sin descuento después de un mes.");
            Assert.AreEqual(2156, tarjeta.saldo, "El saldo debería descontarse correctamente sin descuento después de un mes.");

        }
        [Test]
        public void TarjetaFrecuenteSinSaldo()
        {
            var tarjeta = new Tarjeta(7, 0);
            tarjeta.usoFrecuente = 29;
            var boleto1 = colectivo132.pagarCon(tarjeta);
            Assert.IsNull(boleto1, "El boleto no debería generarse por saldo insuficiente.");
            Assert.AreEqual(0, tarjeta.saldo, "El saldo debería permanecer igual por saldo insuficiente.");
            tarjeta.usoFrecuente = 59;
            var boleto2 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto2, "El boleto debería generarse correctamente.");
            Assert.AreEqual(1185, boleto2.costo, "El costo del boleto debería ser 1185 con descuento.");
            Assert.AreEqual(-1185, tarjeta.saldo, "El saldo debería descontarse correctamente.");
            var boleto3 = colectivo132.pagarCon(tarjeta);
            Assert.IsNull(boleto3, "El boleto no debería generarse por saldo insuficiente.");
            tarjeta.usoFrecuente = 80;
            var boleto4 = colectivo132.pagarCon(tarjeta);
            Assert.IsNull(boleto4, "El boleto no debería generarse por saldo insuficiente.");
        }
        [Test]
        public void PruebaTrasbordos()
        {
            var tarjeta = new Tarjeta(4, 5000);

            // Configurar el reloj simulado
            Tarjeta.Reloj = () => new DateTime(2025, 11, 4, 10, 0, 0);

            // Primer viaje
            var boleto1 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "El primer boleto debería generarse correctamente.");
            Assert.AreEqual(1580, boleto1.costo, "El primer viaje debería cobrarse con el 100% del costo.");

            // Simular que pasaron 15 minutos
            Tarjeta.Reloj = () => new DateTime(2025, 11, 4, 10, 15, 0);

            // Segundo viaje (trasbordo)
            var boleto2 = colectivo145_133.pagarCon(tarjeta);
            Assert.IsNotNull(boleto2, "El segundo boleto debería generarse correctamente.");
            Assert.AreEqual(0, boleto2.costo, "El segundo viaje no debería cobrarse.");
            Assert.AreEqual(1, boleto2.trasbordo, "El boleto debería marcar que es un trasbordo.");
        }

        [Test]
        public void PruebaTrasbordosBoletoGratuitoEstudiantil()
        {
            var tarjeta = new BoletoGratuitoEstudiantil(4, 5000);

            // Configurar el reloj simulado
            Tarjeta.Reloj = () => new DateTime(2025, 11, 4, 10, 0, 0);

            // Primer viaje
            var boleto1 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "El primer boleto debería generarse correctamente.");
            Assert.AreEqual(0, boleto1.costo, "El primer viaje debería cobrarse con el 100% del costo.");

            // Simular que pasaron 15 minutos
            Tarjeta.Reloj = () => new DateTime(2025, 11, 4, 10, 15, 0);

            // Segundo viaje (trasbordo)
            var boleto2 = colectivo145_133.pagarCon(tarjeta);
            Assert.IsNotNull(boleto2, "El segundo boleto debería generarse correctamente.");
            Assert.AreEqual(0, boleto2.costo, "El segundo viaje no debería cobrarse.");
            Assert.AreEqual(1, boleto2.trasbordo, "El boleto debería marcar que es un trasbordo.");
        }

        [Test]
        public void PruebaTrasbordosMedioBoletoGratuito()
        {
            var tarjeta = new MedioBoletoEstudiantil(4, 5000);

            // Configurar el reloj simulado
            Tarjeta.Reloj = () => new DateTime(2025, 11, 4, 10, 0, 0);

            // Primer viaje
            var boleto1 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "El primer boleto debería generarse correctamente.");
            Assert.AreEqual(790, boleto1.costo, "El primer viaje debería cobrarse con el 100% del costo.");

            // Simular que pasaron 15 minutos
            Tarjeta.Reloj = () => new DateTime(2025, 11, 4, 10, 15, 0);

            // Segundo viaje (trasbordo)
            var boleto2 = colectivo145_133.pagarCon(tarjeta);
            Assert.IsNotNull(boleto2, "El segundo boleto debería generarse correctamente.");
            Assert.AreEqual(0, boleto2.costo, "El segundo viaje no debería cobrarse.");
            Assert.AreEqual(1, boleto2.trasbordo, "El boleto debería marcar que es un trasbordo.");
        }

        [Test]
        public void SaldoInsuficienteMedioBoleto()
        {
            var tarjeta = new Tarjeta(4, -1000);


            // Viaje
            var boleto1 = colectivo132.pagarCon(tarjeta);
            Assert.IsNull(boleto1, "El primer boleto debería generarse correctamente.");
        }

        [Test]
        public void SaldoInsuficienteMedioBoletoFueraDeHorario()
        {
            var tarjeta = new Tarjeta(4, -1000);

            Tarjeta.Reloj = () => new DateTime(2025, 11, 4, 23, 15, 0);

            // Viaje
            var boleto1 = colectivo132.pagarCon(tarjeta);
            Assert.IsNull(boleto1, "El primer boleto debería generarse correctamente.");
        }
    }
}

