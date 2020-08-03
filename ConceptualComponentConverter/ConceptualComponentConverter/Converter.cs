using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSM = Tekla.Structures.Model;

namespace ConceptualComponentConverter
{
    public delegate void ReportProgress(int progressPercentage);

    public class Converter : IConverter
    {
        public event ReportProgress ProgressChanged;
        
        private bool cancel = false;
        private ObjectFactory objectFactory = new ObjectFactory();

        public Converter()
        {

        }

        public void Cancel()
        {
            cancel = true;
        }

        
        public void Run()
        {
            var tekla = (Tekla)objectFactory.GetTekla();
            var selectedComponents = tekla.GetSelectedComponents();
            var componentsToConvert = PrepareComponents(selectedComponents);


        }

        private object PrepareComponents(Dictionary<Guid, TSM.BaseComponent> selectedComponents)
        {
            var model = new TSM.Model();
            var conceptualComponents = new Dictionary<Guid, TSM.BaseComponent>(selectedComponents.Count);

            foreach (var component in selectedComponents.Values)
            {
               
            }

        }
    }
}
