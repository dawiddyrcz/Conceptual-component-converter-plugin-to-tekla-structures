using System;
using System.IO;
using System.Windows.Forms;
using Tekla.Structures.Dialog;

namespace ConceptualComponentConverterPlugin
{
    public partial class ConceptualComponentConverterPlugin_DummyForm : PluginFormBase
    {
        private readonly string _appFileName = "ConceptualComponentConverter.exe";
       
        public ConceptualComponentConverterPlugin_DummyForm()
        {
            this.Shown += ConceptualComponentConverterPlugin_DummyForm_Shown;
            InitializeComponent();
        }

        private void ConceptualComponentConverterPlugin_DummyForm_Shown(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                try
                {
                    System.Threading.Thread.Sleep(50);
                    this.Close();
                }
                catch { }
            }
           ));

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {

                CloseAllProcesses();

                var assemblyFile = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var assemlbyDirectory = Path.GetDirectoryName(assemblyFile);

                var teklaBinDir = Tekla.Structures.Dialog.StructuresInstallation.BinFolder;
                var appFullPath = System.IO.Path.Combine(
                    assemlbyDirectory
                    , _appFileName);
                
                Tekla.Structures.Model.Operations.Operation.DisplayPrompt(DateTime.Now.ToString("HH:mm:ss.fff") + " Trying to start application: " + appFullPath);

                System.Diagnostics.Process.Start(appFullPath);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {

            }
        }

        private void CloseAllProcesses()
        {
            var processName = Path.GetFileNameWithoutExtension(_appFileName);
            var processes = System.Diagnostics.Process.GetProcessesByName(processName);

            if (processes != null)
            {
                foreach (var process in processes)
                {
                    process.Kill();

                }
            }
        }
    }
}
