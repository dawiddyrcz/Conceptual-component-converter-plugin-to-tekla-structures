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

        public void Run(bool reverse)
        {
            int max = 20;

            for (int i = 0; i < max; i++)
            {
                if (canceled)
                {
                    ProgressChanged?.Invoke("Converted " + (i+1) + " items");
                    return;
                }

                ProgressChanged?.Invoke("Converting...", i+1, max);
                System.Threading.Thread.Sleep(300);
            }

            ProgressChanged?.Invoke("Converted " + max + " items");
        }

    }
}