namespace ConceptualComponentConverter
{
    public class MockConverter : IConverter
    {
        public event ReportProgress ProgressChanged;

        bool canceled = false;

        public void Cancel()
        {
            canceled = true;
        }

        public void Run()
        {
            int max = 20;

            for (int i = 0; i < max; i++)
            {
                if (canceled) break;

                var progress = (int)100.0*i / max;
                ProgressChanged?.Invoke(progress+1);
                System.Threading.Thread.Sleep(300);
            }
        }

    }
}