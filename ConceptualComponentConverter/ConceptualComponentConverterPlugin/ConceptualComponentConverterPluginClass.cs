using System.Collections.Generic;
using Tekla.Structures.Plugins;

namespace ConceptualComponentConverterPlugin
{
    public class ConceptualComponentConverterPluginClass_StructuresData
    {

    }

    [Plugin("Conceptual Component Converter")]
    [PluginUserInterface("ConceptualComponentConverterPlugin.ConceptualComponentConverterPlugin_DummyForm")]
    [InputObjectDependency(InputObjectDependency.NOT_DEPENDENT)]

    public class ConceptualComponentConverterPluginClass :PluginBase
    {
        private readonly ConceptualComponentConverterPluginClass_StructuresData _data;

        public ConceptualComponentConverterPluginClass(ConceptualComponentConverterPluginClass_StructuresData data)
        {
            this._data = data;
        }

        public override List<InputDefinition> DefineInput() => new List<InputDefinition>();

        public override bool Run(List<InputDefinition> Input) => true;
    }
}
