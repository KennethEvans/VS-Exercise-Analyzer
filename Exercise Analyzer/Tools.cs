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
                string saveFilename = getSaveName(fileName, ".formatted");
                if (saveFilename != null) {
                    tcx.Save(saveFilename);
                }
            } else if (ext.ToLower().Equals(".gpx")) {
                gpx gpxType = gpx.Load(fileName);
                string saveFilename = getSaveName(fileName, ".formatted");
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
            string saveFilename = getSaveName(fileName, ".formatted-xml");
            if (saveFilename != null) {
                // (Use second argument to get unformatted)
                doc.Save(saveFilename);
            }
        }

        public static string getSaveName(string fileName, string tag) {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Select saved file";
            string directory = Path.GetDirectoryName(fileName);
            string ext = Path.GetExtension(fileName);
            string name = Path.GetFileNameWithoutExtension(fileName);
            dlg.InitialDirectory = directory;
            dlg.FileName = name + tag + ext;
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
            dlg.Title = "Select files for info";
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
                    ExerciseData data = ExerciseData.processTcx(fileName);
                    mainForm.writeInfo(NL + data.info());
                } catch (Exception ex) {
                    Utils.excMsg("Error getting TCX single file info", ex);
                    return;
                }
            } else if (ext.ToLower().Equals(".gpx")) {
                try {
                    ExerciseData data = ExerciseData.processGpx(fileName);
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

        public static void recalculateTcx(MainForm mainForm) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TCX|*.tcx";
            dlg.Title = "Select files to recalculate";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    Utils.warnMsg("Failed to open files to process");
                    return;
                }
                string[] fileNames = dlg.FileNames;
                foreach (string fileName in fileNames) {
                    recalculateTcx(fileName, mainForm);
                }
            }
        }

        public static void recalculateTcx(string fileName, MainForm mainForm) {
            try {
                TrainingCenterDatabase tcx = ExerciseData.recalculateTcx(fileName);
                string saveFileName = getSaveName(fileName, ".recalculated");
                if (saveFileName != null) {
                    tcx.Save(saveFileName);
                    mainForm.writeInfo(NL + "Recalculated " + fileName + NL
                        + "  Output is " + saveFileName);
                } else {
                    return;
                }
            } catch (Exception ex) {
                Utils.excMsg("Error recalculating TCX", ex);
                return;
            }
        }

        public static void interpolateTcxFromGpx(MainForm mainForm) {
            string tcxFile = null, gpxFile = null;
            OpenFileDialog dlg1 = new OpenFileDialog();
            dlg1.Filter = "TCX|*.tcx";
            dlg1.Title = "Select TCX file to interpolate to";
            dlg1.Multiselect = false;
            if (dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg1.FileName == null) {
                    Utils.warnMsg("Failed to open file to interpolate to");
                    return;
                }
                tcxFile = dlg1.FileName;
            } else {
                return;
            }
            OpenFileDialog dlg2 = new OpenFileDialog();
            dlg2.Filter = "GPX|*.gpx";
            dlg2.Title = "Select GPX file to interpolate from";
            dlg2.Multiselect = false;
            if (dlg2.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg2.FileName == null) {
                    Utils.warnMsg("Failed to open GPX file to interpolate from");
                    return;
                }
                gpxFile = dlg2.FileName;
                interpolateTcxFromGpx(tcxFile, gpxFile, mainForm);
            }
        }

        public static void interpolateTcxFromGpx(string tcxFile, string gpxFile,
            MainForm mainForm) {
            try {
                TcxResult res =
                    ExerciseData.interpolateTcxFromGpx(tcxFile, gpxFile);
                if (res.TCX == null) {
                    Utils.errMsg("Interpolate Tcx From Gpx failed:" + NL
                        + "for " + Path.GetFileName(tcxFile) + NL
                        + " and " + Path.GetFileName(gpxFile) + NL
                        + res.Message);
                    return;
                }
                TrainingCenterDatabase tcxInterp = res.TCX;

                string saveFileName = getSaveName(tcxFile, ".interpolated");
                if (saveFileName != null) {
                    tcxInterp.Save(saveFileName);
                    mainForm.writeInfo(NL + "Recalculated " + tcxFile + NL
                        + "  from " + gpxFile + NL
                        + "  Output is " + saveFileName
                        + NL + "  " + res.Message);
                } else {
                    return;
                }
            } catch (Exception ex) {
                Utils.excMsg("Error interpolating TCX from GPX", ex);
                return;
            }
        }

        public static void deleteTcxTrackpoints(MainForm mainForm) {
            string tcxFile = null;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TCX|*.tcx";
            dlg.Title = "Select TCX file to delete trackpoints from";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileName == null) {
                    Utils.warnMsg("Failed to open file to delete trackpoints from");
                    return;
                }
                tcxFile = dlg.FileName;
                deleteTcxTrackpoints(tcxFile, mainForm);
            }
        }

        public static void deleteTcxTrackpoints(string tcxFile, MainForm mainForm) {
            try {
                TcxResult res =
                    ExerciseData.deleteTcxTrackpoints(tcxFile);
                if (res.TCX == null) {
                    Utils.errMsg("Delete trackpoints from TCX failed:" + NL
                        + "for " + Path.GetFileName(tcxFile) + NL
                        + res.Message);
                    return;
                }
                string saveFileName = getSaveName(tcxFile, ".trimmed");
                if (saveFileName != null) {
                    res.TCX.Save(saveFileName);
                    mainForm.writeInfo(NL + "Trimmed " + tcxFile + NL
                        + "  Output is " + saveFileName
                        + NL + "  " + res.Message);
                } else {
                    return;
                }
            } catch (Exception ex) {
                Utils.excMsg("Error trackpoints from TCX", ex);
                return;
            }
        }

    }
}
