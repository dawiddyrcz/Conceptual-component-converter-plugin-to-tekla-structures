using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptualComponentConverter
{
    public delegate void ReportProgress(int progressPercentage);

    public class Converter : IConverter
    {
        public Converter()
        {

        }

        public event ReportProgress ProgressChanged;

        public void Run()
        {
            //TODO method
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            //TODO method
            throw new NotImplementedException();
        }
    }
}
