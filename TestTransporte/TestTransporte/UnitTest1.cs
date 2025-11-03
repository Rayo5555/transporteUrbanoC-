using NUnit.Framework;
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

            // Validar que el boleto se gener� y no se descont� saldo
            Assert.IsNotNull(boleto, "El boleto deber�a generarse correctamente.");
            Assert.AreEqual(0, tarjeta.saldo, "El saldo deber�a permanecer en 0 ya que el viaje es gratuito.");
            tarjeta.cargar(2000);
            var boleto2 = colectivo145_133.pagarCon(tarjeta);
            Assert.IsNotNull(boleto2, "El boleto debería generarse correctamente incluso despu�s de cargar saldo.");
            Assert.AreEqual(2000, tarjeta.saldo, "El saldo debería permanecer en 2000 ya que el viaje es gratuito.");
        }

        [Test]
        public void TestFranquiciaCompleta()
        {
            // Crear una tarjeta de franquicia completa
            var tarjeta = new FranquiciaCompleta(3, 0);

            // Intentar pagar un boleto
            var boleto = colectivo132.pagarCon(tarjeta);

            // Validar que el boleto se gener� y no se descont� saldo
            Assert.IsNotNull(boleto, "El boleto deber�a generarse correctamente.");
            Assert.AreEqual(0, tarjeta.saldo, "El saldo debería permanecer en 0 ya que el viaje es gratuito.");
            var tarjeta2 = new FranquiciaCompleta(3, -1000);
            var boleto2 = colectivo132.pagarCon(tarjeta2);
            Assert.IsNotNull(boleto2, "El boleto debeía generarse correctamente incluso con saldo negativo.");
            Assert.AreEqual(-1000, tarjeta2.saldo, "El saldo debería permanecer en -1000 ya que el viaje es gratuito.");
        }

        [Test]
        public void acreditarSaldo()
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
    }

}
