using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using KEUtils;
using www.garmin.com.xmlschemas.TrainingCenterDatabase.v2;
using www.topografix.com.GPX_1_1;

namespace Exercise_Analyzer {
    public partial class MainForm : Form {
        public static void formatTcxGpx() {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX and TCX|*.gpx;*.tcx|GPX|*.gpx|TCX|*.tcx";
            dlg.Title = "Select files to format";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    Utils.warnMsg("Failed to open files to process");
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    formatSingleTcxGpx(fileName);
                }
            }
        }

        public static void formatSingleTcxGpx(string fileName) {
            string ext = Path.GetExtension(fileName);
            if (ext == null) {
                Utils.errMsg("formatSingleTcxGpx: Cannot handle files with no extension");
                return;
            }
            if (ext.ToLower().Equals(".tcx")) {
                TrainingCenterDatabase tcx = TrainingCenterDatabase.Load(fileName);
                string saveFilename = getSaveName(fileName);
                if (saveFilename != null) {
                    tcx.Save(saveFilename);
                }
            } else if (ext.ToLower().Equals(".gpx")) {
                gpx gpxType = gpx.Load(fileName);
                string saveFilename = getSaveName(fileName);
                if (saveFilename != null) {
                    gpxType.Save(saveFilename);
                }
            } else {
                Utils.errMsg("Not a supported extension: " + ext);
                return;
            }
        }

        public static void formatXml() {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Typical|*.gpx;*.tcx;*.xml|GPX|*.gpx|TCX|*.tcx|XML|*.xml";
            dlg.Title = "Select files to format";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    Utils.warnMsg("Failed to open files to process");
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    formatXml(fileName);
                }
            }
        }

        public static void formatXml(string fileName) {
            string ext = Path.GetExtension(fileName);
            if (ext == null) {
                Utils.errMsg("formatXml: Cannot handle files with no extension");
                return;
            }
            XDocument doc = XDocument.Load(fileName);
            string saveFilename = getSaveName(fileName);
            if (saveFilename != null) {
                // (Use second argument to get unformatted)
                doc.Save(saveFilename);
            }
        }

        public static string getSaveName(string fileName) {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Select saved file";
            string directory = Path.GetDirectoryName(fileName);
            string ext = Path.GetExtension(fileName);
            string name = Path.GetFileNameWithoutExtension(fileName);
            dlg.InitialDirectory = directory;
            dlg.FileName = name + ".formatted" + ext;
            if (ext.ToLower().Equals(".tcx")) {
                dlg.Filter = "TCX|*.tcx";
            } else if (ext.ToLower().Equals(".gpx")) {
                dlg.Filter = "GPX|*.gpx";
            } else if (ext.ToLower().Equals(".xml")) {
                dlg.Filter = "XML|*.xml";
            } else {
                dlg.Filter = "All files|*.*";
            }
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                return dlg.FileName;
            } else {
                return null;
            }
        }

        public static void getSingleFileInfo(MainForm mainForm) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX and TCX|*.gpx;*.tcx|GPX|*.gpx|TCX|*.tcx";
            dlg.Title = "Select file for info";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    Utils.warnMsg("Failed to open files to process");
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    getSingleFileInfo(fileName, mainForm);
                }
            }
        }

        public static void getSingleFileInfo(string fileName, MainForm mainForm) {
            string ext = Path.GetExtension(fileName);
            if (ext == null) {
                Utils.errMsg("getSingleFileInfo: Cannot handle files with no extension");
                return;
            }
            if (ext.ToLower().Equals(".tcx")) {
                try {
                    ExerciseData data = ExerciseData.processTcx2(fileName);
                    mainForm.writeInfo(NL + data.info());
                } catch (Exception ex) {
                    Utils.excMsg("Error getting TCX single file info", ex);
                    return;
                }
            } else if (ext.ToLower().Equals(".gpx")) {
                try {
                    ExerciseData data = ExerciseData.processGpx2(fileName);
                    mainForm.writeInfo(NL + data.info());
                } catch (Exception ex) {
                    Utils.excMsg("Error getting GPX single file info", ex);
                    return;
                }
            } else {
                Utils.errMsg("Not supported extension: " + ext);
                return;
            }
        }
    }
}
