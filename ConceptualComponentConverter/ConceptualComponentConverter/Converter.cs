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
    public delegate void ReportProgress(string message, int curent = 0, int max = 1);

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

        
        public void Run(bool fromDetailToConceptual = false)
        {
            TS.TeklaStructures.Connect();

            int i = 0, max = 0;
            ProgressChanged?.Invoke("Getting components from model...");

            var tekla = (Teklaa)objectFactory.GetTekla();
            var selectedComponents = tekla.GetSelectedComponents();

            ProgressChanged?.Invoke("Preparing data...");
            if (cancel) goto END;

            CreateConceptualFilterFile();

            List<TSM.BaseComponent> componentsToConvert;

            if (fromDetailToConceptual)
                componentsToConvert = selectedComponents.Where(c => IsNotConceptual(c)).ToList();
            else
                componentsToConvert = selectedComponents.Where(c => IsConceptual(c)).ToList();

            ProgressChanged?.Invoke("Starting conversion...");
            if (cancel) goto END;

            var findChildrenTask = new Task<List<TSM.BaseComponent>>(() => GetChildrenComponents(selectedComponents));
            findChildrenTask.Start();

            //Main loop where we convert components
            max = componentsToConvert.Count;
            foreach (var component in componentsToConvert)
            {
                if (cancel) goto END;

                ConvertComponent(component);

                ProgressChanged?.Invoke("Converting...", ++i, max);
            }

            ProgressChanged?.Invoke("Converted", i, max);

            //But components could have children components (components inside component)
            //so we need to convert them too
            findChildrenTask.Wait();
            var result = findChildrenTask.Result;

            List<TSM.BaseComponent> childrenToConvert;

            if (fromDetailToConceptual)
                childrenToConvert = result.Where(c => IsNotConceptual(c)).ToList();
            else
                childrenToConvert = result.Where(c => IsConceptual(c)).ToList();

            if (childrenToConvert.Count > 0)
            {
                max += childrenToConvert.Count;

                ProgressChanged?.Invoke("Starting conversion of children components...", i , max);
                if (cancel) goto END;

                //Converting children objects
                foreach (var component in childrenToConvert)
                {
                    if (cancel) goto END;

                    ConvertComponent(component);

                    ProgressChanged?.Invoke("Converting children components...", ++i, max);
                }
            }

            END:
            ProgressChanged?.Invoke($"Converted {i}/{max} components");
        }

        private bool IsConceptual(TSM.BaseComponent component)
        {
            return TSM.Operations.Operation.ObjectMatchesToFilter(component, "_isConceptual__");
        }

        private bool IsNotConceptual(TSM.BaseComponent component)
        {
            return TSM.Operations.Operation.ObjectMatchesToFilter(component, "_isNotConceptual__");
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
            new TSM.UI.ModelObjectSelector().Select(new System.Collections.ArrayList() { component });

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

        private void CreateConceptualFilterFile()
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

        private void CreateNotConceptualFilterFile()
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
        albl_No 
        0 
        && 
        }
    }
";

            var modelPath = new TSM.Model().GetInfo().ModelPath;
            var attributesPath = System.IO.Path.Combine(modelPath, "attributes");
            var filePath = System.IO.Path.Combine(attributesPath, "_isNotConceptual__.SObjGrp");

            File.WriteAllText(filePath, filterText);
        }


        private List<TSM.BaseComponent> GetChildrenComponents(List<TSM.BaseComponent> components)
        {
            var output = new List<TSM.BaseComponent>(100);

            foreach (var component in components)
            {
                var children = component.GetChildren().ToList<TSM.BaseComponent>();
                output.AddRange(children);
            }

            return output;
        }
    }
}
