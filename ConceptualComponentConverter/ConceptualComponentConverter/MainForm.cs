using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConceptualComponentConverter
{
    public partial class MainForm : Form
    {
        ObjectFactory objectFactory = new ObjectFactory();
        public MainForm()
        {
            InitializeComponent();

            //objectFactory.MockTekla = true;

        }

        private void Start_button_Click(object sender, EventArgs e)
        {
            try
            {
                var tekla = objectFactory.GetTekla();

                if (!this.backgroundWorker1.IsBusy)
                {
                    if (tekla.IsRunning())
                    {
                        if (tekla.IsAnyConnectionSelected())
                        {
                            this.backgroundWorker1.RunWorkerAsync();
                        }
                        else throw new MessageException("Nothing is selected");
                    }
                    else throw new MessageException("Could not find Tekla Structures process");
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

       
        private void Cancel_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.backgroundWorker1.IsBusy)
                {
                    this.backgroundWorker1.CancelAsync();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        struct ProgressState
        {
            public string Message;
            public int Current;
            public int Max;

            public ProgressState(string message, int current, int max)
            {
                Message = message;
                Current = current;
                Max = max;
            }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var converter = objectFactory.GetConverter();

                converter.ProgressChanged += (string message, int current, int max) =>
                {
                    this.backgroundWorker1.ReportProgress(current, new ProgressState(message, current, max));

                    if (this.backgroundWorker1.CancellationPending)
                        converter.Cancel();
                };
                
                converter.Run();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                var p = (ProgressState)e.UserState;

                var progressPercentage = (int)(100.0 * p.Current / p.Max);
                progressPercentage = Math.Min(100, progressPercentage);
                progressPercentage = Math.Max(0, progressPercentage);

                this.progressBar1.Value = progressPercentage;
                this.status_label.Text = p.Message;

                if (p.Max == 1)
                    this.count_label.Text = string.Empty;
                else
                    this.count_label.Text = p.Current + "/" + p.Max;
            }
            catch { }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.progressBar1.Value = 0;
        }

        private void HandleException(Exception ex)
        {
            if (ex is MessageException)
            {
                MessageBox.Show(ex.Message, "Message");
            }
            else
            {
                Debug.WriteLine("ERROR: " + ex.ToString());
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
            }
        }

    }
}
