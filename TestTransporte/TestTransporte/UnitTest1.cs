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

            // Validar que el boleto se gener� y el saldo se descont� correctamente
            Assert.IsNotNull(boleto, "El boleto deber�a generarse correctamente.");
            Assert.AreEqual(1210, tarjeta.saldo, "El saldo deber�a descontarse al 50% del costo del boleto (790).");
            tarjeta.cargar(3000);
            Assert.AreEqual(4210, tarjeta.saldo, "El saldo deber�a ser 4210 despu�s de cargar 3000 adicionales.");
        }

        [Test]
        public void TestBoletoGratuitoEstudiantil()
        {
            // Crear una tarjeta de boleto gratuito estudiantil
            var tarjeta = new BoletoGratuitoEstudiantil(2,0);

            // Intentar pagar un boleto
            var boleto = colectivo145_133.pagarCon(tarjeta);

            // Validar que el boleto se gener� y no se descont� saldo
            Assert.IsNotNull(boleto, "El boleto deber�a generarse correctamente.");
            Assert.AreEqual(0, tarjeta.saldo, "El saldo deber�a permanecer en 0 ya que el viaje es gratuito.");
            tarjeta.cargar(2000);
            var boleto2 = colectivo145_133.pagarCon(tarjeta);
            Assert.IsNotNull(boleto2, "El boleto deber�a generarse correctamente incluso despu�s de cargar saldo.");
            Assert.AreEqual(2000, tarjeta.saldo, "El saldo deber�a permanecer en 2000 ya que el viaje es gratuito.");
        }

        [Test]
        public void TestFranquiciaCompleta()
        {
            // Crear una tarjeta de franquicia completa
            var tarjeta = new FranquiciaCompleta(3,0);

            // Intentar pagar un boleto
            var boleto = colectivo132.pagarCon(tarjeta);

            // Validar que el boleto se gener� y no se descont� saldo
            Assert.IsNotNull(boleto, "El boleto deber�a generarse correctamente.");
            Assert.AreEqual(0, tarjeta.saldo, "El saldo deber�a permanecer en 0 ya que el viaje es gratuito.");
            var tarjeta2 = new FranquiciaCompleta(3, -1000);
            var boleto2 = colectivo132.pagarCon(tarjeta2);
            Assert.IsNotNull(boleto2, "El boleto deber�a generarse correctamente incluso con saldo negativo.");
            Assert.AreEqual(-1000, tarjeta2.saldo, "El saldo deber�a permanecer en -1000 ya que el viaje es gratuito.");
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
    }

}
