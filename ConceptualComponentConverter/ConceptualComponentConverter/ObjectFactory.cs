using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ConceptualComponentConverter
{
    public class ObjectFactory
    {
        public bool MockTekla { get; set; } = false;

        public ObjectFactory()
        {

        }
        public ITekla GetTekla()
        {
            if (MockTekla)
                return new MockTekla();
            else
                return new Tekla();

        }

        public IConverter GetConverter()
        {
            if (MockTekla)
                return new MockConverter();
            else
                return new Converter();
        }
    }
}
