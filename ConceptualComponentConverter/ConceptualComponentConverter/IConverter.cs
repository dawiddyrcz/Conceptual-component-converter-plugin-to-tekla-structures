namespace ConceptualComponentConverter
{
    public delegate bool CanceledDelegate();

    public interface IConverter
    {
        event ReportProgress ProgressChanged;

        void Run();
        void Cancel();
    }
}