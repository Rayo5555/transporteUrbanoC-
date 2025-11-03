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
            Assert.IsNotNull(boleto, "El boleto debería generarse correctamente.");
            Assert.AreEqual(1210, tarjeta.saldo, "El saldo debería descontarse al 50% del costo del boleto (790).");
            tarjeta.cargar(3000);
            Assert.AreEqual(4210, tarjeta.saldo, "El saldo debería ser 4210 después de cargar 3000 adicionales.");
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

            // Validar que el boleto se generí y no se descontó saldo
            Assert.IsNotNull(boleto, "El boleto debería generarse correctamente.");
            Assert.AreEqual(0, tarjeta.saldo, "El saldo debería permanecer en 0 ya que el viaje es gratuito.");
            var tarjeta2 = new FranquiciaCompleta(3, -1000);
            var boleto2 = colectivo132.pagarCon(tarjeta2);
            Assert.IsNotNull(boleto2, "El boleto debería generarse correctamente incluso con saldo negativo.");
            Assert.AreEqual(-1000, tarjeta2.saldo, "El saldo debería permanecer en -1000 ya que el viaje es gratuito.");
        }
        [Test]
        public void TestMaxCarga()
        {
            var tarjeta = new Tarjeta(4, 39000);
            int resultado = tarjeta.cargar(2000);
            Assert.AreEqual(2, resultado, "Debería retornar 2 al superar el lmite de saldo permitido.");
            Assert.AreEqual(39000, tarjeta.saldo, "El saldo no debería cambiar al intentar cargar más allá del límite.");
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
    }

}

