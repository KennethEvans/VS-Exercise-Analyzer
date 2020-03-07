using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using About;
using KEUtils;

namespace Exercise_Analyzer {
    public partial class MainForm : Form {
        public static readonly String NL = Environment.NewLine;
        private string inputFile;
        private List<ExerciseData> exerciseDataList;
        private const bool processSilent = false;

        public MainForm() {
            InitializeComponent();

            writeInfo("Exercise Analyzer " + DateTime.Now);
            exerciseDataList = new List<ExerciseData>();
        }

        /// <summary>
        /// Appends the input plus a NL to the textBoxInfo.
        /// </summary>
        /// <param name="line"></param>
        public void writeInfo(string line) {
            textBoxInfo.AppendText(line + NL);
        }

        public void processFiles(MainForm app, string[] fileNames, bool silent = true) {
            if (fileNames == null) return;
            ExerciseData data;
            foreach (string fileName in fileNames) {
                data = null;
                try {
                    string ext = Path.GetExtension(fileName);
                    if (String.IsNullOrEmpty(ext)) {
                        app.writeInfo(fileName + ": Extension must be GPX or TCX");
                        return;
                    }
                    if (ext.ToLower().Equals(".gpx")) {
                        app.writeInfo(NL + "Processing " + fileName);
                        data = ExerciseData.processGpx(fileName);
                    } else if (ext.ToLower().Equals(".tcx")) {
                        app.writeInfo(NL + "Processing " + fileName);
                        data = ExerciseData.processTcx(fileName);
                    }
                    if (data == null) {
                        writeInfo("Failed to process " + fileName);
                    } else { 
                        exerciseDataList.Add(data);
                        if (!silent) writeInfo(data.info());
                    }
                } catch (Exception ex) {
                    app.writeInfo("Failed to read " + fileName
                        + NL + "Exception: " + ex + NL + ex.Message);
                    continue;
                }
            }
        }


        private void file_Exit_click(object sender, EventArgs e) {
            Close();
        }

        private void help_About_click(object sender, EventArgs e) {
            AboutBox dlg = new AboutBox();
            dlg.ShowDialog();
        }

        private void button_clear_click(object sender, EventArgs e) {
            textBoxInfo.Text = "";
        }

        private void file_ProcessSingleFile_click(object sender, EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX|*.gpx|TCX|*.tcx";
            dlg.Title = "Select an Exercise File";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                inputFile = dlg.FileName;
                if (inputFile == null) {
                    Utils.warnMsg("Failed to open Input file");
                    return;
                }
                test(inputFile);
            }
        }

        private void file_ProcessFiles_click(object sender, EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX, TCX|*.gpx;*.tcx";
            dlg.Title = "Select files to process";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    Utils.warnMsg("Failed to open files to process");
                    return;
                }
                string[] fileNames = dlg.FileNames;
                processFiles(this, fileNames, processSilent);
            }
        }
    }
}