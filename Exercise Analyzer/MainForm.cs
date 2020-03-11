//#define consolidateVerbose
//#define infoShowFilenames
// #define exerciseutils

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using About;
using KEUtils;
using Newtonsoft.Json;

namespace Exercise_Analyzer {
    public partial class MainForm : Form {
        public static readonly String NL = Environment.NewLine;
        public const string CSV_SEP = ",";
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

        public List<ExerciseData> processFiles(MainForm app, string[] fileNames, bool silent = true) {
            writeInfo(NL + "Processing " + fileNames.Length + " files");
            if (fileNames == null) {
                writeInfo("  Error: No files given");
                return null;
            }
            List<ExerciseData> exerciseList = new List<ExerciseData>();
            ExerciseData data;
            int nFailed = 0;
            foreach (string fileName in fileNames) {
                data = null;
                try {
                    string ext = Path.GetExtension(fileName);
                    if (String.IsNullOrEmpty(ext)) {
                        app.writeInfo(fileName + ": Extension must be GPX or TCX");
                        nFailed++;
                        continue;
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
                    nFailed++;
                    continue;
                }
            }
            writeInfo(NL + "End processing files: Failed: " + nFailed);
            return exerciseList;
        }

        private void consolidateData() {
            writeInfo(NL + "Consolidating the data");
            sortData();
            ExerciseData gpxData;
            List<ExerciseData> gpxDataList = new List<ExerciseData>();
            // Find GPX files with the same name as a TCX file
            foreach (ExerciseData data in exerciseDataList) {
                if (data.IsTcx) {
                    gpxData = exerciseDataList.Find(item =>
                        item.Extension.ToLower() == ".gpx" &&
                        item.FileNameWithoutExtension == data.FileNameWithoutExtension
                    );
#if consolidateVerbose
                    if(gpxData != null) {
                        writeInfo(NL + "Comparing " + data.SimpleFileName);
                        writeInfo(compareData(data, gpxData));
                    }
#endif
                    if (gpxData != null) gpxDataList.Add(gpxData);
                }
            }
            writeInfo("  Found " + gpxDataList.Count +
                " GPX file names matching TCX file names");
            // Remove the matching GPX files
            bool res;
            int nErrors = 0;
            List<ExerciseData> failedList = new List<ExerciseData>();
            foreach (ExerciseData data in gpxDataList) {
                res = exerciseDataList.Remove(data);
                if (!res) {
                    nErrors++;
                    failedList.Add(data);
                }
            }
            if (nErrors > 0) {
                writeInfo("Failed to remove " + nErrors + " Gpx files");
                foreach (ExerciseData data in failedList) {
                    writeInfo("    " + data.FileName);
                }
            }

            // Find files with similar start time
            IEnumerable<IGrouping<DateTime, ExerciseData>> groupList = exerciseDataList
               .GroupBy(data => data.StartTimeRounded)
               .Where(grp => grp.Count() > 1);
            int nDuplicates = groupList.Count();
            List<IGrouping<DateTime, ExerciseData>> groups = groupList.ToList();
            writeInfo("  Found " + nDuplicates + " Groups with simlilar StartTime");
            foreach (IGrouping<DateTime, ExerciseData> group in groups) {
                DateTime startTimeRounded = group.Key;
                writeInfo("  StartTimeRounded=" + startTimeRounded);
                foreach (ExerciseData data in group) {
                    string fileName = data.FileName;
                    writeInfo("    " + data.FileName + " " + data.StartTime);
                }
            }

            // Prompt for which ones to keep
            List<ExerciseData> removeList = new List<ExerciseData>();
            List<string> fileNamesList = new List<string>();
            foreach (IGrouping<DateTime, ExerciseData> group in groups) {
                DateTime startTimeRounded = group.Key;
                List<string> filesList = new List<string>(group.Count());
                foreach (ExerciseData data in group) {
                    filesList.Add(data.FileName);
                }
                MultiChoiceDialog dlg = new MultiChoiceDialog(filesList);
                dlg.Label = "There are duplicate files for StartTime near "
                    + startTimeRounded + NL + "Select which files to use:";
                DialogResult dialogRes = dlg.ShowDialog();
                if (dialogRes == System.Windows.Forms.DialogResult.Abort) break;
                if (dialogRes == System.Windows.Forms.DialogResult.Cancel) continue;
                List<Result> results = dlg.Results;
                foreach (Result result in results) {
                    if (!result.Checked) {
                        ExerciseData data =
                            exerciseDataList.Find(d => d.FileName.Equals(result.FileName));
                        if (data != null) {
                            removeList.Add(data);
                        } else {
                            // Should not happen
                            writeInfo("  " + startTimeRounded + ": Did not find "
                                + result.FileName + " in the data list");
                        }
                    }
                }
            }
            // Remove the duplicates
            writeInfo("  Removing " + removeList.Count + " duplicates");
            nErrors = 0;
            failedList = new List<ExerciseData>();
            foreach (ExerciseData data in removeList) {
                res = exerciseDataList.Remove(data);
                if (!res) {
                    nErrors++;
                    failedList.Add(data);
                }
            }
            if (nErrors > 0) {
                writeInfo("  Failed to remove " + nErrors + " duplicate files");
            }
        }

        private void sortData() {
            exerciseDataList.Sort((a, b) => {
                int res = DateTime.Compare(a.StartTime, b.StartTime);
                if (res != 0) return res;
                return string.Compare(Path.GetExtension(a.FileName),
                    Path.GetExtension(b.FileName));
            });
        }

        private void dataListSummary(bool verbose = false) {
            writeInfo(NL + "Exercise Data List");
            int nGpx = 0, nTcx = 0;
            foreach (ExerciseData data in exerciseDataList) {
                if (Path.GetExtension(data.FileName.ToLower()) == ".gpx") {
                    nGpx++;
                } else if (Path.GetExtension(data.FileName.ToLower()) == ".tcx") {
                    nTcx++;
                }
            }
            writeInfo("  nItems=" + exerciseDataList.Count
            + " NGpx=" + nGpx + " NTcx=" + nTcx);
            if (verbose) {
                foreach (ExerciseData data in exerciseDataList) {
                    writeInfo("      " + data.StartTime + " \t" + Path.GetFileName(data.FileName));
                }
            }
        }

        public void createCsv(string fileName) {
            if (exerciseDataList == null) return;
            string[] csvColumnNames = new string[] { "id", "category",
                "event", "location", "tags", "year", "month", "week of year",
                "start", "finish", "distance", "duration", "duration(s)",
                "calories", "ave speed", "ave pace", "ave pace(s)",
                "ave moving speed", "ave moving pace", "ave moving pace(s)", "max speed",
                "ave heart rate", "elevation gain", "elevation loss", "max elevation"};

            // From https://docs.microsoft.com/en-us/dotnet/api/system.globalization.calendar.getweekofyear?view=netframework-4.8
            CultureInfo ci = new CultureInfo("en-US");
            Calendar cal = ci.Calendar;
            CalendarWeekRule cwr = ci.DateTimeFormat.CalendarWeekRule;
            DayOfWeek dow = ci.DateTimeFormat.FirstDayOfWeek;

            try {
                using (StreamWriter sw = new StreamWriter(fileName)) {
                    foreach (string col in csvColumnNames) {
                        sw.Write(col + CSV_SEP);
                    }
                    sw.Write(NL);
                    foreach (ExerciseData data in exerciseDataList) {
                        sw.Write(CSV_SEP);  // id
                        sw.Write(data.Category + CSV_SEP); // category
                        sw.Write(CSV_SEP);  //event
                        sw.Write(data.Location + CSV_SEP);  // location
                        sw.Write(CSV_SEP);  // tags
                        sw.Write(data.StartTime.Year + CSV_SEP);  // year
                        // STL uses 0-based month number
                        sw.Write(ExerciseData.formatMonthStl(data.StartTime) + CSV_SEP); // month
                        sw.Write(cal.GetWeekOfYear(data.StartTime, cwr, dow) + CSV_SEP);  // week of year
                        sw.Write(ExerciseData.formatTimeStl(data.StartTime) + CSV_SEP);  // start
                        sw.Write(ExerciseData.formatTimeStl(data.EndTime) + CSV_SEP);  // end
                        sw.Write($"{GpsUtils.M2MI * data.Distance:f2}" + CSV_SEP);  // distance
                        sw.Write(ExerciseData.formatDuration(data.Duration) + CSV_SEP);  // duration
                        sw.Write(data.Duration.TotalSeconds + CSV_SEP);  // duration(s)
                        sw.Write(CSV_SEP);  // calories
                        sw.Write(ExerciseData.formatSpeed(data.SpeedAvg) + CSV_SEP);  // ave speed
                        sw.Write(ExerciseData.formatPace(data.SpeedAvg) + CSV_SEP);  // ave pace
                        sw.Write(ExerciseData.formatPaceSec(data.SpeedAvg) + CSV_SEP);  // ave pace (s)
                        sw.Write(ExerciseData.formatSpeed(data.SpeedAvgMoving) + CSV_SEP);  // ave moving speed
                        sw.Write(ExerciseData.formatPace(data.SpeedAvgMoving) + CSV_SEP);  // ave moving pace
                        sw.Write(ExerciseData.formatPaceSec(data.SpeedAvgMoving) + CSV_SEP);  // ave moving pace(s)
                        sw.Write(ExerciseData.formatSpeed(data.SpeedMax) + CSV_SEP);  // max speed
                        sw.Write(ExerciseData.formatHeartRateAvg(data.HrAvg) + CSV_SEP);  // ave heart rate
                        sw.Write(ExerciseData.formatElevation(data.EleGain) + CSV_SEP);  // elevation gain
                        sw.Write(ExerciseData.formatElevation(data.EleLoss) + CSV_SEP);  // elevation loss
                        sw.Write(ExerciseData.formatElevation(data.EleMax) + CSV_SEP);  // elevation max
                        sw.Write(NL);
                    }
                }
            } catch (Exception ex) {
                Utils.excMsg("Error writing CSV file " + fileName, ex);
                return;
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

#if exerciseutils
        private void file_ProcessSingleFile_click(object sender, EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX and TCX|*.gpx;*.tcx|GPX|*.gpx|TCX|*.tcx";
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
#endif

        private void file_ProcessFiles_click(object sender, EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GPX and TCX|*.gpx;*.tcx|GPX|*.gpx|TCX|*.tcx";
            dlg.Title = "Select files to process";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileNames == null) {
                    Utils.warnMsg("Failed to open files to process");
                    return;
                }
                string[] fileNames = dlg.FileNames;
                List<ExerciseData> list = processFiles(this, fileNames, processSilent);
                if (list != null) {
                    exerciseDataList.AddRange(list);
                }
            }
        }

        private void data_Consolidate_click(object sender, EventArgs e) {
            consolidateData();
        }

        private void data_Clear_click(object sender, EventArgs e) {
            exerciseDataList.Clear();
        }
        private void data_Sort_click(object sender, EventArgs e) {
            sortData();
        }

        private void data_Info_click(object sender, EventArgs e) {
            dataListSummary();
        }

        private void data_InfoVerbose_click(object sender, EventArgs e) {
            dataListSummary(true);
        }

        private void file_MultiChoiceTest_click(object sender, EventArgs e) {
            List<string> fileNames = new List<string>
            { "FileName 1", "FileName 2", "FileName 3", "FileName 4" };
            MultiChoiceDialog dlg = new MultiChoiceDialog(fileNames);
            dlg.Label = "Select which files to use:";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                List<Result> results = dlg.Results;
                string info = "";
                foreach (Result result in results) {
                    if (result.Checked) {
                        info += "X  " + result.FileName + NL;
                    } else {
                        info += "O  " + result.FileName + NL;
                    }
                }
                Utils.infoMsg("Check State: " + NL + info);
            }
        }

        private void data_Export_click(object sender, EventArgs e) {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Json Files|*.json";
            dlg.Title = "Select a data file for Export";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                try {
                    string json = JsonConvert.SerializeObject(exerciseDataList,
                        (((ToolStripMenuItem)sender).Text.Equals("Indented")) ?
                        Formatting.Indented :
                        Formatting.None);
                    File.WriteAllText(dlg.FileName, json);
                } catch (Exception ex) {
                    Utils.excMsg("Error exporting file " + dlg.FileName, ex);
                    return;
                }
            }
        }

        private void data_Import_click(object sender, EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Json Files|*.json";
            dlg.Title = "Select a data file to import";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                try {
                    string json = File.ReadAllText(dlg.FileName);
                    List<ExerciseData> dataList =
                        JsonConvert.DeserializeObject<List<ExerciseData>>(json);
                    if (dataList != null) {
                        exerciseDataList.AddRange(dataList);
                    } else {
                        Utils.errMsg("Failed to convert " + dlg.FileName);
                        return;
                    }
                } catch (Exception ex) {
                    Utils.excMsg("Error importing file " + dlg.FileName, ex);
                    return;
                }
            }
        }

        private void file_SaveCsv_click(object sender, EventArgs e) {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "CSV Files|*.csv";
            dlg.Title = "Select a CSV file for Export";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                try {
                    createCsv(dlg.FileName);
                } catch (Exception ex) {
                    Utils.excMsg("Error saving CSV file " + dlg.FileName, ex);
                    return;
                }
            }
        }

        private void data_SingleItemInfo_click(object sender, EventArgs e) {

        }
    }
}
