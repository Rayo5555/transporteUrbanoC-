using NUnit.Framework;
using TransporteUrbano;

namespace TestTransporte
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            TransporteUrbano.Colectivo colectivo145_133 = new TransporteUrbano.Colectivo("145 133");
            TransporteUrbano.Colectivo colectivo132 = new TransporteUrbano.Colectivo("132");
            
        }

        [Test]
        public void Test1()
        {
            
        }
    }
}