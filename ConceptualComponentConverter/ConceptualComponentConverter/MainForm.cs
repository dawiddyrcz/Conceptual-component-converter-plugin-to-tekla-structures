﻿using System;
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

            //objectFactory.MockTekla = true;\
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
                            var result = MessageBox.Show("Do you want to convert selected components?", "Are you sure?", MessageBoxButtons.YesNo, 
                                icon: MessageBoxIcon.Question, defaultButton: MessageBoxDefaultButton.Button2);

                            if (result == DialogResult.Yes)
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

                try
                {
                    System.IO.File.WriteAllText("log_error.txt", DateTime.Now.ToString() + "\n" + ex.ToString());
                }
                catch { }
            }
        }


        //***************************
        //Pamietaj aby dodac event w designerze
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (new Tekla.Structures.Model.Model().GetConnectionStatus())
            {
                Tekla.Structures.Dialog.MainWindow.Frame.AddExternalWindow(this.Name, this.Handle);
                _events = new Tekla.Structures.Model.Events();
                RegisterEventHandler();
            }

        }

        protected override void OnClosed(EventArgs e)
        {
            if (_events != null)
            {
                UnRegisterEventHandler();
                Tekla.Structures.Dialog.MainWindow.Frame.RemoveExternalWindow(this.Name, this.Handle);
            }
            base.OnClosed(e);
        }

        private Tekla.Structures.Model.Events _events = null;
        private object _tsExitEventHandlerLock = new object();

        public void RegisterEventHandler()
        {
            _events.TeklaStructuresExit += Events_TeklaExitEvent;
            _events.Register();
        }

        public void UnRegisterEventHandler()
        {
            if (_events != null) _events.UnRegister();
        }

        void Events_TeklaExitEvent()
        {
            /* Make sure that the inner code block is running synchronously */
            lock (_tsExitEventHandlerLock)
            {
                Application.Exit();
            }
        }

        private void license_linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/dawiddyrcz/Conceptual-component-converter-plugin-to-tekla-structures/blob/master/LICENSE");
            }
            catch { }
        }

        private void ddbim_linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.ddbim.pl/go/fromconceptualcomponentconverterapp/");
            }
            catch { }
        }
    }
}
