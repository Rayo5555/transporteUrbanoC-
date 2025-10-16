using NUnit.Framework;
using TransporteUrbano;

namespace TestTransporte
{
    public class Tests
    {
        private TransporteUrbano.Tarjeta tarjeta1;
        private TransporteUrbano.Colectivo colectivo132;
        private TransporteUrbano.Colectivo colectivo145_133;
        [SetUp]
        public void Setup()
        {
            colectivo145_133 = new TransporteUrbano.Colectivo("145/133");
            colectivo132 = new TransporteUrbano.Colectivo("132");
            tarjeta1 = new TransporteUrbano.Tarjeta(1, 0);

        }

        [Test]
        public void NoPermitirSaldoMenorAlPermitido()
        {
            // Intentar pagar con saldo insuficiente
            tarjeta1.saldo = 300; // Menor al mínimo permitido de 380
            var boleto = colectivo132.pagarCon(tarjeta1);

            // Validar que no se generó un boleto y el saldo no cambió
            Assert.IsNull(boleto, "No se debería permitir pagar con saldo insuficiente.");
            Assert.AreEqual(300, tarjeta1.saldo, "El saldo no debería cambiar si no se permite el pago.");
        }

        [Test]
        public void DescuentoCorrectoConViajesPlus()
        {
            // Configurar saldo inicial y realizar un pago
            tarjeta1.saldo = 1580; // Suficiente para un viaje
            var boleto = colectivo145_133.pagarCon(tarjeta1);

            // Validar que el saldo se descuenta correctamente
            Assert.IsNotNull(boleto, "Se debería generar un boleto si hay saldo suficiente.");
            Assert.AreEqual(0, tarjeta1.saldo, "El saldo debería ser 0 después de un viaje.");

            // Intentar otro pago con saldo insuficiente (viaje plus)
            boleto = colectivo132.pagarCon(tarjeta1);

            tarjeta1.saldo = 380;

            // Validar que se permite el viaje y el saldo es negativo
            Assert.IsNotNull(boleto, "Se debería permitir un viaje plus.");
            Assert.AreEqual(-1200, tarjeta1.saldo, "El saldo debería reflejar el viaje plus como negativo hasta un máximo de 1200.");


        }
    }
}