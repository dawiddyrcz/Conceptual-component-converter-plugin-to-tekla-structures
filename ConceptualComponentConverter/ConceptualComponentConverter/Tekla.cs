using System;
using System.Collections.Generic;
using TeklaOpenAPIExtension;
using TSM = Tekla.Structures.Model;

namespace ConceptualComponentConverter
{
    public class Tekla : ITekla
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

        internal Dictionary<Guid, TSM.BaseComponent> GetSelectedComponents()
        {
            var selector = new TSM.UI.ModelObjectSelector();
            return selector.GetSelectedObjects().ToDictionaryGuid<TSM.BaseComponent>(true);
        }
    }
}
