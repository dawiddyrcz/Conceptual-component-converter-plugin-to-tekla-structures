namespace ConceptualComponentConverter
{
    public interface IConverter
    {
        event ReportProgress ProgressChanged;

        void Run(bool fromConceptualToDetail);
        void Cancel();
    }
}