namespace ConceptualComponentConverter
{
    public interface IConverter
    {
        event ReportProgress ProgressChanged;

        void Run();
        void Cancel();
    }
}