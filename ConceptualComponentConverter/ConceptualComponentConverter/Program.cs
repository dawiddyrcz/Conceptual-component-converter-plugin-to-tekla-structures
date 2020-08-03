using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ConceptualComponentConverter
{
    internal static class Program
    {
        [STAThread]
        public static void Main()
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                if (new Tekla.Structures.Model.Model().GetConnectionStatus())
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    IntPtr h1 = Tekla.Structures.Dialog.MainWindow.Frame.Handle;
                    var mainForm = new MainForm();
                    mainForm.Show(new WindowWrapper(h1));
                    Application.Run();
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    var mainForm = new MainForm();
                    mainForm.TopMost = true;
                    Application.Run(mainForm);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "FATAL_ERROR", MessageBoxButtons.OK, icon: MessageBoxIcon.Error);

                try
                {
                    File.WriteAllText("log_fatalError.txt",DateTime.Now.ToString() +"\n" + ex.ToString());
                }
                catch { }
            }
        }
    }

    public class WindowWrapper : System.Windows.Forms.IWin32Window
    {
        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private IntPtr _hwnd;
    }
}
