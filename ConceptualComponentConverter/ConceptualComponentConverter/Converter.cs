using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklaOpenAPIExtension;
using TSM = Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using TS = Tekla.Structures;
using System.IO;

namespace ConceptualComponentConverter
{
    public delegate void ReportProgress(int progressPercentage);

    public class Converter : IConverter
    {
        public event ReportProgress ProgressChanged;
        
        private bool cancel = false;
        private readonly ObjectFactory objectFactory = new ObjectFactory();

        public Converter()
        {

        }

        public void Cancel()
        {
            cancel = true;
        }

        
        public void Run()
        {
            TS.TeklaStructures.Connect();
            var tekla = (Teklaa)objectFactory.GetTekla();
            var selectedComponents = tekla.GetSelectedComponents();

            CreateFilterFile();
            var componentsToConvert = selectedComponents.Where(c => IsConceptual(c)).ToList();

            int i = 0;
            int max = componentsToConvert.Count;
            foreach (var component in componentsToConvert)
            {
                if (cancel) return;

                ConvertComponent(component);

                i++;
                ProgressChanged?.Invoke((int)(100.0 * i / max));
            }
        }

        private bool IsConceptual(TSM.BaseComponent component)
        {
            return TSM.Operations.Operation.ObjectMatchesToFilter(component, "_isConceptual__");
        }

        private void CreateFilterFile()
        {
            var filterText = @"TITLE_OBJECT_GROUP 
{
    Version= 1.05 
    Count= 1 
    SECTION_OBJECT_GROUP 
    {
        0 
        1 
        co_component 
        proIsConceptual 
        albl_Is_conceptual 
        == 
        albl_Equals 
        albl_Yes 
        0 
        && 
        }
    }
";

            var modelPath = new TSM.Model().GetInfo().ModelPath;
            var attributesPath = System.IO.Path.Combine(modelPath, "attributes");
            var filePath = System.IO.Path.Combine(attributesPath, "_isConceptual__.SObjGrp");
            
            File.WriteAllText(filePath, filterText);
        }

        private void ConvertComponent(TSM.BaseComponent component)
        {
            //Update component from database
            component.Select();
            component.GetPhase(out TSM.Phase phase);

            //Check if is connection
            TSM.Connection connetion = null;
            TSM.Detail detail = null;
            T3D.Vector upVector = null;

            if (component is TSM.Connection)
            {
                connetion = component as TSM.Connection;
                upVector = connetion.UpVector;
            }
            else if (component is TSM.Detail)
            {
                detail = component as TSM.Detail;
                upVector = detail.UpVector;
            }

            //Select component in model
            new TSM.UI.ModelObjectSelector().Select(new System.Collections.ArrayList(){ component });

            //Convert component
            var akit = new TS.MacroBuilder();
            akit.Callback("acmdChangeJointTypeToCallback", "DETAIL", "main_frame");
            akit.Run();

            component.Select();

            //After conversion some connections changes its direction so we need to repair that connections
            if (connetion != null)
            {
                connetion.Select();
                connetion.AutoDirectionType = TS.AutoDirectionTypeEnum.AUTODIR_NA;
                connetion.UpVector = upVector;
            }

            //After conversion some details changes its direction so we need to repair that details
            if (detail != null)
            {
                detail.Select();
                detail.AutoDirectionType = TS.AutoDirectionTypeEnum.AUTODIR_NA;
                detail.UpVector = upVector;
            }

            //After conversion components change its phase to current we need to repait it
            component.SetPhase(phase);
            component.Modify();
        }
    }
}
