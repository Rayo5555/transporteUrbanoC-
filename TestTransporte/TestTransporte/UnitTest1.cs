using NUnit.Framework;
using System;
using System.Threading;
using TransporteUrbano;

namespace TestTransporte
{
    public class Tests
    {
        private TransporteUrbano.Colectivo colectivo132;
        private TransporteUrbano.Colectivo colectivo145_133;
        [SetUp]
        public void Setup()
        {

            colectivo145_133 = new TransporteUrbano.Colectivo("145/133");
            colectivo132 = new TransporteUrbano.Colectivo("132");

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
            var tarjeta = new BoletoGratuitoEstudiantil(1,2000);

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
            var tarjeta = new MedioBoletoEstudiantil(2, 800);

            // Primer viaje con medio boleto
            var boleto1 = colectivo132.pagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "El primer boleto debería generarse correctamente.");
            Assert.AreEqual(790, boleto1.costo, "El primer viaje debería cobrarse con el 50% del costo.");

            tarjeta.ultimaFechaUso = tarjeta.ultimaFechaUso.AddMinutes(-4);
            tarjeta.ultimaFechaUso = tarjeta.ultimaFechaUso.AddSeconds(-50);

            // Intentar un segundo viaje antes de 5 minutos
            var boleto2 = colectivo132.pagarCon(tarjeta);
            Assert.IsNull(boleto2, "No debería permitirse un segundo viaje antes de 5 minutos.");

        }

        [Test]
        public void TestMedioBoleto_NoMasDeDosViajesPorDia()
        {
            var tarjeta = new MedioBoletoEstudiantil(3, 5000);

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
    }

}


