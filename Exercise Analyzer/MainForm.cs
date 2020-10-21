//#define consolidateVerbose
//#define infoShowFilenames
// #define exerciseutils

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using About;
using KEUtils;
using Newtonsoft.Json;
using ScrolledHTML;
using KEGpsUtils;

namespace Exercise_Analyzer {
    public partial class MainForm : Form {
        public static readonly String NL = Environment.NewLine;
        public const string CSV_SEP = ",";
        public enum Category { Unknown, Walking, Cycling, Workout, Other }
        private List<GpsData> exerciseDataList;
        private const bool processSilent = false;
        private static ScrolledHTMLDialog overviewDlg;

        private GpxTcxMenu gpxTcxMenu;

        public MainForm() {
            InitializeComponent();

            // Add GpxTcxMenu at position 3
            gpxTcxMenu = new GpxTcxMenu();
            gpxTcxMenu.GpxTcxEvent += onGpxTcxEvent;
            gpxTcxMenu.createMenu(menuStrip1, 2);

            writeInfo("Exercise Analyzer " + DateTime.Now);
            exerciseDataList = new List<GpsData>();
        }

        /// <summary>
        /// Appends the input plus a NL to the textBoxInfo.
        /// </summary>
        /// <param name="line"></param>
        public void writeInfo(string line) {
            textBoxInfo.AppendText(line + NL);
        }

        public List<GpsData> processFiles(MainForm app, string[] fileNames, bool silent = true) {
            writeInfo(NL + "Processing " + fileNames.Length + " files");
            if (fileNames == null) {
                writeInfo("  Error: No files given");
                return null;
            }
            List<GpsData> exerciseList = new List<GpsData>();
            GpsData data;
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
                        data = GpsData.processGpx(fileName);
                    } else if (ext.ToLower().Equals(".tcx")) {
                        app.writeInfo(NL + "Processing " + fileName);
                        data = GpsData.processTcx(fileName);
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
            GpsData gpxData;
            List<GpsData> gpxDataList = new List<GpsData>();
            // Find GPX files with the same name as a TCX file
            foreach (GpsData data in exerciseDataList) {
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
            List<GpsData> failedList = new List<GpsData>();
            foreach (GpsData data in gpxDataList) {
                res = exerciseDataList.Remove(data);
                if (!res) {
                    nErrors++;
                    failedList.Add(data);
                }
            }
            if (nErrors > 0) {
                writeInfo("Failed to remove " + nErrors + " Gpx files");
                foreach (GpsData data in failedList) {
                    writeInfo("    " + data.FileName);
                }
            }

            // Find files with similar start time
            IEnumerable<IGrouping<DateTime, GpsData>> groupList = exerciseDataList
               .GroupBy(data => data.StartTimeRounded)
               .Where(grp => grp.Count() > 1);
            int nDuplicates = groupList.Count();
            List<IGrouping<DateTime, GpsData>> groups = groupList.ToList();
            writeInfo("  Found " + nDuplicates + " Groups with simlilar StartTime");
            foreach (IGrouping<DateTime, GpsData> group in groups) {
                DateTime startTimeRounded = group.Key;
                writeInfo("  StartTimeRounded=" + startTimeRounded);
                foreach (GpsData data in group) {
                    string fileName = data.FileName;
                    writeInfo("    " + data.FileName + " " + data.StartTime);
                }
            }

            // Prompt for which ones to keep
            List<GpsData> removeList = new List<GpsData>();
            List<string> fileNamesList = new List<string>();
            foreach (IGrouping<DateTime, GpsData> group in groups) {
                DateTime startTimeRounded = group.Key;
                List<string> filesList = new List<string>(group.Count());
                foreach (GpsData data in group) {
                    filesList.Add(data.FileName);
                }
                MultiChoiceCheckDialog dlg = new MultiChoiceCheckDialog(filesList);
                dlg.Label = "There are duplicate files for StartTime near "
                    + startTimeRounded + NL + "Select which files to use:";
                DialogResult dialogRes = dlg.ShowDialog();
                if (dialogRes == System.Windows.Forms.DialogResult.Abort) break;
                if (dialogRes == System.Windows.Forms.DialogResult.Cancel) continue;
                List<Result> results = dlg.Results;
                foreach (Result result in results) {
                    if (!result.Checked) {
                        GpsData data =
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
            failedList = new List<GpsData>();
            foreach (GpsData data in removeList) {
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
            foreach (GpsData data in exerciseDataList) {
                if (Path.GetExtension(data.FileName.ToLower()) == ".gpx") {
                    nGpx++;
                } else if (Path.GetExtension(data.FileName.ToLower()) == ".tcx") {
                    nTcx++;
                }
            }
            writeInfo("  nItems=" + exerciseDataList.Count
            + " NGpx=" + nGpx + " NTcx=" + nTcx);
            if (verbose) {
                foreach (GpsData data in exerciseDataList) {
                    writeInfo("      " + data.StartTime + " \t" + Path.GetFileName(data.FileName));
                }
            }
        }

        public void createWeeklyReport(string fileName) {
            if (exerciseDataList == null) return;
            string[] csvColumnNames1 = new string[] { "Year", "Month", "Week",
                "Week Start Date",
                "Walking", "Walking", "Walking", "Cycling", "Cycling", "Cycling",
                "Workout", "Workout", "Workout", "Other", "Other", "Other",
                "Total", "Total", "Total"};
            string[] csvColumnNames2 = new string[] { "", "", "",
                "",
                "time", "min", "mi", "time", "min", "mi",
                "time", "min", "mi", "time", "min", "mi",
                "time", "min", "mi"};
            CultureInfo ci = new CultureInfo("en-US");

            // Make a new list of the required information
            List<Breakdown> bdList = new List<Breakdown>();
            foreach (GpsData data in exerciseDataList) {
                if (String.IsNullOrEmpty(data.Category)) {
                    writeInfo(NL + "createWeeklyReport: No category for "
                        + data.FileName);
                }
                bdList.Add(new Breakdown(ci, data.StartTime, data.Category,
                    data.Duration, data.Distance));
            }

            // Group
            var groupList = bdList
                .GroupBy(x => new { x.StartTime, x.Year, x.WeekOfYear, x.Category, x.Duration, x.Distance })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.StartTime.Month).
                    ThenBy(g => g.Key.WeekOfYear)
                .Select(g => new {
                    StartTime = g.Key.StartTime,
                    Year = g.Key.Year,
                    WeekOfYear = g.Key.WeekOfYear,
                    Category = g.Key.Category,
                    Duration = g.Key.Duration,
                    Distance = g.Key.Distance
                });

            try {
                using (StreamWriter sw = new StreamWriter(fileName)) {
                    foreach (string col in csvColumnNames1) {
                        sw.Write(col + CSV_SEP);
                    }
                    sw.Write(NL);
                    foreach (string col in csvColumnNames2) {
                        sw.Write(col + CSV_SEP);
                    }
                    sw.Write(NL);
                    int curYear = -1;
                    string curMonth = "";
                    int curWeek = -1;
                    DateTime curStartOfWeek = DateTime.MinValue;
                    TimeSpan totalWalkingDuration = TimeSpan.FromSeconds(0);
                    double totalWalkingDistance = 0;
                    TimeSpan totalCyclingDuration = TimeSpan.FromSeconds(0);
                    double totalCyclingDistance = 0;
                    TimeSpan totalWorkoutDuration = TimeSpan.FromSeconds(0);
                    double totalWorkoutDistance = 0;
                    TimeSpan totalOtherDuration = TimeSpan.FromSeconds(0);
                    double totalOtherDistance = 0;
                    TimeSpan totalDuration = TimeSpan.FromSeconds(0);
                    double totalDistance = 0;
                    bool dataToWrite = false;
                    foreach (var group in groupList) {
                        if (group.Year != curYear) {
                            if (dataToWrite) {
                                writeWeeklyReportData(sw, curYear, curMonth, curWeek, curStartOfWeek,
                                totalWalkingDuration, totalWalkingDistance,
                                totalCyclingDuration, totalCyclingDistance,
                                totalWorkoutDuration, totalWorkoutDistance,
                                totalOtherDuration, totalOtherDistance,
                                totalDuration, totalDistance);
                            }
                            dataToWrite = true;
                            curYear = group.Year;
                            curMonth = group.StartTime.ToString("MMMM");
                            curWeek = group.WeekOfYear;
                            curStartOfWeek = GpsData.getStartOfWeek(group.StartTime);
                            totalWalkingDistance = 0;
                            totalCyclingDistance = 0;
                            totalWorkoutDistance = 0;
                            totalOtherDistance = 0;
                            totalDistance = 0;
                            totalWalkingDuration = TimeSpan.FromSeconds(0);
                            totalCyclingDuration = TimeSpan.FromSeconds(0);
                            totalWorkoutDuration = TimeSpan.FromSeconds(0);
                            totalOtherDuration = TimeSpan.FromSeconds(0);
                            totalDuration = TimeSpan.FromSeconds(0);
                        } else if (group.WeekOfYear != curWeek) {
                            // Collect data by the week
                            if (dataToWrite) {
                                writeWeeklyReportData(sw, curYear, curMonth, curWeek, curStartOfWeek,
                                totalWalkingDuration, totalWalkingDistance,
                                totalCyclingDuration, totalCyclingDistance,
                                totalWorkoutDuration, totalWorkoutDistance,
                                totalOtherDuration, totalOtherDistance,
                                totalDuration, totalDistance);
                            }
                            curMonth = group.StartTime.ToString("MMMM");
                            curWeek = group.WeekOfYear;
                            curStartOfWeek = GpsData.getStartOfWeek(group.StartTime);
                            totalWalkingDistance = 0;
                            totalCyclingDistance = 0;
                            totalWorkoutDistance = 0;
                            totalOtherDistance = 0;
                            totalDistance = 0;
                            totalWalkingDuration = TimeSpan.FromSeconds(0);
                            totalCyclingDuration = TimeSpan.FromSeconds(0);
                            totalWorkoutDuration = TimeSpan.FromSeconds(0);
                            totalOtherDuration = TimeSpan.FromSeconds(0);
                            totalDuration = TimeSpan.FromSeconds(0);
                        }
                        switch (group.Category) {
                            case Category.Walking:
                                totalWalkingDistance += group.Distance;
                                totalWalkingDuration += group.Duration;
                                totalDistance += group.Distance;
                                totalDuration += group.Duration;
                                break;
                            case Category.Cycling:
                                totalCyclingDistance += group.Distance;
                                totalCyclingDuration += group.Duration;
                                totalDistance += group.Distance;
                                totalDuration += group.Duration;
                                break;
                            case Category.Workout:
                                totalWorkoutDistance += group.Distance;
                                totalWorkoutDuration += group.Duration;
                                totalDistance += group.Distance;
                                totalDuration += group.Duration;
                                break;
                            case Category.Other:
                                totalOtherDistance += group.Distance;
                                totalOtherDuration += group.Duration;
                                totalDistance += group.Distance;
                                totalDuration += group.Duration;
                                break;
                        }
                    }
                    if (dataToWrite) {
                        writeWeeklyReportData(sw, curYear, curMonth, curWeek, curStartOfWeek,
                        totalWalkingDuration, totalWalkingDistance,
                        totalCyclingDuration, totalCyclingDistance,
                        totalWorkoutDuration, totalWorkoutDistance,
                        totalOtherDuration, totalOtherDistance,
                        totalDuration, totalDistance);
                    }
                }
                writeInfo(NL + "Wrote Weekly Summary CSV file " + fileName);
            } catch (Exception ex) {
                writeInfo(NL + "Error writing Weekly Summary CSV file " + fileName);
                Utils.excMsg("Error writing Weekly Summary CSV file " + fileName, ex);
                return;
            }
        }

        private void writeWeeklyReportData(StreamWriter sw, int curYear,
            string curMonth, int curWeek, DateTime curStartOfWeek,
            TimeSpan totalWalkingDuration, double totalWalkingDistance,
            TimeSpan totalCyclingDuration, double totalCyclingDistance,
            TimeSpan totalWorkoutDuration, double totalWorkoutDistance,
            TimeSpan totalOtherDuration, double totalOtherDistance,
            TimeSpan totalDuration, double totalDistance) {
            sw.Write(curYear + CSV_SEP); // year
            sw.Write(curMonth + CSV_SEP); // month
            sw.Write(curWeek + CSV_SEP); // week
            sw.Write($"{GpsData.formatTimeWeekday(curStartOfWeek)}" + CSV_SEP);

            sw.Write($"{GpsData.formatDuration(totalWalkingDuration)}" + CSV_SEP
               + $"{GpsData.formatDurationMinutes(totalWalkingDuration)}" + CSV_SEP
               + $"{GpsData.M2MI * totalWalkingDistance:f1}" + CSV_SEP);
            sw.Write($"{GpsData.formatDuration(totalCyclingDuration)}" + CSV_SEP
               + $"{GpsData.formatDurationMinutes(totalCyclingDuration)}" + CSV_SEP
               + $"{GpsData.M2MI * totalCyclingDistance:f1}" + CSV_SEP);
            sw.Write($"{GpsData.formatDuration(totalWorkoutDuration)}" + CSV_SEP
               + $"{GpsData.formatDurationMinutes(totalWorkoutDuration)}" + CSV_SEP
               + $"{GpsData.M2MI * totalWorkoutDistance:f1}" + CSV_SEP);
            sw.Write($"{GpsData.formatDuration(totalOtherDuration)}" + CSV_SEP
               + $"{GpsData.formatDurationMinutes(totalOtherDuration)}" + CSV_SEP
               + $"{GpsData.M2MI * totalOtherDistance:f1}" + CSV_SEP);
            sw.Write($"{GpsData.formatDuration(totalDuration)}" + CSV_SEP
               + $"{GpsData.formatDurationMinutes(totalDuration)}" + CSV_SEP
               + $"{GpsData.M2MI * totalDistance:f1}" + CSV_SEP);
            sw.Write(NL);
        }

        public void createCsv(string fileName) {
            if (exerciseDataList == null) return;
            string[] csvColumnNames = new string[] {"Category", "Location",
                "Start", "Finish", "Time Zone", "Distance", "Duration", "Duration(s)",
                "Avg Speed", "Min HR", "Avg HR", "Max HR"};

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
                    foreach (GpsData data in exerciseDataList) {
                        sw.Write(data.Category + CSV_SEP); // category
                        sw.Write(data.Location + CSV_SEP);  // location
                        sw.Write(GpsData.formatTime(data.StartTime) + CSV_SEP);  // start
                        sw.Write(GpsData.formatTime(data.EndTime) + CSV_SEP);  // end
                        sw.Write(GpsData.formatTimeZone(data.StartTime, data.TZId) + CSV_SEP);  // end
                        sw.Write($"{GpsData.M2MI * data.Distance:f2}" + CSV_SEP);  // distance
                        sw.Write(GpsData.formatDuration(data.Duration) + CSV_SEP);  // duration
                        sw.Write(data.Duration.TotalSeconds + CSV_SEP);  // duration(s)
                        sw.Write(GpsData.formatSpeed(data.SpeedAvg) + CSV_SEP);  // avg speed
                        sw.Write(GpsData.formatHeartRate(data.HrMin) + CSV_SEP);  // min heart rate
                        sw.Write(GpsData.formatHeartRateAvg(data.HrAvg) + CSV_SEP);  // avg heart rate
                        sw.Write(GpsData.formatHeartRate(data.HrMax) + CSV_SEP);  // max heart rate
                        sw.Write(NL);
                    }
                    writeInfo(NL + "Wrote STL CSV " + fileName);
                }
            } catch (Exception ex) {
                Utils.excMsg("Error writing STL CSV file " + fileName, ex);
                return;
            }
        }

        public void createStlCsv(string fileName) {
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
                    foreach (GpsData data in exerciseDataList) {
                        sw.Write(CSV_SEP);  // id
                        sw.Write(data.Category + CSV_SEP); // category
                        sw.Write(CSV_SEP);  //event
                        sw.Write(data.Location + CSV_SEP);  // location
                        sw.Write(CSV_SEP);  // tags
                        sw.Write(data.StartTime.Year + CSV_SEP);  // year
                        // STL uses 0-based month number
                        sw.Write(GpsData.formatMonthStl(data.StartTime) + CSV_SEP); // month
                        sw.Write(cal.GetWeekOfYear(data.StartTime, cwr, dow) + CSV_SEP);  // week of year
                        sw.Write(GpsData.formatTimeStl(data.StartTime) + CSV_SEP);  // start
                        sw.Write(GpsData.formatTimeStl(data.EndTime) + CSV_SEP);  // end
                        sw.Write($"{GpsData.M2MI * data.Distance:f2}" + CSV_SEP);  // distance
                        sw.Write(GpsData.formatDuration(data.Duration) + CSV_SEP);  // duration
                        sw.Write(data.Duration.TotalSeconds + CSV_SEP);  // duration(s)
                        sw.Write(CSV_SEP);  // calories
                        sw.Write(GpsData.formatSpeed(data.SpeedAvg) + CSV_SEP);  // ave speed
                        sw.Write(GpsData.formatPace(data.SpeedAvg) + CSV_SEP);  // ave pace
                        sw.Write(GpsData.formatPaceSec(data.SpeedAvg) + CSV_SEP);  // ave pace (s)
                        sw.Write(GpsData.formatSpeed(data.SpeedAvgMoving) + CSV_SEP);  // ave moving speed
                        sw.Write(GpsData.formatPace(data.SpeedAvgMoving) + CSV_SEP);  // ave moving pace
                        sw.Write(GpsData.formatPaceSec(data.SpeedAvgMoving) + CSV_SEP);  // ave moving pace(s)
                        sw.Write(GpsData.formatSpeed(data.SpeedMax) + CSV_SEP);  // max speed
                        sw.Write(GpsData.formatHeartRateStlAvg(data.HrAvg) + CSV_SEP);  // ave heart rate
                        sw.Write(GpsData.formatElevation(data.EleGain) + CSV_SEP);  // elevation gain
                        sw.Write(GpsData.formatElevation(data.EleLoss) + CSV_SEP);  // elevation loss
                        sw.Write(GpsData.formatElevation(data.EleMax) + CSV_SEP);  // elevation max
                        sw.Write(NL);
                    }
                    writeInfo(NL + "Wrote STL CSV " + fileName);
                }
            } catch (Exception ex) {
                Utils.excMsg("Error writing STL CSV file " + fileName, ex);
                return;
            }
        }

        private void onGpxTcxEvent(object sender, GpxTcxEventArgs e) {
            string msg = e.Message;
            switch (e.Type) {
                case GpxTcxMenu.EventType.MSG:
                    writeInfo(msg);
                    break;
                case GpxTcxMenu.EventType.INFO:
                    Utils.infoMsg(msg);
                    break;
                case GpxTcxMenu.EventType.WARN:
                    Utils.warnMsg(msg);
                    break;
                case GpxTcxMenu.EventType.ERR:
                    Utils.errMsg(msg);
                    break;
                case GpxTcxMenu.EventType.EXC:
                    Utils.excMsg(msg, e.Exception);
                    break;
                default:
                    Utils.errMsg("Received unknown event type (" + e.Type
                        + ") from GpsTcxMenu");
                    break;
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
                List<GpsData> list = processFiles(this, fileNames, processSilent);
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
            MultiChoiceCheckDialog dlg = new MultiChoiceCheckDialog(fileNames);
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
                    writeInfo(NL + "Exported " + dlg.FileName);
                } catch (Exception ex) {
                    writeInfo(NL + "Error exporting file " + dlg.FileName);
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
                    List<GpsData> dataList =
                        JsonConvert.DeserializeObject<List<GpsData>>(json);
                    if (dataList != null) {
                        exerciseDataList.AddRange(dataList);
                    } else {
                        writeInfo(NL + "Failed to convert  " + dlg.FileName);
                        Utils.errMsg("Failed to convert " + dlg.FileName);
                        return;
                    }
                    writeInfo(NL + "Imported " + dlg.FileName);
                } catch (Exception ex) {
                    writeInfo(NL + "Error importing file " + dlg.FileName);
                    Utils.excMsg("Error importing file " + dlg.FileName, ex);
                    return;
                }
            }
        }

        private void file_WeeklyReport_click(object sender, EventArgs e) {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "CSV Files|*.csv";
            dlg.Title = "Select a CSV file for Weekly Report";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                createWeeklyReport(dlg.FileName);
            }
        }

        private void file_SaveCsv_click(object sender, EventArgs e) {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "CSV Files|*.csv";
            dlg.Title = "Select a CSV file for Export";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                createCsv(dlg.FileName);
            }
        }

        private void file_SaveStlCsv_click(object sender, EventArgs e) {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "CSV Files|*.csv";
            dlg.Title = "Select a CSV file for Export";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                createStlCsv(dlg.FileName);
            }
        }

        private void data_SingleItemInfo_click(object sender, EventArgs e) {
            if (exerciseDataList == null || exerciseDataList.Count == 0) {
                KEUtils.Utils.errMsg("There are no available items");
                return;
            }
            List<string> fileList = new List<string>();
            foreach (GpsData data in exerciseDataList) {
                fileList.Add(data.FileName);
            }
            MultiChoiceListDialog dlg = new MultiChoiceListDialog(fileList);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                List<string> selectedList = dlg.SelectedList;
                if (selectedList == null || selectedList.Count == 0) {
                    KEUtils.Utils.errMsg("No items selected");
                }
                GpsData data;
                foreach (string item in selectedList) {
                    data =
                        exerciseDataList.Find(d => d.FileName.Equals(item));
                    if (data != null) writeInfo(NL + data.info());
                }
            }
        }

        private void help_Overview_click(object sender, EventArgs e) {
            // Create, show, or set visible the overview dialog as appropriate
            if (overviewDlg == null) {
                MainForm app = (MainForm)FindForm().FindForm();
                overviewDlg = new ScrolledHTMLDialog(
                    KEUtils.Utils.getDpiAdjustedSize(app, new Size(800, 600)));
                overviewDlg.Show();
            } else {
                overviewDlg.Visible = true;
            }
        }

        private void help_OverviewOnline_click(object sender, EventArgs e) {
            try {
                Process.Start("https://kenevans.net/opensource/ExerciseAnalyzer/Help/Overview.html");
            } catch (Exception ex) {
                Utils.excMsg("Failed to start browser", ex);
            }
        }
    }

    public class Breakdown {
        public DateTime StartTime { get; set; }
        public MainForm.Category Category { get; set; }
        public CultureInfo CI { get; set; }
        public TimeSpan Duration { get; set; }
        public double Distance { get; set; }

        public Breakdown(CultureInfo ci, DateTime startTime, string category,
            TimeSpan duration, double distance) {
            CI = ci;
            StartTime = startTime;
            Category = MainForm.Category.Unknown;
            if (!String.IsNullOrEmpty(category)) {
                if (category.ToLower().Equals("walking")) {
                    Category = MainForm.Category.Walking;
                } else if (category.ToLower().Equals("cycling")) {
                    Category = MainForm.Category.Cycling;
                } else if (category.ToLower().Equals("workout")) {
                    Category = MainForm.Category.Workout;
                } else {
                    Category = MainForm.Category.Other;
                }
            }
            Duration = duration;
            Distance = distance;
        }

        [JsonIgnore]
        public int Year {
            get {
                return StartTime.Year;
            }
        }

        [JsonIgnore]
        public int Month {
            get {
                return StartTime.Month;
            }
        }

        [JsonIgnore]
        public int WeekOfYear {
            get {
                Calendar cal = CI.Calendar;
                CalendarWeekRule cwr = CI.DateTimeFormat.CalendarWeekRule;
                DayOfWeek dow = CI.DateTimeFormat.FirstDayOfWeek;
                return cal.GetWeekOfYear(StartTime, cwr, dow);
            }
        }

        [JsonIgnore]
        public string MonthString {
            get {
                return StartTime.ToString("MMMM");
            }

        }
    }
}