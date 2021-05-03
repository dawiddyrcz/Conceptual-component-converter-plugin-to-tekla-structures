using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace ConceptualComponentConverter
{
    [RunInstaller(true)]
    public partial class ConComConvInstallerClass : System.Configuration.Install.Installer
    {
        public ConComConvInstallerClass()
        {
            //You can add versions from TS18.1 to TS2020i

            tSInstallFiles.AddTeklaVersion("20.0");
            tSInstallFiles.AddTeklaVersion("20.1");
            tSInstallFiles.AddTeklaVersion("21.0");
            tSInstallFiles.AddTeklaVersion("21.1");

            //For new versions is TSEP

            //tSInstallFiles.AddTeklaVersion("2016");
            //tSInstallFiles.AddTeklaVersion("2016i");
            //tSInstallFiles.AddTeklaVersion("2017");
            //tSInstallFiles.AddTeklaVersion("2017i");
            //tSInstallFiles.AddTeklaVersion("2018");
            //tSInstallFiles.AddTeklaVersion("2018i");
            //tSInstallFiles.AddTeklaVersion("2019");
            //tSInstallFiles.AddTeklaVersion("2019i");
            //tSInstallFiles.AddTeklaVersion("2020");
            //tSInstallFiles.AddTeklaVersion("2020i");

            //You can add versions from TS2016 Learning to TS2020i Learning

            //tSInstallFiles.AddTeklaVersion("2016 Learning");
            //tSInstallFiles.AddTeklaVersion("2016i Learning");
            //tSInstallFiles.AddTeklaVersion("2017 Learning");
            //tSInstallFiles.AddTeklaVersion("2017i Learning");
            //tSInstallFiles.AddTeklaVersion("2018 Learning");
            //tSInstallFiles.AddTeklaVersion("2018i Learning");
            //tSInstallFiles.AddTeklaVersion("2019 Learning");
            //tSInstallFiles.AddTeklaVersion("2019i Learning");
            //tSInstallFiles.AddTeklaVersion("2020 Learning");
            //tSInstallFiles.AddTeklaVersion("2020i Learning");

            InitializeComponent();
        }

        TSInstallFiles tSInstallFiles = new TSInstallFiles();

        private class SpecificTeklaFile
        {
            public string TeklaVersion { get; set; } = string.Empty;
            public string FileName { get; set; } = string.Empty;

            public SpecificTeklaFile(string teklaVersion, string fileName)
            {
                TeklaVersion = teklaVersion;
                FileName = fileName;
            }
        }

        private List<string> filesToCopy_commonExtensionDirectory = new List<string>()
        {
            
        };

        private List<string> filesToCopy_ToPluginFolder = new List<string>()
            {
                "ConceptualComponentConverter.exe",
                "ConceptualComponentConverterPlugin.dll",
            };

        private List<SpecificTeklaFile> filesToCopy_ToPluginFolder_specific = new List<SpecificTeklaFile>()
            {

            };

        private List<string> filesToCopy_ToBitmapFolder = new List<string>()
            {
                "et_element_Conceptual Component Converter.bmp",
            };

        private List<string> filesToCopy_ToCommonSystemFolder = new List<string>()
        {
            "ConceptualComponentConverter_ComponentCatalog.ac.xml"
        };
        
        private void CorrectFilePathes(string targetDir)
        {
            for (int i = 0; i < filesToCopy_ToPluginFolder.Count; i++)
            {
                filesToCopy_ToPluginFolder[i] = Path.Combine(targetDir, filesToCopy_ToPluginFolder[i]);
            }

            for (int i = 0; i < filesToCopy_ToPluginFolder_specific.Count; i++)
            {
                filesToCopy_ToPluginFolder_specific[i].FileName = Path.Combine(targetDir, filesToCopy_ToPluginFolder_specific[i].FileName);
            }

            for (int i = 0; i < filesToCopy_ToBitmapFolder.Count; i++)
            {
                filesToCopy_ToBitmapFolder[i] = Path.Combine(targetDir, filesToCopy_ToBitmapFolder[i]);
            }

            for (int i = 0; i < filesToCopy_ToCommonSystemFolder.Count; i++)
            {
                filesToCopy_ToCommonSystemFolder[i] = Path.Combine(targetDir, filesToCopy_ToCommonSystemFolder[i]);
            }

            for (int i = 0; i < filesToCopy_commonExtensionDirectory.Count; i++)
            {
                filesToCopy_commonExtensionDirectory[i] = Path.Combine(targetDir, filesToCopy_commonExtensionDirectory[i]);
            }
        }

        private void CloseTeklaMessage()
        {
            AGAIN:
            int tsProcessesCount = 0;

            foreach (var currentProces in Process.GetProcesses())
            {
                if (currentProces.ProcessName == "TeklaStructures")
                {
                    if (currentProces != null) tsProcessesCount++;
                }
            }

            if (tsProcessesCount > 0)
            {
                var msgBoxResult = System.Windows.Forms.MessageBox.Show("Close all Tekla Structures processes!", "Close Tekla Structures", System.Windows.Forms.MessageBoxButtons.RetryCancel);

                if (msgBoxResult == System.Windows.Forms.DialogResult.Retry) goto AGAIN;
                else throw new Exception("Instalation canceled by user");
            }
        }
        
        public override void Install(IDictionary stateSaver)
        {
            CloseTeklaMessage();

            var targetDir = Context.Parameters["TargetDir"].ToString();  //add to custom actions atributes:  /TargetDir="[TARGETDIR]\"
            CorrectFilePathes(targetDir);

            tSInstallFiles.CopyFilesTo_EnvBmpDirectory(filesToCopy_ToBitmapFolder);
            tSInstallFiles.CopyFilesTo_EnvCommonSystemDirectory(filesToCopy_ToCommonSystemFolder);
            tSInstallFiles.CopyFilesTo_TSPluginDirectory(filesToCopy_ToPluginFolder);
            tSInstallFiles.CopyTeklaStructuresConfigFiles(filesToCopy_ToPluginFolder);
            tSInstallFiles.CopyFilesTo_EnvCommonExtensionDirectory(filesToCopy_commonExtensionDirectory);
            tSInstallFiles.CopyTeklaStructuresConfigFilesToCommonExtensionDirectory(filesToCopy_commonExtensionDirectory);

            tSInstallFiles.CopyFilesTo_TSPluginDirectory_Specific(filesToCopy_ToPluginFolder_specific);
            tSInstallFiles.CopyTeklaStructuresConfigFiles_Specific(filesToCopy_ToPluginFolder_specific);
            base.Install(stateSaver);
        }

        public override void Uninstall(IDictionary savedState)
        {
            CloseTeklaMessage();

            var targetDir = Context.Parameters["TargetDir"].ToString();  //add to custom actions atributes:  /TargetDir="[TARGETDIR]\"
            CorrectFilePathes(targetDir);

            tSInstallFiles.DeleteFilesFrom_EnvBmpDirectory(filesToCopy_ToBitmapFolder);
            tSInstallFiles.DeleteFilesFrom_EnvCommonSystemDirectory(filesToCopy_ToCommonSystemFolder);
            tSInstallFiles.DeleteFilesFrom_TSPluginDirectory(filesToCopy_ToPluginFolder);
            tSInstallFiles.DeleteConfigFiles(filesToCopy_ToPluginFolder);
            tSInstallFiles.DeleteFilesFrom_EnvCommonExtensionDirectory(filesToCopy_commonExtensionDirectory);
            tSInstallFiles.DeleteConfigFiles(filesToCopy_commonExtensionDirectory);

            tSInstallFiles.DeleteFilesFrom_TSPluginDirectory_Specific(filesToCopy_ToPluginFolder_specific);
            base.Uninstall(savedState);
        }
        
        /// <summary>
        /// This class copy (or delete) files from the directories of Tekla Structures Application
        /// </summary>
        class TSInstallFiles
        {
            public TSInstallFiles()
            {

            }

            private List<TeklaData> tsData = new List<TeklaData>();

            /// <summary>
            /// Adds Tekla Version to internal list and gets information from the system registry
            /// </summary>
            /// <param name="version"></param>
            public void AddTeklaVersion(string version)
            {
                //Bellow you can add another TS version with its registry key
                //Version 17.0 dont have redirections in TeklaStructures.exe.config file so it will not working
                //Version 18.0 not confirmed
                //Version 2019, 2019i, 2020, 2020i added in 2018 year so not confirmed

                switch (version)
                {
                    case "18.1":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "18.1", @"SOFTWARE\Tekla\Structures\18.1\setup", false);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "19.0":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "19.0", @"SOFTWARE\Tekla\Structures\19.0\setup", false);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "19.1":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "19.1", @"SOFTWARE\Tekla\Structures\19.1\setup", false);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "20.0":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "20.0", @"SOFTWARE\Tekla\Structures\20.0\setup", false);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "20.1":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "20.1", @"SOFTWARE\Tekla\Structures\20.1\setup", false);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "21.0":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "21.0", @"SOFTWARE\Tekla\Structures\21.0\setup", false);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "21.1":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "21.1", @"SOFTWARE\Tekla\Structures\21.1\setup", false);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2016":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2016", @"SOFTWARE\Tekla\Structures\2016\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2016i":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2016i", @"SOFTWARE\Tekla\Structures\2016i\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2017":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2017", @"SOFTWARE\Tekla\Structures\2017\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2017i":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2017i", @"SOFTWARE\Tekla\Structures\2017i\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2018":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2018", @"SOFTWARE\Tekla\Structures\2018\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2018i":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2018i", @"SOFTWARE\Tekla\Structures\2018i\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2019":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2019.0", @"SOFTWARE\TRIMBLE\Tekla Structures\2019.0\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2019i":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2019.1", @"SOFTWARE\TRIMBLE\Tekla Structures\2019.1\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2020":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2020.0", @"SOFTWARE\TRIMBLE\Tekla Structures\2020.0\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }

                    case "2016 Learning":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2016", @"SOFTWARE\Tekla\Structures\2016 Learning\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2016i Learning":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2016i", @"SOFTWARE\Tekla\Structures\2016i Learning\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2017 Learning":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2017", @"SOFTWARE\Tekla\Structures\2017 Learning\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2017i Learning":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2017i", @"SOFTWARE\Tekla\Structures\2017i Learning\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2018 Learning":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2018", @"SOFTWARE\Tekla\Structures\2018 Learning\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2018i Learning":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2018i", @"SOFTWARE\Tekla\Structures\2018i Learning\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2019 Learning":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2019", @"SOFTWARE\Tekla\Structures\2019 Learning\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2019i Learning":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2019i", @"SOFTWARE\Tekla\Structures\2019i Learning\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2020 Learning":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2020", @"SOFTWARE\Tekla\Structures\2020 Learning\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    case "2020i Learning":
                        {
                            var exsistingTSDataIndex = this.tsData.FindIndex(x => x.Version == version);
                            var newTSData = new TeklaData(version, "2020i", @"SOFTWARE\Tekla\Structures\2020i Learning\setup", true);

                            if (exsistingTSDataIndex < 0) this.tsData.Add(newTSData);
                            else this.tsData[exsistingTSDataIndex] = newTSData;

                            break;
                        }
                    default:
                        break;
                }
            }

            /// <summary>
            /// Copies all files from the list to the Teklas directory: TSDirectory
            /// </summary>
            /// <param name="filesFullPathList"></param>
            public void CopyFilesTo_TSDirectory(List<string> filesFullPathList)
            {
                foreach (var currentFilePath in filesFullPathList)
                {
                    foreach (var currentTeklaData in this.tsData)
                    {
                        if (currentTeklaData.IsTeklaInstaled) CopyFileToDirectory(currentFilePath, currentTeklaData.TSDirectory);
                    }
                }
            }

            /// <summary>
            /// Delete all files from the list from the Teklas directory: TSDirectory
            /// </summary>
            /// <param name="filesFullPathList"></param>
            public void DeleteFilesFrom_TSDirectory(List<string> filesFullPathList)
            {
                foreach (var currentFilePath in filesFullPathList)
                {
                    foreach (var currentTeklaData in this.tsData)
                    {
                        if (currentTeklaData.IsTeklaInstaled) DeleteFileFromDirectory(currentFilePath, currentTeklaData.TSDirectory);
                    }
                }
            }

            /// <summary>
            /// Copies all files from the list to the Teklas directory: TSPluginDirectory
            /// </summary>
            /// <param name="filesFullPathList"></param>
            public void CopyFilesTo_TSPluginDirectory(List<string> filesFullPathList)
            {
                foreach (var currentFilePath in filesFullPathList)
                {
                    foreach (var currentTeklaData in this.tsData)
                    {
                        if (currentTeklaData.IsTeklaInstaled) CopyFileToDirectory(currentFilePath, currentTeklaData.TSPluginDirectory);
                    }
                }
            }

            public void CopyFilesTo_TSPluginDirectory_Specific(List<SpecificTeklaFile> filePathesAndVersions)
            {
                foreach (var currentFileData in filePathesAndVersions)
                {
                    foreach (var currentTeklaData in this.tsData)
                    {
                        if (currentTeklaData.IsTeklaInstaled)
                        {
                            if (currentTeklaData.Version.Equals(currentFileData.TeklaVersion, StringComparison.InvariantCulture) 
                                ||
                                (currentTeklaData.IsNewTekla && currentFileData.TeklaVersion.Equals("new"))
                                )
                                CopyFileToDirectory(currentFileData.FileName, currentTeklaData.TSPluginDirectory);
                        }
                    }
                }
            }

            /// <summary>
            /// Delete all files from the list from the Teklas directory: TSPluginDirectory
            /// </summary>
            /// <param name="filesFullPathList"></param>
            public void DeleteFilesFrom_TSPluginDirectory(List<string> filesFullPathList)
            {
                foreach (var currentFilePath in filesFullPathList)
                {
                    foreach (var currentTeklaData in this.tsData)
                    {
                        if (currentTeklaData.IsTeklaInstaled)
                        {
                            DeleteFileFromDirectory(currentFilePath, currentTeklaData.TSPluginDirectory);
                        }
                    }
                }
            }

            public void DeleteFilesFrom_TSPluginDirectory_Specific(List<SpecificTeklaFile> filesFullPathListWithVersions)
            {
                foreach (var currentFileData in filesFullPathListWithVersions)
                {
                    foreach (var currentTeklaData in this.tsData)
                    {
                        if (currentTeklaData.IsTeklaInstaled)
                        {
                            try
                            {
                                DeleteFileFromDirectory(currentFileData.FileName, currentTeklaData.TSPluginDirectory);
                            }
                            catch { }
                        }
                    }
                }
            }

            /// <summary>
            /// Copies all files from the list to the Teklas directory: EnvCommonSystemDirectory
            /// </summary>
            /// <param name="filesFullPathList"></param>
            public void CopyFilesTo_EnvCommonSystemDirectory(List<string> filesFullPathList)
            {
                foreach (var currentFilePath in filesFullPathList)
                {
                    foreach (var currentTeklaData in this.tsData)
                    {
                        if (currentTeklaData.IsTeklaInstaled) CopyFileToDirectory(currentFilePath, currentTeklaData.EnvCommonSystemDirectory);
                    }
                }
            }

            /// <summary>
            /// Delete all files from the list from the Teklas directory: EnvCommonSystemDirectory
            /// </summary>
            /// <param name="filesFullPathList"></param>
            public void DeleteFilesFrom_EnvCommonSystemDirectory(List<string> filesFullPathList)
            {
                foreach (var currentFilePath in filesFullPathList)
                {
                    foreach (var currentTeklaData in this.tsData)
                    {
                        if (currentTeklaData.IsTeklaInstaled) DeleteFileFromDirectory(currentFilePath, currentTeklaData.EnvCommonSystemDirectory);
                    }
                }
            }

            /// <summary>
            /// Copies all files from the list to the Teklas directory: Enviroments Common extension directory
            /// </summary>
            /// <param name="filesFullPathList"></param>
            public void CopyFilesTo_EnvCommonExtensionDirectory(List<string> filesFullPathList)
            {
                foreach (var currentFilePath in filesFullPathList)
                {
                    foreach (var currentTeklaData in this.tsData)
                    {
                        if (currentTeklaData.IsTeklaInstaled) CopyFileToDirectory(currentFilePath, currentTeklaData.EnvCommonExtensionDir);
                    }
                }
            }

            /// <summary>
            /// Delete all files from the list from the Teklas directory: Enviroments Common extension directory
            /// </summary>
            /// <param name="filesFullPathList"></param>
            public void DeleteFilesFrom_EnvCommonExtensionDirectory(List<string> filesFullPathList)
            {
                foreach (var currentFilePath in filesFullPathList)
                {
                    foreach (var currentTeklaData in this.tsData)
                    {
                        if (currentTeklaData.IsTeklaInstaled) DeleteFileFromDirectory(currentFilePath, currentTeklaData.EnvCommonExtensionDir);
                    }
                }
            }

            /// <summary>
            /// Copies all files from the list to the Teklas directory: EnvBmpDirectory
            /// </summary>
            /// <param name="filesFullPathList"></param>
            public void CopyFilesTo_EnvBmpDirectory(List<string> filesFullPathList)
            {
                foreach (var currentFilePath in filesFullPathList)
                {
                    foreach (var currentTeklaData in this.tsData)
                    {
                        if (currentTeklaData.IsTeklaInstaled) CopyFileToDirectory(currentFilePath, currentTeklaData.EnvBmpDirectory);
                    }
                }
            }

            /// <summary>
            /// Delete all files from the list from the Teklas directory: EnvBmpDirectory
            /// </summary>
            /// <param name="filesFullPathList"></param>
            public void DeleteFilesFrom_EnvBmpDirectory(List<string> filesFullPathList)
            {
                foreach (var currentFilePath in filesFullPathList)
                {
                    foreach (var currentTeklaData in this.tsData)
                    {
                        if (currentTeklaData.IsTeklaInstaled) DeleteFileFromDirectory(currentFilePath, currentTeklaData.EnvBmpDirectory);
                    }
                }
            }

            private void CopyFileToDirectory(string sourceFilePath, string destinationDirectory)
            {
                if (File.Exists(sourceFilePath))
                {
                    if (Directory.Exists(destinationDirectory))
                    {
                        var fileName = new FileInfo(sourceFilePath).Name;
                        File.Copy(sourceFilePath, Path.Combine(destinationDirectory, fileName), true);
                    }
                }
            }

            private void DeleteFileFromDirectory(string sourceFilePath, string destinationDirectory)
            {
                var fileName = new FileInfo(sourceFilePath).Name;
                string fileFullName = Path.Combine(destinationDirectory, fileName);

                if (File.Exists(fileFullName))
                {
                    File.Delete(fileFullName);
                }
            }

            public void CopyTeklaStructuresConfigFiles(List<string> filesFullPathList)
            {
                foreach (var currentFilePath in filesFullPathList)
                {
                    foreach (var currentTSData in this.tsData)
                    {
                        if (currentTSData.IsTeklaInstaled && File.Exists(currentFilePath) && File.Exists(currentTSData.TSConfigFileFullPath))
                        {
                            ChangeTeklaStructuresConfigFileIfNescesary(currentTSData.TSConfigFileFullPath);

                            var currentFileName = new FileInfo(currentFilePath).Name;
                            var configDllFileName = Path.Combine(currentTSData.TSPluginDirectory, currentFileName + ".config");

                            File.Copy(currentTSData.TSConfigFileFullPath, configDllFileName, true);
                        }
                    }
                }
            }

            public void CopyTeklaStructuresConfigFiles_Specific(List<SpecificTeklaFile> filesFullPathListWithVersions)
            {
                foreach (var currentFileData in filesFullPathListWithVersions)
                {
                    foreach (var currentTeklaData in this.tsData)
                    {
                        if (currentTeklaData.Version.Equals(currentFileData.TeklaVersion, StringComparison.InvariantCulture)
                                ||
                                (currentTeklaData.IsNewTekla && currentFileData.TeklaVersion.Equals("new"))
                                )
                        {
                            if (currentTeklaData.IsTeklaInstaled && File.Exists(currentFileData.FileName) && File.Exists(currentTeklaData.TSConfigFileFullPath))
                            {
                                ChangeTeklaStructuresConfigFileIfNescesary(currentTeklaData.TSConfigFileFullPath);

                                var currentFileName = new FileInfo(currentFileData.FileName).Name;
                                var configDllFileName = Path.Combine(currentTeklaData.TSPluginDirectory, currentFileName + ".config");

                                File.Copy(currentTeklaData.TSConfigFileFullPath, configDllFileName, true);
                            } 
                        }
                    }
                }
            }

            public void CopyTeklaStructuresConfigFilesToCommonExtensionDirectory(List<string> filesFullPathList)
            {
                foreach (var currentFilePath in filesFullPathList)
                {
                    foreach (var currentTSData in this.tsData)
                    {
                        if (currentTSData.IsTeklaInstaled && File.Exists(currentFilePath) && File.Exists(currentTSData.TSConfigFileFullPath))
                        {
                            ChangeTeklaStructuresConfigFileIfNescesary(currentTSData.TSConfigFileFullPath);

                            var currentFileName = new FileInfo(currentFilePath).Name;
                            var configDllFileName = Path.Combine(currentTSData.EnvCommonExtensionDir, currentFileName + ".config");
                            
                            if (Directory.Exists(Path.GetDirectoryName(configDllFileName)))
                            {
                                File.Copy(currentTSData.TSConfigFileFullPath, configDllFileName, true);
                            }
                        }
                    }
                }
            }

            public void DeleteConfigFiles(List<string> filesFullPathList)
            {
                foreach (var currentFilePath in filesFullPathList)
                {
                    foreach (var currentTSData in this.tsData)
                    {
                        if (currentTSData.IsTeklaInstaled && File.Exists(currentTSData.TSConfigFileFullPath))
                        {
                            var currentFileName = new FileInfo(currentFilePath).Name;
                            var configDllFileName = Path.Combine(currentTSData.TSPluginDirectory, currentFileName + ".config");

                            if (File.Exists(configDllFileName)) File.Delete(configDllFileName);
                        }
                    }
                }
            }

            public void DeleteConfigFilesFromCommonExtensionDirectory(List<string> filesFullPathList)
            {
                foreach (var currentFilePath in filesFullPathList)
                {
                    foreach (var currentTSData in this.tsData)
                    {
                        if (currentTSData.IsTeklaInstaled && File.Exists(currentTSData.TSConfigFileFullPath))
                        {
                            var currentFileName = new FileInfo(currentFilePath).Name;
                            var configDllFileName = Path.Combine(currentTSData.EnvCommonExtensionDir, currentFileName + ".config");

                            if (File.Exists(configDllFileName)) File.Delete(configDllFileName);
                        }
                    }
                }
            }

            private void ChangeTeklaStructuresConfigFileIfNescesary(string configFileFullName)
            {
                //For Tekla Structures up to 21.1 redirection in config is to version 99.1.0.0 of assemblies. It is needed to change it to 9999.1.0.0
                if (File.Exists(configFileFullName))
                {
                    string configFileText = File.ReadAllText(configFileFullName);

                    if (configFileText.Contains("-99.1.0.0"))
                    {
                        var bakConfilFileFullName = Path.Combine(configFileFullName + ".bak");
                        if (!File.Exists(bakConfilFileFullName)) File.Copy(configFileFullName, bakConfilFileFullName, true);
                        configFileText = configFileText.Replace("-99.1.0.0", "-9999.1.0.0");
                        File.WriteAllText(configFileFullName, configFileText);
                    }

                    configFileText = File.ReadAllText(configFileFullName);

                    //To nie ma sensu bo nie działa

                    //if (configFileText.Contains("Tekla.Structures.CustomPropertyPlugin") == false)
                    //{
                    //    var bakConfilFileFullName = Path.Combine(configFileFullName + ".bak2");
                    //    if (!File.Exists(bakConfilFileFullName)) File.Copy(configFileFullName, bakConfilFileFullName, true);
                    //    configFileText = configFileText.Replace("</assemblyBinding>", @"<dependentAssembly>
                    //        <assemblyIdentity name=""Tekla.Structures.CustomPropertyPlugin"" publicKeyToken=""2f04dbe497b71114"" culture=""neutral"" />
                    //        <bindingRedirect oldVersion = ""0.0.0.0-9999.1.0.0"" newVersion = ""21.1.0.0"" />
                    //        </dependentAssembly>

                    //        </assemblyBinding> ");
                    //    File.WriteAllText(configFileFullName, configFileText);
                    //}
                }
            }

            /// <summary>
            /// This class gets Tekla Structures information from registry
            /// </summary>
            class TeklaData
            {
                /// <summary>
                /// Gets Tekla Structures folders information from registry and save it in properties
                /// </summary>
                /// <param name="version">Version of Tekla Structures eg: "20.1", "21.0", "2016", "2017i"</param>
                /// <param name="tsVersionDir">Name of Tekla Structures instalation directory eg: "20.1", "21.0", "2016", "2017i"</param>
                /// <param name="regKey">Registry key (in HKLM) where data is stored eg: @"SOFTWARE\Tekla\Structures\2016\setup" </param>
                public TeklaData(string version, string tsVersionDir, string regKey, bool isNewTekla)
                {
                    this.Version = version;
                    this.tsVersionDir = tsVersionDir;
                    this.regKey = regKey;
                    this.IsNewTekla = isNewTekla;
                    this.IsTeklaInstaled = false;

                    GetDataFromRegistry();
                    if (this.IsTeklaInstaled) SetOtherDirectories();
                }
                public bool IsNewTekla { get; internal set; }
                public string Version { get; internal set; }
                public bool IsTeklaInstaled { get; internal set; }

                private readonly string tsVersionDir;
                private readonly string regKey;
                public const string configFileName = "TeklaStructures.exe.config";

                //Registry values
                public string AppDir { get; internal set; }
                public string EnvDir { get; internal set; }
                public string EnvCommonExtensionDir { get; internal set; }
                public string HelpLocation { get; internal set; }
                public string ModelDir { get; internal set; }

                //Other values
                public string TSDirectory { get; internal set; }
                public string TSBinDirectory { get; internal set; }
                public string TSPluginDirectory { get; internal set; }
                public string EnvCommonSystemDirectory { get; internal set; }
                public string EnvBmpDirectory { get; internal set; }
                public string TSConfigFileFullPath { get; internal set; }

                private void GetDataFromRegistry()
                {
                    RegistryKey localKey32 =
                         RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine,
                             RegistryView.Registry32);
                    localKey32 = localKey32.OpenSubKey(this.regKey);

                    if (localKey32 != null)
                    {
                        this.AppDir = string.Empty;
                        this.AppDir = localKey32.GetValue("MainDir").ToString();
                        this.EnvDir = string.Empty;
                        this.EnvDir = localKey32.GetValue("EnvDir").ToString();
                        this.HelpLocation = string.Empty;
                        this.HelpLocation = localKey32.GetValue("HelpLocation").ToString();
                        this.ModelDir = string.Empty;
                        this.ModelDir = localKey32.GetValue("ModelDir").ToString();
                        localKey32.Close();

                        if (this.AppDir != string.Empty & this.EnvDir != string.Empty & this.HelpLocation != string.Empty & this.ModelDir != string.Empty)
                            this.IsTeklaInstaled = true;
                        else
                            this.IsTeklaInstaled = false;
                    }
                    else
                    {
                        //If localkey 32 bit is null than check 64 bit
                        RegistryKey localKey64 =
                            RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine,
                                RegistryView.Registry64);
                        localKey64 = localKey64.OpenSubKey(this.regKey);

                        if (localKey64 != null)
                        {
                            this.AppDir = string.Empty;
                            this.AppDir = localKey64.GetValue("MainDir").ToString();
                            this.EnvDir = string.Empty;
                            this.EnvDir = localKey64.GetValue("EnvDir").ToString();
                            this.HelpLocation = string.Empty;
                            this.HelpLocation = localKey64.GetValue("HelpLocation").ToString();
                            this.ModelDir = string.Empty;
                            this.ModelDir = localKey64.GetValue("ModelDir").ToString();
                            localKey64.Close();

                            if (this.AppDir != string.Empty & this.EnvDir != string.Empty & this.HelpLocation != string.Empty & this.ModelDir != string.Empty)
                                this.IsTeklaInstaled = true;
                            else
                                this.IsTeklaInstaled = false;
                        }
                        else
                        {
                            //No 32 bit and no 64 bit key
                            this.IsTeklaInstaled = false;
                        }
                    }

                }

                private void SetOtherDirectories()
                {
                    this.TSDirectory = Path.Combine(this.AppDir, this.tsVersionDir);
                    this.TSBinDirectory = Path.Combine(this.TSDirectory, "nt", "bin");
                    this.TSPluginDirectory = Path.Combine(this.TSBinDirectory, "plugins");
                    this.EnvCommonSystemDirectory = Path.Combine(this.EnvDir, this.tsVersionDir, "Environments", "common", "system");
                    this.EnvBmpDirectory = Path.Combine(this.EnvDir, this.tsVersionDir, "Bitmaps");

                    this.TSConfigFileFullPath = Path.Combine(this.TSBinDirectory, configFileName);


                    this.EnvCommonExtensionDir = Path.Combine(this.EnvDir, this.tsVersionDir, "Environments", "common", "extensions");
                }
            }

        }
    }
}
