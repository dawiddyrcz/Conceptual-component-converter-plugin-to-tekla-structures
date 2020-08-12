using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Tekla.Structures.Dialog;

namespace ConceptualComponentConverterPlugin
{
    public partial class ConceptualComponentConverterPlugin_DummyForm : PluginFormBase
    {
        private readonly string _appName = "ConceptualComponentConverter.exe";
        private readonly string _appDirectory = System.IO.Path.Combine("applications","Tekla", "Model" , "ConceptualComponentConverter");

        public ConceptualComponentConverterPlugin_DummyForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Opacity = 0;

            this.Shown += ConceptualComponentConverterPlugin_DummyForm_Shown;
            InitializeComponent();
        }

        private void ConceptualComponentConverterPlugin_DummyForm_Shown(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                this.Close();
            }
           ));

        }

        

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {

                CloseAllProcesses();

                var teklaBinDir = Tekla.Structures.Dialog.StructuresInstallation.BinFolder;
                var appFullPath = System.IO.Path.Combine(teklaBinDir, _appDirectory, _appName);

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
            var processName = Path.GetFileNameWithoutExtension(_appName);
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
