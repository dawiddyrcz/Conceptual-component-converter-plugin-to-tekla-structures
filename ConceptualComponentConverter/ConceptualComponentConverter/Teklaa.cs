using System;
using System.Collections.Generic;
using TeklaOpenAPIExtension;
using TSM = Tekla.Structures.Model;

namespace ConceptualComponentConverter
{
    public class Teklaa : ITekla
    {
        public bool IsRunning()
        {
            var model = new TSM.Model();
            return model.GetConnectionStatus();
        }

        public bool IsAnyConnectionSelected()
        {
            var selector = new TSM.UI.ModelObjectSelector();
            var selectedObjects = selector.GetSelectedObjects();

            if (selectedObjects.GetSize() == 0)
                return false;

            while (selectedObjects.MoveNext())
            {
                if (selectedObjects.Current is TSM.BaseComponent)
                {
                    return true;
                }
            }

            return false;
        }

        internal List<TSM.BaseComponent> GetSelectedComponents()
        {
            var selector = new TSM.UI.ModelObjectSelector();
            return selector.GetSelectedObjects().ToList<TSM.BaseComponent>(true);
        }
    }
}
